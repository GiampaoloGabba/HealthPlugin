namespace Plugin.Health
{
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

    [Preserve(AllMembers = true)]
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
        ActiveEnergyBurned,
        BodyTemperature,
        BloodPressureSystolic,
        BloodPressureDiastolic,
        BloodOxygen,
        BloodGlucose,

        iOS_BodyMassIndex,
        iOS_WalkingHeartRate,
        iOS_RestingHeartRate,
        iOS_BasalEnergyBurned,
        iOS_WaistCircumference,
        iOS_StandTime,
        iOS_ExerciseTime
    }
}
