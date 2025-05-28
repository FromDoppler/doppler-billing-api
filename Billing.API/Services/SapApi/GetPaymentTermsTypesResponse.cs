using Newtonsoft.Json;
using System.Collections.Generic;

namespace Billing.API.Services.SapApi
{
    public class GetPaymentTermsTypesResponse
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }

        public List<GetPaymentTermsTypeByIdResponse> value { get; set; }
    }
}
