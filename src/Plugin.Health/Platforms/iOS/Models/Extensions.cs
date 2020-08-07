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

        //TODO: come gestire active+basal per calorie?
        //TODO: HKQuantityTypeIdentifierActiveEnergyBurned + HKQuantityTypeIdentifierBasalEnergyBurned

        internal static HealthKitData ToHealthKit(this HealthDataType healthDataType)
        {
            switch (healthDataType)
            {
                case HealthDataType.StepCount:

                    return new HealthKitData
                    {
                        Unit           = HKUnit.Count,
                        TypeIdentifier = HKQuantityTypeIdentifier.StepCount,
                        Cumulative     = true

                    };

                case HealthDataType.Distance:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Meter,
                        TypeIdentifier = HKQuantityTypeIdentifier.DistanceWalkingRunning,
                        Cumulative     = true
                    };

                case HealthDataType.HeartRate:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute),
                        TypeIdentifier = HKQuantityTypeIdentifier.HeartRate
                    };

                case HealthDataType.Height:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Meter,
                        TypeIdentifier = HKQuantityTypeIdentifier.Height
                    };

                case HealthDataType.Weight:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Gram,
                        TypeIdentifier = HKQuantityTypeIdentifier.BodyMass
                    };

                case HealthDataType.Calories:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Kilocalorie,
                        TypeIdentifier = HKQuantityTypeIdentifier.ActiveEnergyBurned,
                        Cumulative     = true
                    };

                case HealthDataType.CaloriesActive:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Kilocalorie,
                        TypeIdentifier = HKQuantityTypeIdentifier.ActiveEnergyBurned,
                        Cumulative     = true
                    };

                /*case HealthDataType.CaloriesBasal:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Kilocalorie,
                        TypeIdentifier = HKQuantityTypeIdentifier.BasalEnergyBurned,
                        Cumulative     = true
                    };*/

                case HealthDataType.Water:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Liter,
                        TypeIdentifier = HKQuantityTypeIdentifier.DietaryWater,
                        Cumulative     = true
                    };

                case HealthDataType.BodyFat:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Percent,
                        TypeIdentifier = HKQuantityTypeIdentifier.BodyFatPercentage
                    };

                case HealthDataType.BodyTemperature:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.DegreeCelsius,
                        TypeIdentifier = HKQuantityTypeIdentifier.BodyTemperature
                    };

                case HealthDataType.BloodPressureSystolic:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.MillimeterOfMercury,
                        TypeIdentifier = HKQuantityTypeIdentifier.BloodPressureSystolic
                    };

                case HealthDataType.BloodPressureDiastolic:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.MillimeterOfMercury,
                        TypeIdentifier = HKQuantityTypeIdentifier.BloodPressureDiastolic
                    };

                case HealthDataType.BloodOxygen:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Percent,
                        TypeIdentifier = HKQuantityTypeIdentifier.OxygenSaturation
                    };

                case HealthDataType.BloodGlucose:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.FromString("mg/dl"),
                        TypeIdentifier = HKQuantityTypeIdentifier.BloodGlucose
                    };

                
                // IOS SPECIFIC

                case HealthDataType.iOS_BodyMassIndex:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.FromString(""),
                        TypeIdentifier = HKQuantityTypeIdentifier.BodyMassIndex
                    };

                case HealthDataType.iOS_WalkingHeartRate:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute),
                        TypeIdentifier = HKQuantityTypeIdentifier.WalkingHeartRateAverage
                    };

                case HealthDataType.iOS_RestingHeartRate:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute),
                        TypeIdentifier = HKQuantityTypeIdentifier.RestingHeartRate
                    };

                case HealthDataType.iOS_BasalEnergyBurned:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Kilocalorie,
                        TypeIdentifier = HKQuantityTypeIdentifier.BasalEnergyBurned,
                        Cumulative     = true
                    };

                case HealthDataType.iOS_WaistCircumference:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Meter,
                        TypeIdentifier = HKQuantityTypeIdentifier.WaistCircumference
                    };

                case HealthDataType.iOS_StandTime:
                    ThrowIfUnsupported(13, 0);
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Minute,
                        TypeIdentifier = HKQuantityTypeIdentifier.AppleStandTime,
                        Cumulative     = true
                    };

                case HealthDataType.iOS_ExerciseTime:
                    return new HealthKitData
                    {
                        Unit           = HKUnit.Minute,
                        TypeIdentifier = HKQuantityTypeIdentifier.AppleExerciseTime,
                        Cumulative     = true
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
