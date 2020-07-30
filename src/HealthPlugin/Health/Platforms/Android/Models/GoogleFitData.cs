using Android.Gms.Fitness.Data;

namespace Plugin.Health
{
    internal class GoogleFitData
    {
        public DataType TypeIdentifier          { get; set; }
        public DataType AggregateTypeIdentifier { get; set; }
        public Field    Unit                    { get; set; }

    }
}
