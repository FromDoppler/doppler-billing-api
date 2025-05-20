using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Billing.API.Services.SapApi
{
    public class DelinquentCustomerAndInvoice
    {
        public string CardCode {  get; set; }
        public string CardName { get; set; }
        public IList<string> BillingEmails { get; set; }
        [JsonProperty(PropertyName = "DocCur")]
        public string DocCurrency { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public int DocNum { get; set; }
        public decimal DocTotal { get; set; }

        [JsonProperty(PropertyName = "E_Mail")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "FolNumFrom")]
        public int? FolioNumberFrom { get; set; }
        public string Letter { get; set; }

        [JsonProperty(PropertyName = "PTICode")]
        public string PointOfIssueCode { get; set; }
        public decimal PaidToDate { get; set; }

        [JsonProperty(PropertyName = "GroupNum")]
        public int PaymentTermsType { get; set; }

        [JsonProperty(PropertyName = "ExtraDays")]
        public int PaymentTermsExtraDays { get; set; }

        [JsonProperty(PropertyName = "ExtraMonth")]
        public int PaymentTermsExtraMonth { get; set; }

        public DateTime DocDateAsDateTime
        {
            get
            {
                return new DateTime(int.Parse(DocDate.Substring(0, 4)), int.Parse(DocDate.Substring(4, 2)), int.Parse(DocDate.Substring(6, 2)));
            }
        }
        public DateTime DocDueDateAsDateTime
        {
            get
            {
                return new DateTime(int.Parse(DocDueDate.Substring(0, 4)), int.Parse(DocDueDate.Substring(4, 2)), int.Parse(DocDueDate.Substring(6, 2)));
            }
        }

        [JsonProperty(PropertyName = "DocTotalSy")]
        public decimal DocTotalUsd { get; set; }
        [JsonProperty(PropertyName = "PaidSys")]
        public decimal PaidToDateUsd { get; set; }
    }
}
