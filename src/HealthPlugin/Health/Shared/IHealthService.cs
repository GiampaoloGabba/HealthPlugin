using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public interface IHealthService
    {
        bool IsDataTypeAvailable(HealthDataType healthDataType);
        Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes);

        IHealthService SetDateRange(DateTime startDate, DateTime endDate);

        IHealthService SetStartDate(DateTime startDate);

        IHealthService SetEndDate(DateTime endDate);

        IHealthService SetAggregateType(AggregateTime aggregateTime);

        public Task<IEnumerable<HealthData>> FetchDataAsync(HealthDataType healthDataType);

        Task<IReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>> FetchMultipleDataAsync(params HealthDataType[] dataTypes);
    }
}
