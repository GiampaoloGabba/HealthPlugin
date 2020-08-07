using Android.Gms.Fitness.Data;

namespace Plugin.Health
{
    internal class GoogleFitData
    {
        public DataType TypeIdentifier { get; set; }
        public DataType AggregateType  { get; set; }
        public bool     Cumulative     { get; set; }
        public Field    Unit           { get; set; }

        public Field MinOverride     { get; set; }
        public Field MaxOverride     { get; set; }
        public Field AverageOverride { get; set; }

    }
}
