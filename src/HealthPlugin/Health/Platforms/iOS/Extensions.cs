using System;
using HealthKit;

namespace Plugin.Health
{
    internal static class Extensions
    {
        internal static HealthKitData ToHealthKit(this HealthDataType healthDataType)
        {
            switch (healthDataType)
            {
                case HealthDataType.StepCount:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.StepCount,
                        Unit           = HKUnit.Count,
                    };

                case HealthDataType.Distance:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.DistanceWalkingRunning,
                        Unit           = HKUnit.Meter,
                    };

                case HealthDataType.HeartRate:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.HeartRate,
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute),
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
