using System;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public abstract class HealthDataWriterBase : IHealthDataWriter
    {
        public virtual Task<bool> WriteAsync(HealthDataType healthDataType, double value, DateTime start, DateTime? end)
        {
            throw new NotImplementedException();
        }
    }
}
