namespace Plugin.Health
{
    public enum Permissions
    {
        Read,
        Write,
        Both
    }

    public enum AggregateTime
    {
        None,
        Year,
        Month,
        Week,
        Day,
        Hour
    }

    [Preserve(AllMembers = true)]
    public enum HealthDataType
    {
        StepCount,
        Distance,
        HeartRate,
        Height,
        Weight,
        Calories,
        CaloriesActive,
        Water,
        BodyFat,
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
        iOS_ExerciseTime,

        Droid_BasalMetabolicRate,
        MindfulSession,
        RespiratoryRate,
        Workouts,
        DateOfBirth,
        BiologicalSex,
        SleepAnalysis
    }
}
