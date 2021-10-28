using Billing.API.Enums;
using Billing.API.Extensions;
using Billing.API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Billing.API.Services.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ILogger<InvoiceService> _logger;
        private readonly IOptions<InvoiceProviderOptions> _options;
        private readonly ISapServiceSettingsService _sapServiceSettingsService;
        private readonly ISapApiService _sapApiService;

        public InvoiceService(ILogger<InvoiceService> logger,
            IOptions<InvoiceProviderOptions> options,
            ISapServiceSettingsService sapServiceSettingsService,
            ISapApiService sapApiService)
        {
            _logger  = logger;
            _options = options;
            _sapServiceSettingsService = sapServiceSettingsService;
            _sapApiService = sapApiService;
        }

        public async Task<PaginatedResult<InvoiceListItem>> GetInvoices(string clientPrefix, int clientId, int page, int pageSize, string sortColumn, bool sortAsc)
        {
            var sapSystems = _options.Value.ConfigsBySystem;
            IQueryable<InvoiceListItem> invoices = new List<InvoiceListItem>().AsQueryable();

            // Get invoices for all supported sap system because the client can be in more than one sap system.
            // For example: One client exists in SAP AR and then it changes the billing system to QBL then the client is created in the SAP US
            // for it reason we need get the invoices for all supported sap system.
            foreach (var sapSystem in sapSystems)
            {
                var documents = await GetInvoices(clientPrefix, clientId, sapSystem.Key, null);

                foreach(var document in documents)
                {
                    var documentNumber = string.IsNullOrEmpty(document.Letter) || string.IsNullOrEmpty(document.PointOfIssueCode) || string.IsNullOrEmpty(document.FolioNumberFrom) ?
                        document.DocNum : $"{(Enum.Parse< FolioLetterEnum>(document.Letter)).GetDescription()}-{document.PointOfIssueCode.PadLeft(4, '0')}-{document.FolioNumberFrom.PadLeft(8, '0')}";
                    invoices = invoices.Append(new InvoiceListItem(
                    document.DocumentType,
                    documentNumber,
                    clientPrefix,
                    clientId.ToString(),
                    document.DocDate.ToDateTimeOffSet(),
                    document.DocDueDate.ToDateTimeOffSet(),
                    document.DocDate.ToDateTimeOffSet(),
                    document.DocCurrency,
                    document.DocTotal.ToDouble(),
                    document.PaidToDate.ToDouble(),
                    $"invoice_{sapSystem.Key}_{document.DocumentType}_{document.DocDate:yyyy-MM-dd}_{document.DocNum}.pdf",
                    int.Parse(document.DocNum)));
                }
            }

            // TODO: When we can get the data from database we will try to move the pagination into the sql query
            var invoiceSorted = GetInvoicesSorted(invoices, sortColumn, sortAsc).ToList();
            var paginatedInvoices = invoiceSorted;

            if ((page > 0) && (pageSize > 0))
            {
                paginatedInvoices = invoiceSorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            return new PaginatedResult<InvoiceListItem> { Items = paginatedInvoices, TotalItems = invoiceSorted.Count };
        }

        public async Task<byte[]> GetInvoiceFile(string clientPrefix, int clientId, string sapSystem, int fileId, string date, string documentType)
        {
            var response = await _sapApiService.GetAttachmentPdf(fileId, date, documentType, sapSystem);
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<string> TestSapConnection()
        {
            var sapResponse = await _sapApiService.TestSapConnection();
            if (sapResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("[TEST] Connect with Sap correctly");
            }
            else
            {
                _logger.LogInformation("[TEST] Connect with Sap failed");
            }

            return $"Success {sapResponse.StatusCode}";
        }

        public async Task<string> TestSapUsConnection()
        {
            var sapResponse = await _sapApiService.TestSapUsConnection();
            if (sapResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("[TEST] Connect with Sap correctly");
            }
            else
            {
                _logger.LogInformation("[TEST] Connect with Sap failed");
            }

            return $"Success {sapResponse.StatusCode}";
        }

        private async Task<IEnumerable<DocumentItem>> GetInvoices(string clientPrefix, int clientId, string sapSystem, int? fileId = null)
        {
            var invoices = new List<DocumentItem>();

            /* Invoices */
            var response = await _sapApiService.GetInvoices("Invoices", clientPrefix, clientId, sapSystem, fileId);
            invoices.AddRange(response);

            /* CreditNotes */
            response = await _sapApiService.GetInvoices("CreditNotes", clientPrefix, clientId, sapSystem, fileId);
            invoices.AddRange(response);

            return invoices;
        }

        private static IEnumerable<InvoiceListItem> GetInvoicesSorted(IQueryable<InvoiceListItem> invoices, string sortColumn, bool sortAsc)
        {
            return invoices.OrderBy(sortColumn + (!sortAsc ? " descending" : ""));
        }
    }
}
