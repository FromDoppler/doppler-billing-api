using Newtonsoft.Json;

namespace Billing.API.Services.SapApi
{
    public class SapContactEmployee
    {
        public string CardCode { get; set; }

        [JsonProperty(PropertyName = "E_Mail")]
        public string Email { get; set; }
        public string EmailGroupCode { get; set; }
    }
}
