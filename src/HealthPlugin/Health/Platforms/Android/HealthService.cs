using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Java.Util.Concurrent;
using Plugin.CurrentActivity;
using Debug = System.Diagnostics.Debug;

namespace Plugin.Health
{
    [Preserve(AllMembers = true)]
    public class HealthService : BaseHealthService
    {
        //Info su oauth e googlefit
        //https://developers.google.com/fit/android/get-started
        //https://developers.google.com/fit/android/get-api-key
        //https://developers.google.com/fit/android/authorization#android-10

        //Esempi di utilizzo GoogleFit
        //aggregation: https://github.com/dariosalvi78/cordova-plugin-health/blob/master/src/android/HealthPlugin.java
        //datatypes: https://developers.google.com/android/reference/com/google/android/gms/fitness/data/DataType#TYPE_STEP_COUNT_DELTA

        Activity _activity => CrossCurrentActivity.Current.Activity ??
                              throw new NullReferenceException("Please call HealthService.Init() method in the platform specific project to use Health Plugin");
        Context _context => CrossCurrentActivity.Current.AppContext;
        const int REQUEST_CODE = 1;
        static TaskCompletionSource<bool> _tcsAuth;

        public override bool IsDataTypeAvailable(HealthDataType healthDataType)
        {
            try
            {
                return healthDataType.ToGoogleFit() != null;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.WriteLine($"GoogleFit - Datatype {healthDataType} is not supported in this device", e);
                return false;
            }
        }

        public override Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes)
        {
            var fitnessOptions = GetFitnessOptions(dataTypes);

            _tcsAuth = new TaskCompletionSource<bool>();

            if (HasOAuthPermission(fitnessOptions))
            {
                _tcsAuth.SetResult(true);
            }
            else
            {
                GoogleSignIn.RequestPermissions(_activity, REQUEST_CODE,
                    GoogleSignIn.GetLastSignedInAccount(_context), fitnessOptions);
            }

            return _tcsAuth.Task;
        }

        bool HasOAuthPermission(FitnessOptions fitnessOptions)
        {
            return GoogleSignIn.HasPermissions(GoogleSignIn.GetLastSignedInAccount(_context), fitnessOptions);
        }

        FitnessOptions GetFitnessOptions(params HealthDataType[] dataTypes)
        {
            var fitnessBuilder = FitnessOptions.InvokeBuilder();
            foreach (var dataType in dataTypes)
            {
                fitnessBuilder.AddDataType(dataType.ToGoogleFit().TypeIdentifier, FitnessOptions.AccessRead);
            }
            return fitnessBuilder.Build();
        }

        protected override async Task<IEnumerable<HealthData>> QueryAsync(HealthDataType healthDataType,
                                                                          AggregateType aggregateType,
                                                                          AggregateTime aggregateTime,
                                                                          DateTime startDate, DateTime endDate)
        {
            var authorized = HasOAuthPermission(GetFitnessOptions(healthDataType));

            if (!authorized)
                throw new UnauthorizedAccessException($"Not enough permissions to request {healthDataType}");

            var googleFitData     = healthDataType.ToGoogleFit();
            var googleFitDataType = googleFitData.TypeIdentifier;
            var startTime         = startDate.ToJavaTimeStamp();
            var endTime           = endDate.ToJavaTimeStamp();

            var readBuilder = new DataReadRequest.Builder()
                              .SetTimeRange(startTime, endTime, TimeUnit.Milliseconds)
                              .EnableServerQueries();

            if (aggregateTime != AggregateTime.None && googleFitData.AggregateTypeIdentifier != null)
            {
                readBuilder.Aggregate(googleFitDataType, googleFitData.AggregateTypeIdentifier);

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

                    case AggregateTime.Minute:
                        readBuilder.BucketByTime(1, TimeUnit.Minutes);
                        break;
                }
            }
            else
            {
                readBuilder.Read(googleFitDataType);
            }

            var readRequest = readBuilder.Build();

            var response = await FitnessClass.GetHistoryClient(_activity, GoogleSignIn.GetLastSignedInAccount(_activity))
                                             .ReadDataAsync(readRequest).ConfigureAwait(false);

            if (response == null)
                return new List<HealthData>();

            if (response.Buckets.Any())
            {
                var output = new List<HealthData>();
                foreach (var bucket in response.Buckets)
                {
                    foreach (var dataSet in bucket.DataSets)
                    {
                        output.AddRange(dataSet.DataPoints.Select(result => CreateHealthData(result, googleFitData.Unit)));
                    }
                }

                return output;
            }

            return response.GetDataSet(googleFitDataType)?.DataPoints?
                .Select(result => CreateHealthData(result, googleFitData.Unit)).ToList();
        }

        HealthData CreateHealthData(DataPoint dataPoint, Field unit)
        {
            return new HealthData
            {
                StartDate   = dataPoint.GetStartTime(TimeUnit.Milliseconds).ToDateTime(),
                EndDate     = dataPoint.GetEndTime(TimeUnit.Milliseconds).ToDateTime(),
                Value       = ReadValue(dataPoint, unit),
                UserEntered = dataPoint.OriginalDataSource?.StreamName == "user_input"
            };
        }

        double ReadValue(DataPoint dataPoint, Field unit)
        {
            try
            {
                //need to pass type of field
                //return dataPoint.GetValue(Field.FieldAverage).AsFloat();
                return dataPoint.GetValue(unit).AsFloat();
            }
            catch
            {
                try
                {
                    return dataPoint.GetValue(unit).AsInt();
                }
                catch
                {
                    try
                    {
                        return dataPoint.GetValue(unit).AsString().ToDbl();
                    }
                    catch (Exception e3)
                    {
                        Debug.WriteLine($"GoogleFit - Unable to convert datatype format", e3);
                        return 0;
                    }
                }
            }
        }

        public static void Init(Application application)
        {
            CrossCurrentActivity.Current.Init(application);
        }

        public static void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _tcsAuth.TrySetResult(requestCode == REQUEST_CODE && resultCode == Result.Ok);
        }
    }
}
