using System.Collections.Generic;

namespace Billing.API.Services.SapApi
{
    public class SapConfig
    {
        public int SessionTimeoutPadding { get; set; }
        public Dictionary<string, SapServiceConfig> SapServiceConfigsBySystem { get; set; }
    }
}
