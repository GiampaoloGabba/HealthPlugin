using System;
using System.Threading.Tasks;

namespace Plugin.Health
{
    [Preserve(AllMembers = true)]
    public interface IHealthDataWriter
    {
        public Task<bool> WriteAsync(HealthDataType healthDataType, double value, DateTime start, DateTime? end = null);
        public Task<bool> WriteAsync(WorkoutDataType workoutDataType, double calories, DateTime start, DateTime? end = null, string name = null);
    }
}
