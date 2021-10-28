using System;

namespace Billing.API.Models
{
    public class DocumentItem
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocObjectCode { get; set; }
        public int DocEntry { get; set; }
        public string DocNum { get; set; }
        public string Letter { get; set; }
        public string PointOfIssueCode { get; set; }
        public string FolioNumberFrom { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string DocCurrency { get; set; }
        public decimal DocTotal { get; set; }
        public decimal PaidToDate { get; set; }
        public string DocumentType { get { return DocObjectCode == "oCreditNotes" ? "NC" : "FC"; } }
    }
}
