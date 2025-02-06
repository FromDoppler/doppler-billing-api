using Newtonsoft.Json;
using System;

namespace Billing.API.Models
{
    public class UnpaidInvoice
    {
        public string DocCurrency { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public int DocNum { get; set; }
        public decimal DocTotal { get; set; }
        public int? FolioNumberFrom { get; set; }
        public string Letter { get; set; }
        public string PointOfIssueCode { get; set; }
        public decimal PaidToDate { get; set; }
    }
}
