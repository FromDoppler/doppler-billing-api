using Newtonsoft.Json;
using System.Collections.Generic;

namespace Billing.API.Models
{
    public class DelinquentCustomerAndInvoice
    {
        public string CardCode {  get; set; }
        public string CardName { get; set; }
        public string Email { get; set; }
        public decimal TotalToPay { get; set; }
        public IEnumerable<UnpaidInvoice> UnpaidInvoices { get; set; }
    }
}
