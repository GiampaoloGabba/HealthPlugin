using System;
using System.Threading.Tasks;
using Foundation;
using HealthKit;
using static Plugin.Health.HealthKitData;

namespace Plugin.Health
{
    public class HealthDataWriter : HealthDataWriterBase
    {
        readonly HealthService _healthService;
        readonly HKHealthStore _healthStore;

        public HealthDataWriter(HealthService healthService)
        {
            _healthService = healthService;
            _healthStore = healthService.HealthStore;
        }

        public override async Task<bool> WriteAsync(HealthDataType healthDataType, double value, DateTime start, DateTime? end = null)
        {
            if (end == null)
            {
                end = start;
            }

            var healthKit = healthDataType.ToHealthKit();

            if (healthKit.HKType == HKTypes.Category)
            {
                var type = HKCategoryType.Create(healthDataType.ToHealthKit().CategoryTypeIdentifier);
                var sample = HKCategorySample.FromType(type, (nint)value, (NSDate)start, (NSDate)end);

                var (success, error) = await _healthStore.SaveObjectAsync(sample).ConfigureAwait(false);

                return success;

            }
            else if (healthKit.HKType == HKTypes.Quantity)
            {

                var type = HKQuantityType.Create(healthDataType.ToHealthKit().QuantityTypeIdentifier);
                var quantity = HKQuantity.FromQuantity(healthDataType.ToHealthKit().Unit, value);
                var sample = HKQuantitySample.FromType(type, quantity, (NSDate)start, (NSDate)end);

                var (success, error) = await _healthStore.SaveObjectAsync(sample).ConfigureAwait(false);

                return success;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /*
        public async Task<bool> SaveBmi(double value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.iOS_BodyMassIndex, value, start, end);
        }

        public async Task<bool> SaveMindfulSession(int value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.MindfulSession, value, start, end);
        }

        public async Task<bool> SaveHeight(double value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.Height, value, start, end);
        }

        public async Task<bool> SaveWeight(double value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.Weight, value, start, end);
        }

        public async Task<bool> SaveStep(int value, DateTime start, DateTime? end)
        {
            return await WriteAsync(HealthDataType.StepCount, value, start, end);
        }

        public async Task<bool> SaveSleepAnalysis(HKCategoryValueSleepAnalysis value, DateTime start, DateTime end)
        {
            return await WriteAsync(HealthDataType.SleepAnalysis, (int)value, start, end);
        }
        */

        public override async Task<bool> WriteAsync(WorkoutDataType workoutDataType, double calories, DateTime start, DateTime? end = null, string name = null)
        {
            var totalEnergyBurned = HKQuantity.FromQuantity(HKUnit.CreateJouleUnit(HKMetricPrefix.Kilo), 20);

            var metadata = new HKMetadata();

            if (name != null)
            {
                metadata.WorkoutBrandName = name;
            }

            var workout = HKWorkout.Create(Convert(workoutDataType), (NSDate)start, (NSDate)end, new HKWorkoutEvent[] { }, totalEnergyBurned, null, metadata);


            var (success, error) = await _healthStore.SaveObjectAsync(workout);

            return success;
        }

        private HKWorkoutActivityType Convert(WorkoutDataType workoutDataType)
        {
            return workoutDataType switch
            {
                WorkoutDataType.Biking => HKWorkoutActivityType.Cycling,
                WorkoutDataType.Running => HKWorkoutActivityType.Running,
                WorkoutDataType.Yoga => HKWorkoutActivityType.Yoga,
                WorkoutDataType.FunctionalStrengthTraining => HKWorkoutActivityType.FunctionalStrengthTraining,
                WorkoutDataType.TraditionalStrengthTraining => HKWorkoutActivityType.TraditionalStrengthTraining,
                WorkoutDataType.CoreTraining => HKWorkoutActivityType.CoreTraining,
                WorkoutDataType.Flexibility => HKWorkoutActivityType.Flexibility,
                WorkoutDataType.HighIntensityIntervalTraining => HKWorkoutActivityType.HighIntensityIntervalTraining,
                WorkoutDataType.MindAndBody => HKWorkoutActivityType.MindAndBody,
                _ => throw new NotSupportedException(),
            };
        }
    }
}