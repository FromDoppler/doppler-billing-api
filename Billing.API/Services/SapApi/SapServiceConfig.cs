using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.API.Services.SapApi
{
    public class SapServiceConfig
    {
        public string BaseServerUrl { get; set; }
        public string CompanyDB { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
