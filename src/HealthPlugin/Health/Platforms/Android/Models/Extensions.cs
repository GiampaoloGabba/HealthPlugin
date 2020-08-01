using System;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Result;

namespace Plugin.Health
{
    internal static class Extensions
    {
        internal static GoogleFitData ToGoogleFit(this HealthDataType healthDataType)
        {
            switch (healthDataType)
            {
                case HealthDataType.StepCount:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeStepCountDelta,
                        AggregateType  = DataType.AggregateStepCountDelta,
                        Cumulative     = true,
                        Unit           = Field.FieldSteps,
                    };

                case HealthDataType.Distance:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeDistanceDelta,
                        AggregateType  = DataType.AggregateDistanceDelta,
                        Cumulative     = true,
                        Unit           = Field.FieldDistance,
                    };

                case HealthDataType.HeartRate:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeHeartRateBpm,
                        AggregateType  = DataType.AggregateHeartRateSummary,
                        Unit           = Field.FieldBpm,
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
