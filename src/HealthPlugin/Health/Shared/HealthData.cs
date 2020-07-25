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
        Minute,
        Second
    }

    public enum HealthDataType
    {
        StepCount,
        Distance,
        HeartRate,
    }
}
