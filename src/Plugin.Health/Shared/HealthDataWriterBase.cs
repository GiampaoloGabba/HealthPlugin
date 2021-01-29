using System;
using System.Threading.Tasks;

namespace Plugin.Health
{
    public abstract class HealthDataWriterBase : IHealthDataWriter
    {
        public abstract Task<bool> WriteAsync(HealthDataType healthDataType, double value, DateTime start, DateTime? end);
        public abstract Task<bool> WriteAsync(WorkoutDataType workoutDataType, double calories, DateTime start, DateTime? end = null, string name = null);
    }
}
