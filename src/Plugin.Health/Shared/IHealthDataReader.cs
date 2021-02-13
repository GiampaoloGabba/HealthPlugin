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
        /// Fetch data from HealthKit/Google fit
        /// </summary>
        IReadOnlyDictionary<HealthDataType, Task<IEnumerable<HealthData>>> Read(params HealthDataType[] healthDataType);

        /// <summary>
        /// Fetch data from HealthKit/Google fit
        /// </summary>
        Task<IEnumerable<WorkoutData>> ReadWorkouts();

        /// <summary>
        /// Fetch aggregated data from HealthKit/Google fit
        /// </summary>
        IReadOnlyDictionary<HealthDataType, Task<IEnumerable<AggregatedHealthData>>> ReadAggregate(AggregateTime aggregateTime, params HealthDataType[] healthDataType);
    }
}
