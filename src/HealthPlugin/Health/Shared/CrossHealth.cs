using System;

namespace Plugin.Health
{
    public partial class CrossHealth
    {
        static readonly Lazy<IHealthService> Implementation = new Lazy<IHealthService>(CreateHealthService, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => Implementation.Value != null;

        /// <summary>
        /// Current settings to use
        /// </summary>
        public static IHealthService Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IHealthService CreateHealthService()
        {
#if NETSTANDARD1_0
            return null;
#else
            return new HealthService();
#endif
        }

        private static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }

    }
}
