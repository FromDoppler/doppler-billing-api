using System.Collections.Generic;
using System.Threading.Tasks;
using Billing.API.Models;

namespace Billing.API.Services.Invoice
{
    public interface IInvoiceService
    {
        Task<PaginatedResult<InvoiceListItem>> GetInvoices(string clientPrefix, int clientId, int page, int pageSize, string sortColumn, bool sortAsc);

        Task<byte[]> GetInvoiceFile(string clientPrefix, int clientId, string sapSystem, int fileId, string date, string documentType);

        Task<PaginatedResult<DelinquentCustomerAndInvoice>> GetDelinquentCustomersAndInvoices(string sapSystem, string fromDate, string toDate, int page, int pageSize, string sortColumn, bool sortAsc, bool includePaymentTerms, bool includeBillingEmails);

        Task<string> TestSapConnection();

        Task<string> TestSapUsConnection();
    }
}
