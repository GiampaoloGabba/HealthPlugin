﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HealthKit;

namespace Plugin.Health
{
    [Preserve(AllMembers = true)]
    public class HealthDataReader : HealthDataReaderBase
    {
        readonly HealthService _healthService;
        readonly HKHealthStore _healthStore;

        public HealthDataReader(HealthService healthService)
        {
            _healthService = healthService;
            _healthStore = healthService.HealthStore;
        }

        protected override Task<IEnumerable<HealthData>> QueryAsync(HealthDataType healthDataType,
                                                                    AggregateType aggregateType,
                                                                    AggregateTime aggregateTime,
                                                                    DateTime startDate, DateTime endDate)
        {
            if (_healthStore == null || !HKHealthStore.IsHealthDataAvailable)
                throw new NotSupportedException("HealthKit data is not available on this device");

            var authorized = IsAuthorizedToRead(healthDataType);

            if (!authorized)
                throw new UnauthorizedAccessException($"Not enough permissions to request {healthDataType}");

            var taskComplSrc   = new TaskCompletionSource<IEnumerable<HealthData>>();
            var healthKitType  = healthDataType.ToHealthKit();
            var quantityType   = HKQuantityType.Create(healthKitType.TypeIdentifier);
            var predicate      = HKQuery.GetPredicateForSamples((NSDate) startDate, (NSDate) endDate, HKQueryOptions.StrictStartDate);
            var sortDescriptor = new[] {new NSSortDescriptor(HKSample.SortIdentifierEndDate, true)};

            if (aggregateTime != AggregateTime.None && aggregateType != AggregateType.None)
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

                    case AggregateTime.Minute:
                        interval.Hour = 1;
                        break;
                }

                HKStatisticsOptions hkStatisticsOptions;

                switch (aggregateType)
                {
                    case AggregateType.CumulativeSum:
                        hkStatisticsOptions = healthKitType.NegateCumulativeSum == false
                                                  ? HKStatisticsOptions.CumulativeSum
                                                  : healthKitType.DefaultStatisticOption;
                        break;

                    case AggregateType.Average:
                        hkStatisticsOptions = HKStatisticsOptions.DiscreteAverage;
                        break;

                    case AggregateType.Min:
                        hkStatisticsOptions = HKStatisticsOptions.DiscreteMin;
                        break;

                    case AggregateType.Max:
                        hkStatisticsOptions = HKStatisticsOptions.DiscreteMax;
                        break;

                    default:
                        hkStatisticsOptions = healthKitType.DefaultStatisticOption;
                        break;
                }


                var queryAggregate = new HKStatisticsCollectionQuery(quantityType, predicate, hkStatisticsOptions,
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