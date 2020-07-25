using HealthKit;

namespace Plugin.Health
{
    internal class HealthKitData
    {
        public HKQuantityTypeIdentifier TypeIdentifier  { get; set; }
        public HKStatisticsOptions      StatisticOption { get; set; } = HKStatisticsOptions.DiscreteAverage;
        public HKUnit Unit { get; set; }

    }
}
