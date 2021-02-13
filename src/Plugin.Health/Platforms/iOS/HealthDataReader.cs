using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HealthKit;

namespace Plugin.Health
{
    public class HealthDataReader : HealthDataReaderBase
    {
        //TODO: gestire calorie totali

        readonly HealthService _healthService;
        readonly HKHealthStore _healthStore;

        public HealthDataReader(HealthService healthService)
        {
            _healthService = healthService;
            _healthStore = healthService.HealthStore;
        }

        protected override Task<IEnumerable<T>> Query<T>(HealthDataType healthDataType,
                                                         AggregateTime aggregateTime,
                                                         DateTime startDate, DateTime endDate)
        {
            if (_healthStore == null || !HKHealthStore.IsHealthDataAvailable)
                throw new NotSupportedException("HealthKit data is not available on this device");

            var authorized = IsAuthorizedToRead(healthDataType);

            if (!authorized)
                throw new UnauthorizedAccessException($"Not enough permissions to request {healthDataType}");

            var taskComplSrc = new TaskCompletionSource<IEnumerable<T>>();
            var healthKitType = healthDataType.ToHealthKit();
            var quantityType = HKQuantityType.Create(healthKitType.QuantityTypeIdentifier);
            var predicate = HKQuery.GetPredicateForSamples((NSDate) startDate, (NSDate) endDate, HKQueryOptions.StrictStartDate);

            if (aggregateTime != AggregateTime.None)
            {
                var anchor   = NSCalendar.CurrentCalendar.DateBySettingsHour(0, 0, 0, NSDate.Now, NSCalendarOptions.None);
                var interval = new NSDateComponents();

                switch (aggregateTime)
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
                }

                HKStatisticsOptions hkStatisticsOptions;

                if (healthKitType.Cumulative)
                {
                    hkStatisticsOptions = HKStatisticsOptions.CumulativeSum;
                }
                else
                {
                    hkStatisticsOptions = HKStatisticsOptions.DiscreteAverage |
                                          HKStatisticsOptions.DiscreteMax |
                                          HKStatisticsOptions.DiscreteMax;
                }

                var queryAggregate = new HKStatisticsCollectionQuery(quantityType, predicate, hkStatisticsOptions,
                    anchor, interval)
                {
                    InitialResultsHandler = (collectionQuery, results, error) =>
                    {
                        var healthData = new List<T>();

                        foreach (var result in results.Statistics)
                        {
                            var hData = new AggregatedHealthData
                            {
                                StartDate = (DateTime) result.StartDate,
                                EndDate   = (DateTime) result.EndDate,
                            };

                            if (healthKitType.Cumulative)
                            {
                                hData.Sum = result.SumQuantity().GetDoubleValue(healthKitType.Unit);
                            }
                            else
                            {
                                hData.Min = result.MinimumQuantity().GetDoubleValue(healthKitType.Unit);
                                hData.Max = result.MaximumQuantity().GetDoubleValue(healthKitType.Unit);
                                hData.Average = result.AverageQuantity().GetDoubleValue(healthKitType.Unit);
                            }

                            healthData.Add(hData as T);
                        }

                        taskComplSrc.SetResult(healthData);
                    }
                };

                _healthStore.ExecuteQuery(queryAggregate);
            }
            else
            {
                var sortDescriptor = new[] {new NSSortDescriptor(HKSample.SortIdentifierEndDate, true)};


                HKSampleType sampleType;

                if(healthKitType.HKType == HealthKitData.HKTypes.Category)
                {
                    sampleType = HKCategoryType.Create(healthKitType.CategoryTypeIdentifier);
                } else if(healthKitType.HKType == HealthKitData.HKTypes.Quantity)
                {
                    sampleType = HKQuantityType.Create(healthKitType.QuantityTypeIdentifier);
                } else if(healthKitType.HKType == HealthKitData.HKTypes.Workout)
                {
                    sampleType = HKSampleType.GetWorkoutType();
                }
                else
                {
                    throw new NotSupportedException();
                }

                var query = new HKSampleQuery(sampleType, predicate,
                    HKSampleQuery.NoLimit, sortDescriptor,
                    (resultQuery, results, error) =>
                    {

                        IEnumerable<T> healthData = default(IEnumerable<T>);

                        if (sampleType == HKSampleType.GetWorkoutType())
                        {
                            healthData = results?.Select(result => new WorkoutData
                            {
                                StartDate = (DateTime)result.StartDate,
                                EndDate = (DateTime)result.EndDate,
                                Duration = (result as HKWorkout).Duration,
                                Device = (result as HKWorkout).Device?.ToString(),
                                WorkoutType = (result as HKWorkout).WorkoutDataType()
                                //TotalDistance = Convert.ToDouble((result as HKWorkout).TotalDistance),
                                //TotalEnergyBurned = Convert.ToDouble((result as HKWorkout).TotalEnergyBurned)


                            } as T); 
                        }
                        else
                        {
                            healthData = results?.Select(result => new HealthData
                            {


                                StartDate = (DateTime)result.StartDate,
                                EndDate = (DateTime)result.EndDate,
                                Value = ReadValue(result, healthKitType.Unit),
                                UserEntered = result.Metadata?.WasUserEntered ?? false,

                            } as T);

                        }

                       

                        taskComplSrc.SetResult(healthData);
                    });

                _healthStore.ExecuteQuery(query);
            }

            return taskComplSrc.Task;
        }

        bool IsAuthorizedToRead(HealthDataType healthDataType)
        {
            if (_healthService.IsDataTypeAvailable(healthDataType))
            {
                //Note: authorizationStatus is to determine the access status only to write but not to read.
                //There is no option to know whether your app has read access.
                //https://stackoverflow.com/a/29128231/9823528

                //So for now i just return true if the OS supports the datatype
                return true;
            }

            return false;
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
    }
}
