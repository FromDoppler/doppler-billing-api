using Newtonsoft.Json;
using System.Collections.Generic;

namespace Billing.API.Models
{
    public class DocumentList
    {

        [JsonProperty(PropertyName = "odata.metadata")]
        public string metadata { get; set; }
        [JsonProperty(PropertyName = "odata.count")]
        public string count { get; set; }
        
        [JsonProperty(PropertyName = "odata.nextLink")]
        public string nextLink { get; set; }
        public List<DocumentItem> value { get; set; }
    }
}
