using Newtonsoft.Json;

namespace Billing.API.Services.SapApi
{
    public class GetPaymentTermsTypeByIdResponse
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }

        [JsonProperty(PropertyName = "GroupNumber")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "PaymentTermsGroupName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "NumberOfAdditionalDays")]
        public int AdditionalDays { get; set; }

        [JsonProperty(PropertyName = "NumberOfAdditionalMonths")]
        public int AdditionalMonths { get; set; }
    }
}
