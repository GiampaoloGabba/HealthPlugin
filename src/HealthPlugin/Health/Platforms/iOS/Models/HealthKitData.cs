using HealthKit;

namespace Plugin.Health
{
    internal class HealthKitData
    {
        public HKQuantityTypeIdentifier TypeIdentifier  { get; set; }
        public HKStatisticsOptions DefaultStatisticOption { get; set; } = HKStatisticsOptions.DiscreteAverage;
        public bool NegateCumulativeSum { get; set; } = true;
        public HKUnit Unit { get; set; }

    }
}
