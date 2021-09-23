using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.API.Services.SapApi
{
    public class SapServiceSettings
    {
        public static SapServiceConfig GetSettings(SapConfig sapConfig, string sapSystem)
        {
            if (!sapConfig.SapServiceConfigsBySystem.TryGetValue(sapSystem, out var serviceSettings))
            {
                throw new ArgumentException($"The sapSystem '{sapSystem}' does not have settings.", sapSystem);
            }

            return serviceSettings;
        }
    }
}
