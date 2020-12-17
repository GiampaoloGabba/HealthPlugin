using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Java.Util.Concurrent;
using Org.Json;
using Debug = System.Diagnostics.Debug;
using Task = Android.Gms.Tasks.Task;

namespace Plugin.Health
{
    [Preserve(AllMembers = true)]
    public class HealthDataReader : HealthDataReaderBase
    {
        //Info su oauth e googlefit
        //https://developers.google.com/fit/android/get-started
        //https://developers.google.com/fit/android/get-api-key
        //https://developers.google.com/fit/android/authorization#android-10

        //Esempi di utilizzo GoogleFit
        //aggregation: https://github.com/dariosalvi78/cordova-plugin-health/blob/master/src/android/HealthPlugin.java
        //datatypes: https://developers.google.com/android/reference/com/google/android/gms/fitness/data/DataType#TYPE_STEP_COUNT_DELTA

        //TODO gestire calorie basali nel bucket time

        readonly Activity _currentActivity;
        readonly HealthService _healthService;

        public HealthDataReader(HealthService healthService)
        {
            _healthService = healthService;
            _currentActivity = healthService.CurrentActivity;
        }

        protected override async Task<IEnumerable<T>> Query<T>(HealthDataType healthDataType,
                                                               AggregateTime aggregateTime,
                                                               DateTime startDate, DateTime endDate)
        {
            var authorized = _healthService.HasOAuthPermission(_healthService.FitnessReadOptions(new HealthDataType[] { healthDataType }));

            if (!authorized)
                throw new UnauthorizedAccessException($"Not enough permissions to request {healthDataType}");

            var  fitData   = healthDataType.ToGoogleFit();
            long startTime = startDate.ToJavaTimeStamp();
            long endTime   = endDate.ToJavaTimeStamp();

            var readBuilder = new DataReadRequest.Builder()
                              .SetTimeRange(startTime, endTime, TimeUnit.Milliseconds)
                              .EnableServerQueries();

            if (aggregateTime != AggregateTime.None)
            {
                readBuilder.Aggregate(fitData.TypeIdentifier, fitData.AggregateType);

                switch (aggregateTime)
                {
                    case AggregateTime.Year:
                        readBuilder.BucketByTime(365, TimeUnit.Days);
                        break;

                    case AggregateTime.Month:
                        readBuilder.BucketByTime(31, TimeUnit.Days);
                        break;

                    case AggregateTime.Week:
                        readBuilder.BucketByTime(7, TimeUnit.Days);
                        break;

                    case AggregateTime.Day:
                        readBuilder.BucketByTime(1, TimeUnit.Days);
                        break;

                    case AggregateTime.Hour:
                        readBuilder.BucketByTime(1, TimeUnit.Hours);
                        break;
                }
            }
            else
            {
                readBuilder.Read(fitData.TypeIdentifier);
            }

            var readRequest = readBuilder.Build();

            var response = await FitnessClass.GetHistoryClient(_currentActivity, GoogleSignIn.GetLastSignedInAccount(_currentActivity))
                                             .ReadDataAsync(readRequest).ConfigureAwait(false);

            double valueToSubstract = 0;
            if (healthDataType == HealthDataType.CaloriesActive)
            {
                valueToSubstract = await GetBasalAvg(endDate);
            }

            if (response == null)
                return new List<T>();

            if (response.Buckets.Any())
            {
                var output = new List<T>();
                foreach (var bucket in response.Buckets)
                {
                    var dataSet = bucket.GetDataSet(fitData.AggregateType);
                    output.AddRange((IEnumerable<T>) dataSet.DataPoints.Select(result =>
                        CreateAggregatedData(result, fitData, valueToSubstract)));

                    if (!dataSet.DataPoints.Any() && healthDataType == HealthDataType.Droid_BasalMetabolicRate)
                    {
                        output.Add(
                            new AggregatedHealthData()
                            {
                                StartDate = bucket.GetStartTime(TimeUnit.Milliseconds).ToDateTime(),
                                EndDate   = bucket.GetEndTime(TimeUnit.Milliseconds).ToDateTime(),
                                Sum       = valueToSubstract
                            } as T);
                    }

                }

                return output;
            }

            return (IEnumerable<T>) response.GetDataSet(fitData.TypeIdentifier)?.DataPoints?
                                             .Select(dataPoint => new HealthData
                                             {
                                                 StartDate   = dataPoint.GetStartTime(TimeUnit.Milliseconds).ToDateTime(),
                                                 EndDate     = dataPoint.GetEndTime(TimeUnit.Milliseconds).ToDateTime(),
                                                 Value       = ReadValue(dataPoint, fitData.Unit, valueToSubstract) ?? 0,
                                                 UserEntered = dataPoint.OriginalDataSource?.StreamName == "user_input"
                                             }).ToList();
        }

