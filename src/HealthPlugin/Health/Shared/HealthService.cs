using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public partial class HealthService
    {
        DateTime _defaultStartDate = DateTime.Today;
        DateTime _defaultEndDate = DateTime.Now;
        HealthData.AggregateType _aggregateType = HealthData.AggregateType.None;

        public IHealthService SetSDateRange(DateTime startDate, DateTime endDate)
        {
            _defaultStartDate = startDate;
            _defaultEndDate   = endDate;
            return this;
        }

        public IHealthService SetStartDate(DateTime startDate)
        {
            _defaultStartDate = startDate;
            return this;
        }

        public IHealthService SetEndDate(DateTime endDate)
        {
            _defaultEndDate = endDate;
            return this;
        }

        public IHealthService SetAggregateType(HealthData.AggregateType aggregateType)
        {
            _aggregateType = aggregateType;
            return this;
        }

        public async Task<List<HealthData>> FetchDataAsync(HealthData.DataType dataType, bool requestPermissionIfNeeded = false)
        {
            return await FetchDataAsync(dataType, _defaultStartDate, _defaultEndDate, requestPermissionIfNeeded);
        }

    }
}
