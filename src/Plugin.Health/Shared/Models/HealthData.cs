﻿using System;

namespace Plugin.Health
{
    public interface IHealthData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate   { get; set; }
    }

    [Preserve(AllMembers = true)]
    public class HealthData : IHealthData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate   { get; set; }
        public double   Value     { get; set; }
        public bool     UserEntered { get; set; }
    }


    [Preserve(AllMembers = true)]
    public class WorkoutData : IHealthData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Duration { get; internal set; }
        public string Device { get; internal set; }
        public WorkoutDataType WorkoutType { get; internal set; }
        public double TotalDistance { get; internal set; }
        public double TotalEnergyBurned { get; internal set; }
    }

    [Preserve(AllMembers = true)]
    public class AggregatedHealthData : IHealthData
    {
        public DateTime StartDate   { get; set; }
        public DateTime EndDate     { get; set; }
        public double?  Max         { get; set; }
        public double?  Min         { get; set; }
        public double?  Sum         { get; set; }
        public double?  Average     { get; set; }
    }
}
