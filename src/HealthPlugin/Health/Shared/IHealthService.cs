using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public interface IHealthService
    {
        bool IsDataTypeAvailable(HealthDataType healthDataType);
        Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes);

        IHealthService DateRange(DateTime startDate, DateTime endDate);

        IHealthService Aggregate(AggregateTime aggregateTime);

        IHealthService AddDataType(HealthDataType healthDataType, AggregateType aggregateType = AggregateType.None);

        Task<IReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>> FetchDataAsync();
    }
}
