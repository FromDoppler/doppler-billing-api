using Billing.API.Models;
using Billing.API.Services.SapApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Billing.API.Services
{
    public interface ISapApiService
    {
        Task<HttpResponseMessage> GetAttachmentPdf(int fileId, string date, string documentType, string sapSystem);

        Task<IList<DocumentItem>> GetInvoices(string documentType, string clientPrefix, int clientId, string sapSystem, int? fileId = null);

        Task<IList<SapApi.DelinquentCustomerAndInvoice>> GetDelinquentCustomersAndInvoices(string sapSystem, string fromDate, string toDate);

        Task<GetContactEmployeesResponse> GetContactExployeesByCardCode(string cardCode, string sapSystem);

        Task<GetPaymentTermsTypeResponse> GetPaymentTermsTypeById(int id, string sapSystem);

        Task<HttpResponseMessage> TestSapUsConnection();

        Task<HttpResponseMessage> TestSapConnection();
    }
}
