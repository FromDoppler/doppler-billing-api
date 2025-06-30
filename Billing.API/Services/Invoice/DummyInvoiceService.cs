using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Billing.API.DopplerSecurity;
using Billing.API.Models;
using Microsoft.Extensions.Options;

namespace Billing.API.Services.Invoice
{
    public class DummyInvoiceService : IInvoiceService
    {
        private readonly CryptoHelper _cryptoHelper;
        private readonly IOptions<InvoiceProviderOptions> _options;

        public DummyInvoiceService(CryptoHelper cryptoHelper, IOptions<InvoiceProviderOptions> options)
        {
            _cryptoHelper = cryptoHelper;
            _options = options;
        }

        public async Task<PaginatedResult<InvoiceListItem>> GetInvoices(string clientPrefix, int clientId, int page, int pageSize, string sortColumn, bool sortAsc)
        {
            if (clientId <= 0)
                throw new ArgumentException();

            var invoices = GetDummyInvoices(clientPrefix, clientId, page, pageSize, sortColumn, sortAsc);

            return await Task.FromResult(invoices);
        }

        public Task<byte[]> GetInvoiceFile(string clientPrefix, int clientId, string sapSystem, int fileId, string date, string documentType)
        {
            var bytes = File.ReadAllBytes("sample.pdf");

            return Task.FromResult(bytes);
        }

