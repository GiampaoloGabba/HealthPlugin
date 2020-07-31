using System.Threading.Tasks;

namespace Plugin.Health
{
    [Preserve(AllMembers = true)]
    public interface IHealthService
    {
        /// <summary>
        /// Check if the HealthDataType is supported by the platform
        /// </summary>
        /// <param name="healthDataType">HealthDataType to check</param>
        bool IsDataTypeAvailable(HealthDataType healthDataType);

        /// <summary>
        /// Request permission di read the requested HealthDataTypes
        /// </summary>
        /// <param name="dataTypes">HealthDataTypes to check</param>
        /// <returns></returns>
        Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes);

        /// <summary>
        /// Instantiate the DataReader for HealthKit/GoogleFit
        /// </summary>
        /// <returns></returns>
        IHealthDataReader DataReader();
    }
}
