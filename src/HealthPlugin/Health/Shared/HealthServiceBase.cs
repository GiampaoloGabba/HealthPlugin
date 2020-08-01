using System.Threading.Tasks;

namespace Plugin.Health
{
    public abstract class HealthServiceBase : IHealthService
    {
        public abstract bool IsAvailable();
        public abstract bool IsDataTypeAvailable(HealthDataType healthDataType);
        public abstract Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes);
        public abstract IHealthDataReader DataReader();

        //platforms
        public abstract void PromptInstallGoogleFit();
    }
}
