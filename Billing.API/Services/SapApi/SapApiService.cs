using Billing.API.Services.SapApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Billing.API.Models;
using System.Reflection.PortableExecutable;

namespace Billing.API.Services
{
    public class SapApiService : ISapApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private SapLoginCookies _sapCookies;
        private readonly SapConfig _sapConfig;

        public SapApiService(IHttpClientFactory httpClientFactory,
            IOptions<SapConfig> sapConfig)
        {
            _httpClientFactory = httpClientFactory;
            _sapConfig = sapConfig.Value;
        }

        public async Task<HttpResponseMessage> GetAttachmentPdf(int fileId, string date, string documentType, string sapSystem)
        {
            var serviceSetting = SapServiceSettings.GetSettings(_sapConfig, sapSystem);

            var sentDocuments = await GetSentDocumentsFromSap(fileId, date, documentType, sapSystem);

            if (sentDocuments.value.Count > 0)
            {
                var attachment = GetAttachmentItem(sentDocuments.value, fileId);
                if (attachment != null)
                {
                    var message = new HttpRequestMessage()
                    {
                        RequestUri = new Uri($"{serviceSetting.BaseServerUrl}Attachments2({attachment.AbsEntry})/$value?filename='{attachment.FileName}.{attachment.FileExt}'"),
                        Method = HttpMethod.Get
                    };

                    var cookies = await StartSession(sapSystem);
                    message.Headers.Add("Cookie", cookies.B1Session);
                    message.Headers.Add("Cookie", cookies.RouteId);

                    var client = _httpClientFactory.CreateClient();
                    return await client.SendAsync(message);
                }
            }

            return null;
        }

        public async Task<IList<DocumentItem>> GetInvoices(string documentType, string clientPrefix, int clientId, string sapSystem, int? fileId = null)
        {
            var serviceSetting = SapServiceSettings.GetSettings(_sapConfig, sapSystem);
            var selectColumns = "$select=CardCode, CardName, DocObjectCode, DocEntry, DocNum, Letter, PointOfIssueCode, FolioNumberFrom, DocDate, DocDueDate, DocCurrency, DocTotal, PaidToDate";
            var filter = $"$filter=startswith(CardCode,'{clientPrefix}{clientId:00000000000}.') or startswith(CardCode,'{clientPrefix}{clientId:0000000000000}')";
            var url = $"{serviceSetting.BaseServerUrl}{documentType}?{selectColumns}&{filter}&$inlinecount=allpages";

            var message = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            var cookies = await StartSession(sapSystem);
            message.Headers.Add("Cookie", cookies.B1Session);
            message.Headers.Add("Cookie", cookies.RouteId);

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(message);
            var result = response.Content.ReadAsStringAsync().Result;
            var documentsFromSap = JsonConvert.DeserializeObject<DocumentList>(result);

            var hasMoreDocuments = !string.IsNullOrEmpty(documentsFromSap.nextLink);

            var allDocuments = documentsFromSap.value;

            while (hasMoreDocuments)
            {
                message = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"{serviceSetting.BaseServerUrl}/{documentsFromSap.nextLink}"),
                    Method = HttpMethod.Get
                };

                message.Headers.Add("Cookie", cookies.B1Session);
                message.Headers.Add("Cookie", cookies.RouteId);

                client = _httpClientFactory.CreateClient();
                response = await client.SendAsync(message);
                result = response.Content.ReadAsStringAsync().Result;

                documentsFromSap = JsonConvert.DeserializeObject<DocumentList>(result);

                if (documentsFromSap.value.Count > 0)
                {
                    allDocuments.AddRange(JsonConvert.DeserializeObject<DocumentList>(result).value);
                }

