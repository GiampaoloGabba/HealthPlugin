using System;

namespace Plugin.Health
{
    public class HealthData
    {
        public DateTime   StartDate   { get; set; }
        public DateTime   EndDate     { get; set; }
        public double     Value       { get; set; }
        public bool       UserEntered { get; set; }
    }


    public enum AggregateTime
    {
        None,
        Year,
        Month,
        Week,
        Day,
        Hour,
        Minute
    }

    public enum AggregateType
    {
        None,
        CumulativeSum,
        Average,
        Min,
        Max
    }

    public enum HealthDataType
    {
        StepCount,
        Distance,
        HeartRate,
        Height,
        Weight,
        Energy,
        Water,
        BodyFat,
        BodyMassIndex,
        ActiveEnergyBurned,
        BodyTemperature,
        BloodPressureSystolic,
        BloodPressureDiastolic,
        BloodOxygen,
        BloodGlucose,

        iOS_WalkingHeartRate,
        iOS_RestingHeartRate,
        iOS_BasalEnergyBurned,
        iOS_WaistCircumference,
        iOS_StandTime,
        iOS_ExerciseTime
    }
}
