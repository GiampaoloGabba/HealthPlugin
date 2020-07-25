﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public abstract class BaseHealthService : IHealthService
    {
        protected DateTime StartDate = DateTime.Today;
        protected DateTime EndDate = DateTime.Now;
        protected AggregateTime AggregateTime = AggregateTime.None;

        public abstract bool IsDataTypeAvailable(HealthDataType healthDataType);

        public abstract Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes);

        public abstract Task<IEnumerable<HealthData>> FetchDataAsync(HealthDataType healthDataType);

        public async Task<IReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>> FetchMultipleDataAsync(params HealthDataType[] dataTypes)
        {
            var dic = new Dictionary<HealthDataType, IEnumerable<HealthData>>();

            foreach (var dataType in dataTypes)
            {
                dic.Add(dataType, await FetchDataAsync(dataType));
            }

            return new ReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>(dic);
        }

        public IHealthService SetDateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate   = endDate;
            return this;
        }

        public IHealthService SetStartDate(DateTime startDate)
        {
            StartDate = startDate;
            return this;
        }

        public IHealthService SetEndDate(DateTime endDate)
        {
            EndDate = endDate;
            return this;
        }

        public IHealthService SetAggregateType(AggregateTime aggregateTime)
        {
            AggregateTime = aggregateTime;
            return this;
        }

    }
}
