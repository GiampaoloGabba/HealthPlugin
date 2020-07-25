using System;
using HealthKit;

namespace Plugin.Health
{
    internal static class Extensions
    {
        internal static HealthKitData ToHealthKit(this HealthData.DataType dataType)
        {
            switch (dataType)
            {
                case HealthData.DataType.StepCount:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.StepCount,
                        Unit           = HKUnit.Count,
                    };

                case HealthData.DataType.Distance:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.DistanceWalkingRunning,
                        Unit           = HKUnit.Meter,
                    };

                case HealthData.DataType.HeartRate:
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