        // utility function that gets the basal metabolic rate averaged over a week
        private async Task<double> GetBasalAvg(DateTime endDate)
        {
            float basalAvg = 0;
            long startDate = endDate.AddDays(-7).ToJavaTimeStamp();

            var readRequest = new DataReadRequest.Builder()
                              .Aggregate(DataType.TypeBasalMetabolicRate, DataType.AggregateBasalMetabolicRateSummary)
                              .BucketByTime(1, TimeUnit.Days)
                              .SetTimeRange(startDate, endDate.ToJavaTimeStamp(), TimeUnit.Milliseconds).Build();

            var response = await FitnessClass.GetHistoryClient(_currentActivity, GoogleSignIn.GetLastSignedInAccount(_currentActivity))
                                             .ReadDataAsync(readRequest);

            if (response.Status.IsSuccess)
            {
                var avgsN = 0;
                foreach (var bucket in response.Buckets)
                {
                    // in the com.google.bmr.summary data type, each data point represents
                    // the average, maximum and minimum basal metabolic rate, in kcal per day, over the time interval of the data point.
                    var dataSet = bucket.GetDataSet(DataType.AggregateBasalMetabolicRateSummary);
                    foreach (var dataPoint in dataSet.DataPoints)
                    {
                        var avg = dataPoint.GetValue(Field.FieldAverage).AsFloat();
                        if (avg > 0)
                        {
                            basalAvg += avg;
                            avgsN++;
                        }
                    }
                }

                // do the average of the averages
                if (avgsN != 0) basalAvg /= avgsN; // this a daily average
                return basalAvg;
            }

            throw new Exception(response.Status.StatusMessage);
        }

        AggregatedHealthData CreateAggregatedData(DataPoint dataPoint, GoogleFitData fitData, double valueToSubstract)
        {
            var hData = new AggregatedHealthData
            {
                StartDate = dataPoint.GetStartTime(TimeUnit.Milliseconds).ToDateTime(),
                EndDate   = dataPoint.GetEndTime(TimeUnit.Milliseconds).ToDateTime(),
            };

            if (fitData.Cumulative)
            {
                hData.Sum = ReadValue(dataPoint, fitData.Unit, valueToSubstract);
            }
            else
            {
                hData.Min = ReadValue(dataPoint, fitData.MinOverride ?? Field.FieldMin, valueToSubstract);
                hData.Max = ReadValue(dataPoint, fitData.MaxOverride ?? Field.FieldMax, valueToSubstract);
                hData.Average = ReadValue(dataPoint, fitData.AverageOverride ?? Field.FieldAverage, valueToSubstract);
            }

            return hData;
        }

        double? ReadValue(DataPoint dataPoint, Field unit, double valueToSubstract)
        {
            double? output = 0;
            try
            {
                //need to pass type of field
                //return dataPoint.GetValue(Field.FieldAverage).AsFloat();
                output = dataPoint.GetValue(unit).AsFloat();
            }
            catch
            {
                try
                {
                    output = dataPoint.GetValue(unit).AsInt();
                }
                catch
                {
                    try
                    {
                        output = dataPoint.GetValue(unit).AsString().ToDbl();
                    }
                    catch (Exception e3)
                    {
                        Debug.WriteLine(e3,"GoogleFit - Unable to convert datatype format");
                        output = null;
                    }
                }
            }

            if (output != null)
            {
                output = Math.Max(0, output.Value - valueToSubstract);
            }

            return output;
        }
    }
}
