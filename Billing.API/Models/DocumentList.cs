using Newtonsoft.Json;
using System.Collections.Generic;

namespace Billing.API.Models
{
    public class DocumentList
    {

        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }
        public List<DocumentItem> value { get; set; }
    }
}
