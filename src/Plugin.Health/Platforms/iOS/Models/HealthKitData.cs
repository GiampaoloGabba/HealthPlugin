using HealthKit;

namespace Plugin.Health
{
    public class HealthKitData
    {
        public enum HKTypes {
            Quantity,
            Characteristic,
            Category,
            Workout
        }

        public HealthKitData(HKQuantityTypeIdentifier identifier)
        {
            HKType = HKTypes.Quantity;
            QuantityTypeIdentifier = identifier;
            Permission = HKQuantityType.Create(identifier);
        }

        public HealthKitData(HKCharacteristicTypeIdentifier identifier)
        {
            HKType = HKTypes.Characteristic;
            CharacteristicTypeIdentifier = identifier;
            Permission = HKCharacteristicType.Create(identifier);
        }

        public HealthKitData(HKCategoryTypeIdentifier identifier)
        {
            CategoryTypeIdentifier = identifier;
            HKType = HKTypes.Category;
            Permission = HKCategoryType.Create(identifier);
        }

        public HealthKitData(HKWorkoutType identifier)
        {
            HKType = HKTypes.Workout;
            Permission = identifier;
        }

        public HKTypes HKType { get; }
        public HKQuantityTypeIdentifier QuantityTypeIdentifier { get; }
        public HKCharacteristicTypeIdentifier CharacteristicTypeIdentifier { get; }
        public HKCategoryTypeIdentifier CategoryTypeIdentifier { get; }

        public bool Cumulative { get; set; }
        public HKUnit Unit { get; set; }
        public HKObjectType Permission { get; }
    }
}
