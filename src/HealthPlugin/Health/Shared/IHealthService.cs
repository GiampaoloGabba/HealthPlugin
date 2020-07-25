using System;
using System.Collections.Generic;
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
        /// Set the DateTime range to the data query
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        IHealthService DateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Activate data aggregation in HealthKit/GoogleFit query
        /// </summary>
        /// <param name="aggregateTime">Time unit for the aggregaton</param>
        IHealthService Aggregate(AggregateTime aggregateTime);

        /// <summary>
        /// Add HealthDataTypes to the data query
        /// </summary>
        /// <param name="healthDataType">HealthDataType to query</param>
        /// <param name="aggregateType">Aggregation Options (ignored if Aggregate is not set)</param>
        IHealthService AddDataType(HealthDataType healthDataType, AggregateType aggregateType = AggregateType.None);

        /// <summary>
        /// Fetch data from HealthKit/Google fit
        /// </summary>
        Task<IReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>> FetchDataAsync();
    }
}
