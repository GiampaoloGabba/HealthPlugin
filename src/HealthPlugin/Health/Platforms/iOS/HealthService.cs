using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HealthKit;

namespace Plugin.Health
{
    [Preserve(AllMembers = true)]
    public class HealthService : HealthServiceBase
    {
        protected internal readonly HKHealthStore HealthStore;

        public HealthService()
        {
            if (HKHealthStore.IsHealthDataAvailable)
                HealthStore = new HKHealthStore();
            else
                Debug.WriteLine("HEALTHKIT is not available on this device!");
        }

        public override bool IsDataTypeAvailable(HealthDataType healthDataType)
        {
            try
            {
                return healthDataType.ToHealthKit() != null;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.WriteLine($"HEALTHKIT - Datatype {healthDataType} is not supported in this device", e);
                return false;
            }
        }

        public override async Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes)
        {

            if (HealthStore == null)
                throw new NotSupportedException("HEALTHKIT is not available on this device!");

            var result = dataTypes.Where(IsDataTypeAvailable).ToArray();

            if (HKHealthStore.IsHealthDataAvailable && result.Any())
            {
                var (res, error) = await HealthStore.RequestAuthorizationToShareAsync(null, DataTypesToRead(result)).ConfigureAwait(false);

                if (error != null)
                    throw new Exception($"HEALTHKIT - Error during Permission request: {error.LocalizedDescription}");

                return true;
            }
            return false;
        }

        public override IHealthDataReader DataReader()
        {
            return new HealthDataReader(this);
        }

        NSSet DataTypesToRead(HealthDataType[] dataTypes)
        {
            var types = new HKObjectType[dataTypes.Length];
            for (var i = 0; i < dataTypes.Length; i++)
            {
                types.SetValue(HKQuantityType.Create(dataTypes[i].ToHealthKit().TypeIdentifier), i);
            }
            return NSSet.MakeNSObjectSet(types);
        }
    }
}
