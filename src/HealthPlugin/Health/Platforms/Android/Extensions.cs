using System;
using Android.Gms.Fitness.Data;

namespace Plugin.Health
{
    internal static class Extensions
    {
        internal static GoogleFitData ToGoogleFit(this HealthData.DataType dataType)
        {
            switch (dataType)
            {
                case HealthData.DataType.StepCount:
                    return new GoogleFitData
                    {
                        TypeIdentifier          = DataType.TypeStepCountDelta,
                        AggregateTypeIdentifier = DataType.AggregateStepCountDelta,
                        Unit                    = Field.FieldSteps,
                    };

                case HealthData.DataType.Distance:
                    return new GoogleFitData
                    {
                        TypeIdentifier          = DataType.TypeDistanceDelta,
                        AggregateTypeIdentifier = DataType.AggregateDistanceDelta,
                        Unit                    = Field.FieldDistance,
                    };

                case HealthData.DataType.HeartRate:
                    return new GoogleFitData
                    {
                        TypeIdentifier          = DataType.TypeHeartRateBpm,
                        AggregateTypeIdentifier = null,
                        Unit                    = Field.FieldBpm,
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static long ToJavaTimeStamp(this DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime ()
                                 .Subtract (new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                                 .TotalMilliseconds;
        }

        internal static DateTime ToDateTime(this long javaTimeStamp )
        {
            // Java timestamp is milliseconds past epoch
            var dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds( javaTimeStamp ).ToLocalTime();
            return dtDateTime;
        }
    }
}