        public Task<PaginatedResult<DelinquentCustomerAndInvoice>> GetDelinquentCustomersAndInvoices(string sapSystem, string fromDate, string toDate, int page, int pageSize, string sortColumn, bool sortAsc, bool includePaymentTerms, bool includeBillingEmails)
        {
            var dummyData = new List<DelinquentCustomerAndInvoice>
            {
                new() {
                    CardCode = "CD00000120465.0",
                    CardName = "Test User",
                    Email = "test_error_total_mp_int_1@mailinator.com",
                    TotalToPay = 100,
                    UnpaidInvoices =  new List<UnpaidInvoice>
                    {
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 6),
                            DocDueDate = new DateTime(2025, 2, 16),
                            DocNum = 1,
                            DocTotal = 100,
                            FolioNumberFrom = sapSystem == "US" ? null : 1,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 100,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        }
                    }
                },
                new() {
                    CardCode = "CD00000120464.0",
                    CardName = "Test User 3",
                    Email = "test_billing_email_sap_int_1@mailinator.com",
                    TotalToPay = 1555.0m,
                    UnpaidInvoices =  new List<UnpaidInvoice>
                    {
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 3),
                            DocDueDate = new DateTime(2025, 2, 13),
                            DocNum = 4,
                            DocTotal = 100,
                            FolioNumberFrom = sapSystem == "US" ? null : 4,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 100,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        },
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 3),
                            DocDueDate = new DateTime(2025, 2, 13),
                            DocNum = 5,
                            DocTotal = 500.55m,
                            FolioNumberFrom = sapSystem == "US" ? null : 5,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 500.55m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        },
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 5),
                            DocDueDate = new DateTime(2025, 2, 15),
                            DocNum = 6,
                            DocTotal = 599,
                            FolioNumberFrom = sapSystem == "US" ? null : 6,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 599,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        },
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 4),
                            DocDueDate = new DateTime(2025, 2, 14),
                            DocNum = 7,
                            DocTotal = 300.45m,
                            FolioNumberFrom = sapSystem == "US" ? null : 7,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 300.45m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        }
                    }
                },
                new() {
                    CardCode = "CD00000120463.0",
                    CardName = "Test User 2",
                    Email = "test_onsite_int_1@mailinator.com",
                    TotalToPay = 150,
                    UnpaidInvoices =  new List<UnpaidInvoice>
                    {
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 6),
                            DocDueDate = new DateTime(2025, 2, 16),
                            DocNum = 2,
                            DocTotal = 100,
                            FolioNumberFrom = sapSystem == "US" ? null : 2,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 100,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        },
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 6),
                            DocDueDate = new DateTime(2025, 2, 16),
                            DocNum = 3,
                            DocTotal = 50,
                            FolioNumberFrom = sapSystem == "US" ? null : 3,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 50,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        }
                    }
                },
                new() {
                    CardCode = "CD00000120462.0",
                    CardName = "Test User",
                    Email = "test_buy_conversation_no_activated_int_1@mailinator.com",
                    TotalToPay = 75.25m,
                    UnpaidInvoices =  new List<UnpaidInvoice>
                    {
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 9),
                            DocDueDate = new DateTime(2025, 2, 19),
                            DocNum = 8,
                            DocTotal = 75.25m,
                            FolioNumberFrom = sapSystem == "US" ? null : 8,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 75.25m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        }
                    }
                },
                new() {
                    CardCode = "CD00000120461.0",
                    CardName = "Test User",
                    Email = "test_bin_validation_int_1@mailinator.com",
                    TotalToPay = 25.75m,
                    UnpaidInvoices =  new List<UnpaidInvoice>
                    {
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 1),
                            DocDueDate = new DateTime(2025, 2, 11),
                            DocNum = 9,
                            DocTotal = 25.75m,
                            FolioNumberFrom = sapSystem == "US" ? null : 9,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 25.75m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        }
                    }
                },
                new() {
                    CardCode = "CD00000120460.0",
                    CardName = "Test User",
                    Email = "hvarela+register-event4@makingsense.com",
                    TotalToPay = 128.75m,
                    UnpaidInvoices =  new List<UnpaidInvoice>
                    {
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 1, 1),
                            DocDueDate = new DateTime(2025, 1, 11),
                            DocNum = 10,
                            DocTotal = 25.75m,
                            FolioNumberFrom = sapSystem == "US" ? null : 10,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 25.75m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        },
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 1),
                            DocDueDate = new DateTime(2025, 2, 11),
                            DocNum = 11,
                            DocTotal = 25.75m,
                            FolioNumberFrom = sapSystem == "US" ? null : 11,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 25.75m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        },
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 3),
                            DocDueDate = new DateTime(2025, 2, 13),
                            DocNum = 12,
                            DocTotal = 25.75m,
                            FolioNumberFrom = sapSystem == "US" ? null : 12,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 25.75m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        },
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 4),
                            DocDueDate = new DateTime(2025, 2, 14),
                            DocNum = 13,
                            DocTotal = 25.75m,
                            FolioNumberFrom = sapSystem == "US" ? null : 13,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 25.75m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        },
                        new() {
                            DocCurrency = sapSystem == "US" ? "$" : "ARS",
                            DocDate = new DateTime(2025, 2, 5),
                            DocDueDate = new DateTime(2025, 2, 15),
                            DocNum = 14,
                            DocTotal = 25.75m,
                            FolioNumberFrom = sapSystem == "US" ? null : 14,
                            Letter = sapSystem == "US" ? null : "A",
                            PaidToDate = 0,
                            PointOfIssueCode = sapSystem == "US" ? null : "0001",
                            DocTotalUsd = 25.75m,
                            PaidToDateUsd = 0,
                            PaymentTerms = new PaymentTerms
                            {
                                NumberOfAdditionalDays = 0,
                                Name = "Cash Basic"
                            }
                        }
                    }
                }
            };

            var year = int.Parse(fromDate.Substring(0, 4));
            var month = int.Parse(fromDate.Substring(4, 2));
            var day = int.Parse(fromDate.Substring(6, 2));

            var from = new DateTime(year, month, day);

            year = int.Parse(toDate.Substring(0, 4));
            month = int.Parse(toDate.Substring(4, 2));
            day = int.Parse(toDate.Substring(6, 2));

            var to = new DateTime(year, month, day);

            var filteredDataDummy = new List<DelinquentCustomerAndInvoice>();

            foreach (var dummy in dummyData)
            {
                var data = new DelinquentCustomerAndInvoice()
                {
                    CardCode = dummy.CardCode,
                    CardName = dummy.CardName,
                    Email = dummy.Email,
                    TotalToPay = dummy.TotalToPay,
                    UnpaidInvoices = dummy.UnpaidInvoices.Where(ui => ui.DocDate >= from && ui.DocDate <= to).ToList(),
                };

                if (data.UnpaidInvoices.Any())
                {
                    filteredDataDummy.Add(data);
                }
            }

            return Task.FromResult(new PaginatedResult<DelinquentCustomerAndInvoice> { Items = filteredDataDummy, TotalItems = filteredDataDummy.Count });
        }

        public async Task<string> TestSapConnection()
        {
            return await Task.FromResult("Successful");
        }

        public async Task<string> TestSapUsConnection()
        {
            return await Task.FromResult("Successful");
        }

        private PaginatedResult<InvoiceListItem> GetDummyInvoices(string clientPrefix, int clientId, int page, int pageSize, string sortColumn, bool sortAsc)
        {
            var invoices = Enumerable.Range(1, 50).Select(x => new InvoiceListItem(
                x == 50 ? "NC" : "FC",
                $"A-0001-00000000{x}",
                $"Prod {x}",
                $"{clientPrefix}{clientId:0000000000000}",
                new DateTimeOffset(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, TimeSpan.Zero).AddDays(x),
                new DateTimeOffset(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, TimeSpan.Zero).AddDays(x + 15),
                DateTime.Today.AddDays(x),
                "ARS",
                x == 50 ? -100 : 15500,
                x == 50 ? -100 : 1000,
                $"invoice_AR_{DateTime.Today.AddDays(x):yyyy-MM-dd}_{x}.pdf",
                x)
            ).AsQueryable();

            var invoiceSorted = GetInvoicesSorted(invoices, sortColumn, sortAsc).ToList();
            var paginatedInvoices = invoiceSorted;

            if (page > 0 && pageSize > 0)
            {
                paginatedInvoices = invoiceSorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            return new PaginatedResult<InvoiceListItem> { Items = paginatedInvoices, TotalItems = invoiceSorted.Count };
        }

        private static IEnumerable<InvoiceListItem> GetInvoicesSorted(IQueryable<InvoiceListItem> invoices, string sortColumn, bool sortAsc)
        {
            return invoices.OrderBy(sortColumn + (!sortAsc ? " descending" : ""));
        }
    }
}
