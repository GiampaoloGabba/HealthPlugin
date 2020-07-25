using System;
using HealthKit;
using UIKit;

namespace Plugin.Health
{
    internal static class Extensions
    {
        //More info for init HKUnit:
        //https://developer.apple.com/documentation/healthkit/hkunit/1615733-init

        //More info on datatype:
        //https://github.com/dariosalvi78/cordova-plugin-health

        internal static HealthKitData ToHealthKit(this HealthDataType healthDataType)
        {
            switch (healthDataType)
            {
                case HealthDataType.StepCount:

                    return new HealthKitData
                    {
                        TypeIdentifier  = HKQuantityTypeIdentifier.StepCount,
                        Unit            = HKUnit.Count,
                        StatisticOption = HKStatisticsOptions.CumulativeSum
                    };

                case HealthDataType.Distance:
                    return new HealthKitData
                    {
                        TypeIdentifier  = HKQuantityTypeIdentifier.DistanceWalkingRunning,
                        Unit            = HKUnit.Meter,
                        StatisticOption = HKStatisticsOptions.CumulativeSum
                    };

                case HealthDataType.HeartRate:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.HeartRate,
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute),
                    };

                case HealthDataType.Height:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.Height,
                        Unit           = HKUnit.Meter
                    };

                case HealthDataType.Weight:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.BodyMass,
                        Unit           = HKUnit.Gram
                    };

                case HealthDataType.Energy:
                    return new HealthKitData
                    {
                        TypeIdentifier  = HKQuantityTypeIdentifier.ActiveEnergyBurned,
                        Unit            = HKUnit.Kilocalorie,
                        StatisticOption = HKStatisticsOptions.CumulativeSum
                    };

                case HealthDataType.Water:
                    return new HealthKitData
                    {
                        TypeIdentifier  = HKQuantityTypeIdentifier.DietaryWater,
                        Unit            = HKUnit.Liter,
                        StatisticOption = HKStatisticsOptions.CumulativeSum
                    };

                case HealthDataType.BodyFat:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.BodyFatPercentage,
                        Unit           = HKUnit.Percent
                    };

                case HealthDataType.BodyMassIndex:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.BodyMassIndex,
                        Unit           = HKUnit.FromString("")
                    };

                case HealthDataType.ActiveEnergyBurned:
                    return new HealthKitData
                    {
                        TypeIdentifier  = HKQuantityTypeIdentifier.ActiveEnergyBurned,
                        Unit            = HKUnit.Kilocalorie,
                        StatisticOption = HKStatisticsOptions.CumulativeSum
                    };

                case HealthDataType.BodyTemperature:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.BodyTemperature,
                        Unit           = HKUnit.DegreeCelsius
                    };

                case HealthDataType.BloodPressureSystolic:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.BloodPressureSystolic,
                        Unit           = HKUnit.MillimeterOfMercury
                    };

                case HealthDataType.BloodPressureDiastolic:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.BloodPressureDiastolic,
                        Unit           = HKUnit.MillimeterOfMercury
                    };

                case HealthDataType.BloodOxygen:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.OxygenSaturation,
                        Unit           = HKUnit.Percent
                    };

                case HealthDataType.BloodGlucose:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.BloodGlucose,
                        Unit           = HKUnit.FromString("mg/dl")
                    };

                // IOS SPECIFIC

                case HealthDataType.iOS_WalkingHeartRate:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.WalkingHeartRateAverage,
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute)
                    };

                case HealthDataType.iOS_RestingHeartRate:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.RestingHeartRate,
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute)
                    };

                case HealthDataType.iOS_BasalEnergyBurned:
                    return new HealthKitData
                    {
                        TypeIdentifier  = HKQuantityTypeIdentifier.BasalEnergyBurned,
                        Unit            = HKUnit.Kilocalorie,
                        StatisticOption = HKStatisticsOptions.CumulativeSum
                    };

                case HealthDataType.iOS_WaistCircumference:
                    return new HealthKitData
                    {
                        TypeIdentifier = HKQuantityTypeIdentifier.WaistCircumference,
                        Unit           = HKUnit.Meter
                    };

                case HealthDataType.iOS_StandTime:
                    ThrowIfUnsupported(13, 0);
                    return new HealthKitData
                    {
                        TypeIdentifier  = HKQuantityTypeIdentifier.AppleStandTime,
                        Unit            = HKUnit.Minute,
                        StatisticOption = HKStatisticsOptions.CumulativeSum
                    };

                case HealthDataType.iOS_ExerciseTime:
                    return new HealthKitData
                    {
                        TypeIdentifier  = HKQuantityTypeIdentifier.AppleExerciseTime,
                        Unit            = HKUnit.Minute,
                        StatisticOption = HKStatisticsOptions.CumulativeSum
                    };

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ThrowIfUnsupported(int major, int minor)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(major, minor))
                throw new NotSupportedException();
        }
    }
}
