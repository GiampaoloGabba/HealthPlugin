using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Health
{
    [Preserve(AllMembers = true)]
    public interface IHealthDataReader
    {
        /// <summary>
        /// Set the DateTime range to the data query
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        IHealthDataReader DateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Activate data aggregation in HealthKit/GoogleFit query
        /// </summary>
        /// <param name="aggregateTime">Time unit for the aggregaton</param>
        IHealthDataReader Aggregate(AggregateTime aggregateTime);

        /// <summary>
        /// Add HealthDataTypes to the data query
        /// </summary>
        /// <param name="healthDataType">HealthDataType to query</param>
        /// <param name="aggregateType">Aggregation Options (ignored if Aggregate is not set)</param>
        IHealthDataReader AddDataType(HealthDataType healthDataType, AggregateType aggregateType = AggregateType.None);

        /// <summary>
        /// Fetch data from HealthKit/Google fit
        /// </summary>
        Task<IReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>> FetchDataAsync();
    }
}
