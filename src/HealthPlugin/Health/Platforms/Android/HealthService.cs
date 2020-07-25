﻿using System;
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
using Debug = System.Diagnostics.Debug;

namespace Plugin.Health
{
    public partial class HealthService : IHealthService
    {
        //Info su oauth e googlefit
        //https://developers.google.com/fit/android/get-started
        //https://developers.google.com/fit/android/get-api-key
        //https://developers.google.com/fit/android/authorization#android-10

        //Esempi di utilizzo GoogleFit
        //https://github.com/googlearchive/android-fit/blob/master/BasicHistoryApi/app/src/main/java/com/google/android/gms/fit/samples/basichistoryapi/MainActivity.java
        //https://github.com/xamarin/monodroid-samples/blob/master/google-services/Fitness/BasicHistoryApi/BasicHistoryApi/MainActivity.cs
        //https://github.com/krokyze/FitKit/blob/master/android/src/main/kotlin/com/example/fit_kit/FitKitPlugin.kt
        //https://github.com/cph-cachet/flutter-plugins/blob/master/packages/health/android/src/main/kotlin/cachet/plugins/health/HealthPlugin.kt

        static Activity _activity;
        Context _context;
        const int RequestCode = 1;
        static TaskCompletionSource<bool> _tcsAuth;

        public HealthService()
        {
            if (_activity == null)
            {
                throw new Exception("Please call HealthService.Init() method in the platform specific project to use Health Plugin");
            }

            _context = Application.Context;
        }

        public bool IsDataTypeAvailable(HealthData.DataType dataType)
        {
            try
            {
                return dataType.ToGoogleFit() != null;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.WriteLine($"GoogleFit - Datatype {dataType} is not supported in this device", e);
                return false;
            }
        }

        public Task<bool> RequestPermissionAsync(params HealthData.DataType[] dataTypes)
        {
            var fitnessOptions = GetFitnessOptions(dataTypes);

            _tcsAuth = new TaskCompletionSource<bool>();

            if (HasOAuthPermission(fitnessOptions))
            {
                _tcsAuth.SetResult(true);
            }
            else
            {
                GoogleSignIn.RequestPermissions(_activity, RequestCode,
                    GoogleSignIn.GetLastSignedInAccount(_context), fitnessOptions);
            }

            return _tcsAuth.Task;
        }

        bool HasOAuthPermission(FitnessOptions fitnessOptions)
        {
            return GoogleSignIn.HasPermissions(GoogleSignIn.GetLastSignedInAccount(_context), fitnessOptions);
        }

        FitnessOptions GetFitnessOptions(params HealthData.DataType[] dataTypes)
        {
            var fitnessBuilder = FitnessOptions.InvokeBuilder();
            foreach (var dataType in dataTypes)
            {
                fitnessBuilder.AddDataType(dataType.ToGoogleFit().TypeIdentifier, FitnessOptions.AccessRead);
            }
            return fitnessBuilder.Build();
        }

        async Task<List<HealthData>> FetchDataAsync(HealthData.DataType dataType, DateTime startDate, DateTime endDate, bool requestPermissionIfNeeded)
        {
            var authorized = HasOAuthPermission(GetFitnessOptions(dataType));

            if (!authorized && !requestPermissionIfNeeded)
                throw new UnauthorizedAccessException($"Not enough permissions to request {dataType}");

            if (!authorized)
            {
                var permission = await RequestPermissionAsync(dataType);
                if (!permission)
                    throw new UnauthorizedAccessException($"Not enough permissions to request {dataType}");
            }

            return await QueryAsync(dataType, startDate, endDate);
        }

        async Task<List<HealthData>> QueryAsync(HealthData.DataType dataType, DateTime startDate, DateTime endDate)
        {
            var googleFitData     = dataType.ToGoogleFit();
            var googleFitDataType = googleFitData.TypeIdentifier;
            var startTime         = startDate.ToJavaTimeStamp();
            var endTime           = endDate.ToJavaTimeStamp();

            var readBuilder = new DataReadRequest.Builder()
                              .SetTimeRange(startTime, endTime, TimeUnit.Milliseconds)
                              .EnableServerQueries();

            if (_aggregateType != HealthData.AggregateType.None && googleFitData.AggregateTypeIdentifier != null)
            {
                readBuilder.Aggregate(googleFitDataType, googleFitData.AggregateTypeIdentifier);

                switch (_aggregateType)
                {
                    case HealthData.AggregateType.Year:
                        readBuilder.BucketByTime(365, TimeUnit.Days);
                        break;

                    case HealthData.AggregateType.Month:
                        readBuilder.BucketByTime(31, TimeUnit.Days);
                        break;

                    case HealthData.AggregateType.Week:
                        readBuilder.BucketByTime(7, TimeUnit.Days);
                        break;

                    case HealthData.AggregateType.Hour:
                        readBuilder.BucketByTime(1, TimeUnit.Hours);
                        break;

                    case HealthData.AggregateType.Minute:
                        readBuilder.BucketByTime(1, TimeUnit.Minutes);
                        break;

                    case HealthData.AggregateType.Second:
                        readBuilder.BucketByTime(1, TimeUnit.Seconds);
                        break;

                    default:
                        readBuilder.BucketByTime(1, TimeUnit.Days);
                        break;
                }
            }
            else
            {
                readBuilder.Read(googleFitDataType);
            }

            var readRequest = readBuilder.Build();

            var response = await FitnessClass.GetHistoryClient(_activity, GoogleSignIn.GetLastSignedInAccount(_activity))
                                             .ReadDataAsync(readRequest);

            if (response == null)
                return new List<HealthData>();

            if (response.Buckets.Any())
            {
                var output = new List<HealthData>();
                foreach (var bucket in response.Buckets)
                {
                    foreach (var dataSet in bucket.DataSets)
                    {
                        output.AddRange(dataSet.DataPoints.Select(result=>CreateHealthData(result, googleFitData.Unit)));
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

        public static void Init(Activity activity)
        {
            _activity = activity;
        }

        public static void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _tcsAuth.TrySetResult(requestCode == RequestCode && resultCode == Result.Ok);
        }
    }
}