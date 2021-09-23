using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.API.Services.SapApi
{
    public class SapLoginCookies
    {
        public string B1Session { get; set; }
        public string RouteId { get; set; }
        public DateTime SessionEndAt { get; set; }
    }
}
