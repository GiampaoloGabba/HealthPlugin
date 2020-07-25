using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public interface IHealthService
    {
        bool IsDataTypeAvailable(HealthData.DataType dataType);
        Task<bool> RequestPermissionAsync(params HealthData.DataType[] dataTypes);

        IHealthService SetSDateRange(DateTime startDate, DateTime endDate);

        IHealthService SetStartDate(DateTime startDate);

        IHealthService SetEndDate(DateTime endDate);

        IHealthService SetAggregateType(HealthData.AggregateType aggregateType);

        Task<List<HealthData>> FetchDataAsync(HealthData.DataType dataType, bool requestPermissionIfNeeded = false);
    }
}
