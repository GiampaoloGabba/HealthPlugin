using System;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Result;

namespace Plugin.Health
{
    internal static class Extensions
    {
        //TODO: come gestire active+basal per calorie?
        //TODO: TYPE_CALORIES_EXPENDED = (TYPE_CALORIES_EXPENDED - (TYPE_BASAL_METABOLIC_RATE * time window))+ (TYPE_BASAL_METABOLIC_RATE * time window)

        internal static GoogleFitData ToGoogleFit(this HealthDataType healthDataType)
        {
            switch (healthDataType)
            {
                case HealthDataType.StepCount:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeStepCountDelta,
                        AggregateType  = DataType.AggregateStepCountDelta,
                        Unit           = Field.FieldSteps,
                        Cumulative     = true
                    };

                case HealthDataType.Distance:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeDistanceDelta,
                        AggregateType  = DataType.AggregateDistanceDelta,
                        Unit           = Field.FieldDistance,
                        Cumulative     = true
                    };

                case HealthDataType.HeartRate:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeHeartRateBpm,
                        AggregateType  = DataType.AggregateHeartRateSummary,
                        Unit           = Field.FieldBpm
                    };

                case HealthDataType.Height:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeHeight,
                        AggregateType  = DataType.AggregateHeightSummary,
                        Unit           = Field.FieldHeight
                    };

                case HealthDataType.Weight:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeWeight,
                        AggregateType  = DataType.AggregateWeightSummary,
                        Unit           = Field.FieldWeight
                    };

                case HealthDataType.Energy:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeCaloriesExpended,
                        AggregateType  = DataType.AggregateCaloriesExpended,
                        Unit           = Field.FieldCalories,
                        Cumulative     = true
                    };

                case HealthDataType.Water:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeHydration,
                        AggregateType  = DataType.AggregateHydration,
                        Unit           = Field.FieldVolume,
                        Cumulative     = true
                    };

                case HealthDataType.BodyFat:
                    return new GoogleFitData
                    {
                        TypeIdentifier = DataType.TypeBodyFatPercentage,
                        AggregateType  = DataType.TypeBodyFatPercentage,
                        Unit           = Field.FieldPercentage
                    };

                case HealthDataType.ActiveEnergyBurned:
                    return new GoogleFitData
                    {
                        //TypeIdentifier = DataType, //TODO: quale datatype
                        //AggregateType  = DataType, //TODO: quale datatype
                        // Unit           = Field //TODO: quale unit?
                    };

                case HealthDataType.BodyTemperature:
                    return new GoogleFitData
                    {
                        TypeIdentifier = HealthDataTypes.TypeBodyTemperature,
                        AggregateType  = HealthDataTypes.AggregateBodyTemperatureSummary,
                        Unit           = HealthFields.FieldBodyTemperature,
                        Cumulative     = true
                    };

                case HealthDataType.BloodPressureSystolic:
                    return new GoogleFitData
                    {
                        TypeIdentifier  = HealthDataTypes.TypeBloodPressure,
                        AggregateType   = HealthDataTypes.AggregateBloodPressureSummary,
                        Unit            = HealthFields.FieldBloodPressureSystolic,
                        MinOverride     = HealthFields.FieldBloodPressureSystolicMin,
                        MaxOverride     = HealthFields.FieldBloodPressureSystolicMax,
                        AverageOverride = HealthFields.FieldBloodPressureSystolicAverage,
                    };

                case HealthDataType.BloodPressureDiastolic:
                    return new GoogleFitData
                    {
                        TypeIdentifier  = HealthDataTypes.TypeBloodPressure,
                        AggregateType   = HealthDataTypes.AggregateBloodPressureSummary,
                        Unit            = HealthFields.FieldBloodPressureDiastolic,
                        MinOverride     = HealthFields.FieldBloodPressureDiastolicMin,
                        MaxOverride     = HealthFields.FieldBloodPressureDiastolicMax,
                        AverageOverride = HealthFields.FieldBloodPressureDiastolicAverage,
                    };

                case HealthDataType.BloodOxygen:
                    return new GoogleFitData
                    {
                        TypeIdentifier  = HealthDataTypes.TypeOxygenSaturation,
                        AggregateType   = HealthDataTypes.AggregateOxygenSaturationSummary,
                        Unit            = HealthFields.FieldOxygenSaturation,
                        MinOverride     = HealthFields.FieldOxygenSaturationMin,
                        MaxOverride     = HealthFields.FieldOxygenSaturationMax,
                        AverageOverride = HealthFields.FieldOxygenSaturationAverage,
                    };

                case HealthDataType.BloodGlucose:
                    return new GoogleFitData
                    {
                        TypeIdentifier = HealthDataTypes.TypeBloodGlucose,
                        AggregateType  = HealthDataTypes.AggregateBloodGlucoseSummary,
                        Unit           = HealthFields.FieldBloodGlucoseLevel,
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
