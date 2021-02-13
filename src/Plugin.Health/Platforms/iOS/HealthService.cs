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
        //https://github.com/EddyVerbruggen/HealthKit

        //TODO: esplorare store e delete
        //TODO: esplorare attività
        //TODO: esplorare sessioni

        protected internal readonly HKHealthStore HealthStore;

        public HealthService()
        {
            if (HKHealthStore.IsHealthDataAvailable)
                HealthStore = new HKHealthStore();
            else
                Debug.WriteLine("HEALTHKIT is not available on this device!");
        }

        public override bool IsAvailable()
        {
            return HKHealthStore.IsHealthDataAvailable;
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

        public override async Task<bool> RequestPermissionAsync(HealthDataType[] writeDataTypes, HealthDataType[] readDataTypes)
        {

            if (HealthStore == null)
                throw new NotSupportedException("HEALTHKIT is not available on this device!");


            if(writeDataTypes.Where(t => t.ToHealthKit().HKType == HealthKitData.HKTypes.Characteristic).Any())
            {
                throw new NotSupportedException($"Characteristics cannot be written!");
            }

            var readPermissions = readDataTypes.Where(IsDataTypeAvailable).ToArray();
            var writePermissions = writeDataTypes.Where(IsDataTypeAvailable).ToArray();
            

            if (HKHealthStore.IsHealthDataAvailable && readPermissions.Any())
            {
                var (res, error) = await HealthStore.RequestAuthorizationToShareAsync(DataTypesToPermissions(writePermissions), DataTypesToPermissions(readPermissions)).ConfigureAwait(false);

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

        public override IHealthDataWriter DataWriter()
        {
            return new HealthDataWriter(this);
        }

        public override void PromptInstallGoogleFit()
        {
            //Do nothing
            //Consider if we should throw an exception....
            //throw new NotImplementedException();
        }

        NSSet DataTypesToPermissions(HealthDataType[] dataTypes)
        {
            var types = new HKObjectType[dataTypes.Length];

            for (var i = 0; i < dataTypes.Length; i++)
            {
                var dataType = dataTypes[i].ToHealthKit();

                types.SetValue(dataType.Permission, i);
            }

            return NSSet.MakeNSObjectSet(types);
        }
    }
}