                hasMoreDocuments = !string.IsNullOrEmpty(documentsFromSap.nextLink);
            }


            var documents = new List<DocumentItem>();

            foreach (DocumentItem document in allDocuments)
            {
                var hasSentDocument = await HasSentDocument(document.DocNum.ToInt32(), documentType == "Invoices" ? "FC" : "NC", document.DocDate, sapSystem);
                if (hasSentDocument)
                {
                    documents.Add(document);
                }
            }

            return documents;
        }

        public async Task<HttpResponseMessage> TestSapUsConnection()
        {
            return await Login("US");
        }

        public async Task<HttpResponseMessage> TestSapConnection()
        {
            return await Login("AR");
        }

        public async Task<IList<SapApi.DelinquentCustomerAndInvoice>> GetDelinquentCustomersAndInvoices(string sapSystem, string fromDate, string toDate)
        {
            var serviceSetting = SapServiceSettings.GetSettings(_sapConfig, sapSystem);
            var url = $"{serviceSetting.BaseServerUrl}SQLQueries('DelinquentCustomer')/List?fromDate='{fromDate}'&toDate='{toDate}'";

            var message = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            var cookies = await StartSession(sapSystem);
            message.Headers.Add("Cookie", cookies.B1Session);
            message.Headers.Add("Cookie", cookies.RouteId);
            message.Headers.Add("Prefer", $"odata.maxpagesize={0}");

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(message);
            var result = response.Content.ReadAsStringAsync().Result;
            var responseFromGetDelinquentCustomersAndInvoices = JsonConvert.DeserializeObject<GetDelinquentCustomersAndInvoicesResponse>(result);
            var delinquentCustomerAndInvoices = responseFromGetDelinquentCustomersAndInvoices.value;

            return delinquentCustomerAndInvoices;
        }

        private async Task<GetContactEmployeesResponse> GetContactExployeesByCardCode(string cardCode, string sapSystem)
        {
            var serviceSetting = SapServiceSettings.GetSettings(_sapConfig, sapSystem);
            var url = $"{serviceSetting.BaseServerUrl}BusinessPartners('{cardCode}')?$select=ContactEmployees";
            var message = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            var cookies = await StartSession(sapSystem);
            message.Headers.Add("Cookie", cookies.B1Session);
            message.Headers.Add("Cookie", cookies.RouteId);
            message.Headers.Add("Prefer", "odata.maxpagesize=0");

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(message);

            var result = response.Content.ReadAsStringAsync().Result;
            var contactEmployees = JsonConvert.DeserializeObject<GetContactEmployeesResponse>(result);

            return contactEmployees;
        }

        private async Task<SapLoginCookies> StartSession(string sapSystem)
        {
            try
            {
                var sapResponse = await Login(sapSystem);
                sapResponse.EnsureSuccessStatusCode();

                var sessionTimeout = JObject.Parse(await sapResponse.Content.ReadAsStringAsync());
                _sapCookies = new SapLoginCookies
                {
                    B1Session = sapResponse.Headers.GetValues("Set-Cookie").Where(x => x.Contains("B1SESSION"))
                        .Select(y => y.ToString().Substring(0, 46)).FirstOrDefault(),
                    RouteId = sapResponse.Headers.GetValues("Set-Cookie").Where(x => x.Contains("ROUTEID"))
                        .Select(y => y.ToString().Substring(0, 14)).FirstOrDefault(),
                    SessionEndAt = DateTime.UtcNow.AddMinutes((double)sessionTimeout["SessionTimeout"] - _sapConfig.SessionTimeoutPadding)
                };

            }
            catch (Exception e)
            {
                throw;
            }

            return _sapCookies;
        }

        private async Task<AttachmentList> GetSentDocumentsFromSap(int fileId, string date, string documentType, string sapSystem)
        {
            var serviceSetting = SapServiceSettings.GetSettings(_sapConfig, sapSystem);

            var prefixFileName = documentType == "FC" ? serviceSetting.InvoiceFileNamePrefix : serviceSetting.CreditNoteFileNamePrefix;

            var cookies = await StartSession(sapSystem);

            var prefixes = prefixFileName.Split(',');

            foreach (var prefix in prefixes)
            {

                var year = date.Split("-")[0];
                var patternFile = $"{prefix}%25_{fileId}_{year.Substring(0, year.Length - 1)}";
                var message = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"{serviceSetting.BaseServerUrl}SQLQueries('Attachment')/List?docNum='{patternFile}_%25'"),
                    Method = HttpMethod.Get
                };

                message.Headers.Add("Cookie", cookies.B1Session);
                message.Headers.Add("Cookie", cookies.RouteId);

                var client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(message);

                var result = response.Content.ReadAsStringAsync().Result;
                var attachments = JsonConvert.DeserializeObject<AttachmentList>(result);

                if (attachments.value.Count > 0)
                {
                    return JsonConvert.DeserializeObject<AttachmentList>(result);
                }
            }

            return new AttachmentList { value = new List<AttachmentItem>() };
        }

        private AttachmentItem GetAttachmentItem(IList<AttachmentItem> attachments, int fileId)
        {
            AttachmentItem attachment = null;

            foreach (var a in attachments)
            {
                var values = a.FileName.Split("_");
                var docNumber = values.Length > 1 ? values[1] : string.Empty;

                attachment = docNumber == fileId.ToString() ? a : null;

                if (attachment != null)
                    break;
            }

            return attachment;
        }

        private async Task<bool> HasSentDocument(int documentNumber, string documentType, DateTime documentDate, string sapSystem)
        {
            var attachments = await GetSentDocumentsFromSap(documentNumber, documentDate.Year.ToString(), documentType, sapSystem);

            if (attachments.value.Count > 0)
            {
                var attachment = GetAttachmentItem(attachments.value, documentNumber);
                return attachment != null;
            }

            return false;
        }

        private async Task<HttpResponseMessage> Login(string sapSystem)
        {
            var serviceSetting = SapServiceSettings.GetSettings(_sapConfig, sapSystem);
            var client = _httpClientFactory.CreateClient();
            var sapResponse = await client.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri($"{serviceSetting.BaseServerUrl}Login/"),
                Content = new StringContent(JsonConvert.SerializeObject(
                        new SapServiceConfig
                        {
                            CompanyDB = serviceSetting.CompanyDB,
                            Password = serviceSetting.Password,
                            UserName = serviceSetting.UserName
                        }),
                    Encoding.UTF8,
                    "application/json"),
                Method = HttpMethod.Post
            });

            return sapResponse;
        }
    }
}
