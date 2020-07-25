using HealthKit;

namespace Plugin.Health
{
    internal class HealthKitData
    {
        public HKQuantityTypeIdentifier TypeIdentifier { get; set; }
        public HKUnit                   Unit           { get; set; }

    }
}
