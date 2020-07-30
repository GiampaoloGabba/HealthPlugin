﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public abstract class HealthDataReaderBase : IHealthDataReader
    {
        private readonly Dictionary<HealthDataType, AggregateType> _selectedDataTypes = new Dictionary<HealthDataType, AggregateType>();
        private AggregateTime _aggregateTime = AggregateTime.None;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate   = DateTime.Now;

        public async Task<IReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>> FetchDataAsync()
        {
            var dic = new Dictionary<HealthDataType, IEnumerable<HealthData>>();

            if (dic.Count == 0)
                throw new InvalidOperationException("No DataTypes specified! Please use .AddDataType before calling .FetchDataAsync");

            foreach (var data in _selectedDataTypes)
            {
                dic.Add(data.Key, await QueryAsync(data.Key, data.Value, _aggregateTime, _startDate, _endDate).ConfigureAwait(false));
            }

            return new ReadOnlyDictionary<HealthDataType, IEnumerable<HealthData>>(dic);
        }

        public IHealthDataReader AddDataType(HealthDataType healthDataType, AggregateType aggregateType)
        {
            if (!_selectedDataTypes.ContainsKey(healthDataType))
                _selectedDataTypes.Add(healthDataType, aggregateType);
            return this;
        }

        public IHealthDataReader DateRange(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate   = endDate;
            return this;
        }

        public IHealthDataReader Aggregate(AggregateTime aggregateTime)
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
