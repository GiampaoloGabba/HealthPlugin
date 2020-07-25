using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HealthKit;

namespace Plugin.Health
{
    public class HealthService : BaseHealthService
    {
        readonly HKHealthStore _healthStore;

        public HealthService()
        {
            if (HKHealthStore.IsHealthDataAvailable)
                _healthStore = new HKHealthStore();
            else
                Debug.WriteLine("HEALTHKIT is not available on this device!");
        }

        public override bool IsDataTypeAvailable(HealthDataType healthDataType)
        {
            try
            {
                return healthDataType.ToHealthKit() != null;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.WriteLine($"HEALTHKIT - Datatype {healthDataType} is not supported in this device", e);
                return false;
            }
        }

        public override async Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes)
        {

            if (_healthStore == null)
                throw new NotSupportedException("HEALTHKIT is not available on this device!");

            var result = dataTypes.Where(IsDataTypeAvailable).ToArray();

            if (HKHealthStore.IsHealthDataAvailable && result.Any())
            {
                var (res, error) = await _healthStore.RequestAuthorizationToShareAsync(null, DataTypesToRead(result));

                if (error != null)
                    throw new Exception($"HEALTHKIT - Error during Permission request: {error.LocalizedDescription}");

                return true;
            }

            return false;
        }

        public override async Task<IEnumerable<HealthData>> FetchDataAsync(HealthDataType healthDataType)
        {
            if (_healthStore == null || !HKHealthStore.IsHealthDataAvailable)
                throw new NotSupportedException("HealthKit data is not available on this device");

            var authorized = IsAuthorizedToRead(healthDataType);

            if (!authorized)
                throw new UnauthorizedAccessException($"Not enough permissions to request {healthDataType}");

            return await QueryAsync(healthDataType, StartDate, EndDate);
        }

        Task<IEnumerable<HealthData>> QueryAsync(HealthDataType healthDataType, DateTime startDate, DateTime endDate)
        {
            var taskComplSrc = new TaskCompletionSource<IEnumerable<HealthData>>();
            var healthKitType = healthDataType.ToHealthKit();
            var quantityType = HKQuantityType.Create(healthKitType.TypeIdentifier);
            var predicate = HKQuery.GetPredicateForSamples((NSDate) startDate, (NSDate) endDate, HKQueryOptions.StrictStartDate);
            var sortDescriptor = new[] {new NSSortDescriptor(HKSample.SortIdentifierEndDate, true)};

            if (AggregateTime != AggregateTime.None)
            {
                var anchor   = NSCalendar.CurrentCalendar.DateBySettingsHour(0, 0, 0, NSDate.Now, NSCalendarOptions.None);
                var interval = new NSDateComponents();

                switch (AggregateTime)
                {
                    case AggregateTime.Year:
                        interval.Year = 1;
                        break;

                    case AggregateTime.Month:
                        interval.Month = 1;
                        break;

                    case AggregateTime.Week:
                        interval.Week = 1;
                        break;

                    case AggregateTime.Day:
                        interval.Day = 1;
                        break;

                    case AggregateTime.Hour:
                        interval.Hour = 1;
                        break;

                    case AggregateTime.Minute:
                        interval.Hour = 1;
                        break;
                }

                var queryAggregate = new HKStatisticsCollectionQuery(quantityType, predicate, HKStatisticsOptions.CumulativeSum,
                    anchor, interval)
                {
                    InitialResultsHandler = (collectionQuery, results, error) =>
                    {
                        var healthData = results.Statistics.Select(result => new HealthData
                        {
                            StartDate   = (DateTime) result.StartDate,
                            EndDate     = (DateTime) result.EndDate,
                            Value       = result.SumQuantity().GetDoubleValue(healthKitType.Unit),
                        }).ToList();

                        taskComplSrc.SetResult(healthData);
                    }
                };

                _healthStore.ExecuteQuery(queryAggregate);
            }
            else
            {
                var query = new HKSampleQuery(quantityType, predicate, HKSampleQuery.NoLimit, sortDescriptor,
                    (resultQuery, results, error) =>
                    {
                        var healthData = results?.Select(result => new HealthData
                        {
                            StartDate   = (DateTime) result.StartDate,
                            EndDate     = (DateTime) result.EndDate,
                            Value       = ReadValue(result, healthKitType.Unit),
                            UserEntered = result.Metadata?.WasUserEntered ?? false
                        }).ToList();

                        taskComplSrc.SetResult(healthData);
                    });

                _healthStore.ExecuteQuery(query);
            }

            return taskComplSrc.Task;
        }

        /*public bool IsAuthorizedToWrite(HealthData.DataType dataType)
        {
            if (IsDataTypeAvailable(dataType))
            {
                return _healthStore.GetAuthorizationStatus(HKQuantityType.Create(dataType.ToHealthKit().TypeIdentifier))
                                .HasFlag(HKAuthorizationStatus.SharingAuthorized);
            }

            return false;
        }*/

        bool IsAuthorizedToRead(HealthDataType healthDataType)
        {
            if (IsDataTypeAvailable(healthDataType))
            {
                //Note: authorizationStatus is to determine the access status only to write but not to read.
                //There is no option to know whether your app has read access.
                //https://stackoverflow.com/a/29128231/9823528

                //So for now i just return true if the OS supports the datatype
                return true;
            }

            return false;
        }

        double ReadValue(HKSample sample, HKUnit unit)
        {
            switch (sample)
            {
                case HKQuantitySample qtySample:
                    return qtySample.Quantity.GetDoubleValue(unit);
                case HKCategorySample cateSample:
                    return cateSample.Value;
                default:
                    return -1;
            }
        }

        NSSet DataTypesToRead(HealthDataType[] dataTypes)
        {
            var types = new HKObjectType[dataTypes.Length];
            for (var i = 0; i < dataTypes.Length; i++)
            {
                types.SetValue(HKQuantityType.Create(dataTypes[i].ToHealthKit().TypeIdentifier), i);
            }
            return NSSet.MakeNSObjectSet(types);
        }
    }
}
