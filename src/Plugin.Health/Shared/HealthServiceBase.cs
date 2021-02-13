using System.Threading.Tasks;


namespace Plugin.Health
{
    public abstract class HealthServiceBase : IHealthService
    {
        public abstract bool IsAvailable();
        public abstract bool IsDataTypeAvailable(HealthDataType healthDataType);
        public abstract Task<bool> RequestPermissionAsync(HealthDataType[] writeDataTypes, HealthDataType[] readDataTypes);
        public abstract IHealthDataReader DataReader();
        public abstract IHealthDataWriter DataWriter();

        //platforms
        public abstract void PromptInstallGoogleFit();
    }
}
