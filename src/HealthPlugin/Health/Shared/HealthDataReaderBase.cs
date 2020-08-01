﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
 using System.Linq;
 using System.Threading.Tasks;

namespace Plugin.Health
{
    public abstract class HealthDataReaderBase : IHealthDataReader
    {
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate   = DateTime.Now;

        public IHealthDataReader DateRange(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate   = endDate;
            return this;
        }

        public IReadOnlyDictionary<HealthDataType, Task<IEnumerable<HealthData>>> Read(params HealthDataType[] healthDataTypes)
        {
            return FetchData<HealthData>(AggregateTime.None,healthDataTypes);
        }

        public IReadOnlyDictionary<HealthDataType, Task<IEnumerable<AggregatedHealthData>>> ReadAggregate(AggregateTime aggregateTime,params HealthDataType[] healthDataTypes)
        {
            return FetchData<AggregatedHealthData>(aggregateTime,healthDataTypes);
        }

        private IReadOnlyDictionary<HealthDataType, Task<IEnumerable<T>>> FetchData<T>(AggregateTime aggregateTime,params HealthDataType[] healthDataTypes) where T : class, IHealthData
        {
            if (healthDataTypes == null || healthDataTypes.Length == 0)
                throw new InvalidOperationException("No DataTypes specified! Please use .AddDataType before calling .FetchDataAsync");

            var dic = healthDataTypes.ToDictionary(dataType => dataType,
                dataType => Query<T>(dataType, aggregateTime, _startDate, _endDate));

            return new ReadOnlyDictionary<HealthDataType, Task<IEnumerable<T>>>(dic);
        }

        protected abstract Task<IEnumerable<T>> Query<T>(HealthDataType dataTypes,
                                                                   AggregateTime aggregateTime,
                                                                   DateTime startDate, DateTime endDate) where T: class, IHealthData;
    }
}
