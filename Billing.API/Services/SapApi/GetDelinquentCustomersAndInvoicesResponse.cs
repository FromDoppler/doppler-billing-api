using Billing.API.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Billing.API.Services.SapApi
{
    public class GetDelinquentCustomersAndInvoicesResponse
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }

        public List<DelinquentCustomerAndInvoice> value { get; set; }
    }
}
