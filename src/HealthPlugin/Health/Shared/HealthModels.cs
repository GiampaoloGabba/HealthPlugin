using System;

namespace Plugin.Health
{
    public class HealthData
    {
        public enum DataType
        {
            StepCount,
            Distance,
            HeartRate,
        }

        public enum HealthUnit
        {
            Count,
            CountMinute,
            Meter,
            Kilo,
            KiloCalorie,
            Liter,
            Minute,
            Percent,
            Celsius
        }

        public enum AggregateType
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

        public DateTime   StartDate   { get; set; }
        public DateTime   EndDate     { get; set; }
        public double     Value       { get; set; }
        public HealthUnit Unit        { get; set; }
        public bool       UserEntered { get; set; }
    }
}
