using Newtonsoft.Json;
using System.Collections.Generic;

namespace Billing.API.Models
{
    public class AttachmentList
    {

        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }
        public List<AttachmentItem> value { get; set; }
    }
}
