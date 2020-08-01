using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Fitness;
using Plugin.CurrentActivity;
using Debug = System.Diagnostics.Debug;

namespace Plugin.Health
{
    [Preserve(AllMembers = true)]
    public class HealthService : HealthServiceBase
    {
        //Info su oauth e googlefit
        //https://developers.google.com/fit/android/get-started
        //https://developers.google.com/fit/android/get-api-key
        //https://developers.google.com/fit/android/authorization#android-10

        //Esempi di utilizzo GoogleFit
        //aggregation: https://github.com/dariosalvi78/cordova-plugin-health/blob/master/src/android/HealthPlugin.java
        //datatypes: https://developers.google.com/android/reference/com/google/android/gms/fitness/data/DataType#TYPE_STEP_COUNT_DELTA

        internal Activity CurrentActivity => CrossCurrentActivity.Current.Activity ??
                              throw new NullReferenceException("Please call HealthService.Init() method in the platform specific project to use Health Plugin");
        Context _currentContext => CrossCurrentActivity.Current.AppContext;
        const int REQUEST_CODE = 1;
        static TaskCompletionSource<bool> _tcsAuth;

        // detects if a) Google APIs are available, b) Google Fit is actually installed
        public override bool IsAvailable() {
            // first check that the Google APIs are available
            var gapi      = GoogleApiAvailability.Instance;
            int apiresult = gapi.IsGooglePlayServicesAvailable(_currentContext);
            return apiresult == ConnectionResult.Success && IsGooglefitInstalled();
        }

        public override void PromptInstallGoogleFit()
        {
            try {
                CurrentActivity.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.google.android.apps.fitness")));
            } catch (ActivityNotFoundException e) {
                CurrentActivity.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.google.android.apps.fitness")));
            }
        }

        private bool IsGooglefitInstalled()
        {
            // then check that Google Fit is actually installed
            var pm = _currentContext.PackageManager;
            try {
                pm.GetPackageInfo("com.google.android.apps.fitness", PackageInfoFlags.Activities);
                return true;
            } catch (PackageManager.NameNotFoundException e) {
                Debug.WriteLine("Google Fit not installed");
            }

            return false;
        }

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
                GoogleSignIn.RequestPermissions(CurrentActivity, REQUEST_CODE,
                    GoogleSignIn.GetLastSignedInAccount(_currentContext), fitnessOptions);
            }

            return _tcsAuth.Task;
        }

        public override IHealthDataReader DataReader()
        {
            return new HealthDataReader(this);
        }

        internal bool HasOAuthPermission(FitnessOptions fitnessOptions)
        {
            return GoogleSignIn.HasPermissions(GoogleSignIn.GetLastSignedInAccount(_currentContext), fitnessOptions);
        }

        internal FitnessOptions GetFitnessOptions(params HealthDataType[] dataTypes)
        {
            var fitnessBuilder = FitnessOptions.InvokeBuilder();
            foreach (var dataType in dataTypes)
            {
                fitnessBuilder.AddDataType(dataType.ToGoogleFit().TypeIdentifier, FitnessOptions.AccessRead);
            }
            return fitnessBuilder.Build();
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
