using Newtonsoft.Json;

namespace Billing.API.Services.SapApi
{
    public class GetPaymentTermsTypeResponse
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }

        [JsonProperty(PropertyName = "PaymentTermsGroupName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "NumberOfAdditionalDays")]
        public int AdditionalDays { get; set; }

        [JsonProperty(PropertyName = "NumberOfAdditionalMonths")]
        public int AdditionalMonths { get; set; }
    }
}
