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

                    return new HealthKitData(HKQuantityTypeIdentifier.StepCount)
                    {
                        Unit           = HKUnit.Count,
                        Cumulative     = true
                    };

                case HealthDataType.Distance:
                    return new HealthKitData(HKQuantityTypeIdentifier.DistanceWalkingRunning)
                    {
                        Unit           = HKUnit.Meter,
                        Cumulative     = true
                    };

                case HealthDataType.HeartRate:
                    return new HealthKitData(HKQuantityTypeIdentifier.HeartRate)
                    {
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute)
                    };

                case HealthDataType.Height:
                    return new HealthKitData(HKQuantityTypeIdentifier.Height)
                    {
                        Unit           = HKUnit.Meter
                    };

                case HealthDataType.Weight:
                    return new HealthKitData(HKQuantityTypeIdentifier.BodyMass)
                    {
                        Unit           = HKUnit.Gram
                    };

                case HealthDataType.Calories:
                    return new HealthKitData(HKQuantityTypeIdentifier.ActiveEnergyBurned)
                    {
                        Unit           = HKUnit.Kilocalorie,
                        Cumulative     = true
                    };

                case HealthDataType.CaloriesActive:
                    return new HealthKitData(HKQuantityTypeIdentifier.ActiveEnergyBurned)
                    {
                        Unit           = HKUnit.Kilocalorie,
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
                    return new HealthKitData(HKQuantityTypeIdentifier.DietaryWater)
                    {
                        Unit           = HKUnit.Liter,
                        Cumulative     = true
                    };

                case HealthDataType.BodyFat:
                    return new HealthKitData(HKQuantityTypeIdentifier.BodyFatPercentage)
                    {
                        Unit           = HKUnit.Percent
                    };

                case HealthDataType.BodyTemperature:
                    return new HealthKitData(HKQuantityTypeIdentifier.BodyTemperature)
                    {
                        Unit           = HKUnit.DegreeCelsius
                    };

                case HealthDataType.BloodPressureSystolic:
                    return new HealthKitData(HKQuantityTypeIdentifier.BloodPressureSystolic)
                    {
                        Unit           = HKUnit.MillimeterOfMercury
                    };

                case HealthDataType.BloodPressureDiastolic:
                    return new HealthKitData(HKQuantityTypeIdentifier.BloodPressureDiastolic)
                    {
                        Unit           = HKUnit.MillimeterOfMercury
                    };

                case HealthDataType.BloodOxygen:
                    return new HealthKitData(HKQuantityTypeIdentifier.OxygenSaturation)
                    {
                        Unit           = HKUnit.Percent
                    };

                case HealthDataType.BloodGlucose:
                    return new HealthKitData(HKQuantityTypeIdentifier.BloodGlucose)
                    {
                        Unit           = HKUnit.FromString("mg/dl")
                    };

                // IOS SPECIFIC

                case HealthDataType.iOS_BodyMassIndex:
                    return new HealthKitData(HKQuantityTypeIdentifier.BodyMassIndex)
                    {
                        Unit           = HKUnit.FromString("")
                    };

                case HealthDataType.iOS_WalkingHeartRate:
                    return new HealthKitData(HKQuantityTypeIdentifier.WalkingHeartRateAverage)
                    {
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute)
                    };

                case HealthDataType.iOS_RestingHeartRate:
                    return new HealthKitData(HKQuantityTypeIdentifier.RestingHeartRate)
                    {
                        Unit           = HKUnit.Count.UnitDividedBy(HKUnit.Minute)
                    };

                case HealthDataType.iOS_BasalEnergyBurned:
                    return new HealthKitData(HKQuantityTypeIdentifier.BasalEnergyBurned)
                    {
                        Unit           = HKUnit.Kilocalorie,
                        Cumulative     = true
                    };

                case HealthDataType.iOS_WaistCircumference:
                    return new HealthKitData(HKQuantityTypeIdentifier.WaistCircumference)
                    {
                        Unit           = HKUnit.Meter
                    };

                case HealthDataType.iOS_StandTime:
                    ThrowIfUnsupported(13, 0);
                    return new HealthKitData(HKQuantityTypeIdentifier.AppleStandTime)
                    {
                        Unit           = HKUnit.Minute,
                        Cumulative     = true
                    };

                case HealthDataType.iOS_ExerciseTime:
                    return new HealthKitData(HKQuantityTypeIdentifier.AppleExerciseTime)
                    {
                        Unit           = HKUnit.Minute,
                        Cumulative     = true,
                    };

                case HealthDataType.MindfulSession:
                    return new HealthKitData(HKCategoryTypeIdentifier.MindfulSession)
                    {
                    };

                case HealthDataType.RespiratoryRate:
                    return new HealthKitData(HKQuantityTypeIdentifier.RespiratoryRate)
                    {
                        Unit = HKUnit.Count.UnitDividedBy(HKUnit.Minute)
                    };

                case HealthDataType.BiologicalSex:
                    return new HealthKitData(HKCharacteristicTypeIdentifier.BiologicalSex)
                    {
                    };

                case HealthDataType.DateOfBirth:
                    return new HealthKitData(HKCharacteristicTypeIdentifier.DateOfBirth)
                    {
                    };

                case HealthDataType.Workouts:
                    
                    return new HealthKitData(HKObjectType.GetWorkoutType())
                    {
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
