﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public abstract class BaseHealthService : IHealthService
    {
        private readonly Dictionary<HealthDataType, AggregateType> _selectedDataTypes = new Dictionary<HealthDataType, AggregateType>();
        private AggregateTime _aggregateTime = AggregateTime.None;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate   = DateTime.Now;

        public abstract bool IsDataTypeAvailable(HealthDataType healthDataType);

        public abstract Task<bool> RequestPermissionAsync(params HealthDataType[] dataTypes);

        public async Task<IReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>> FetchDataAsync()
        {
            var dic = new Dictionary<HealthDataType, IEnumerable<HealthData>>();

            if (dic.Count == 0)
                throw new InvalidOperationException("No DataTypes specified! Please use .AddDataType before calling .FetchDataAsync");

            foreach (var data in _selectedDataTypes)
            {
                dic.Add(data.Key, await QueryAsync(data.Key, data.Value, _aggregateTime, _startDate, _endDate));
            }

            return new ReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>(dic);
        }

        public IHealthService AddDataType(HealthDataType healthDataType, AggregateType aggregateType)
        {
            if (!_selectedDataTypes.ContainsKey(healthDataType))
                _selectedDataTypes.Add(healthDataType, aggregateType);
            return this;
        }

        public IHealthService DateRange(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate   = endDate;
            return this;
        }

        public IHealthService Aggregate(AggregateTime aggregateTime)
        {
            _aggregateTime = aggregateTime;
            return this;
        }

        protected abstract Task<IEnumerable<HealthData>> QueryAsync(HealthDataType dataTypes,
                                                                    AggregateType aggregateType,
                                                                    AggregateTime aggregateTime,
                                                                    DateTime startDate, DateTime endDate);

    }
}
