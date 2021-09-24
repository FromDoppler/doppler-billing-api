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

        public async Task<HttpResponseMessage> GetAttachmentPdf(int absEntry, string fileName, string fileExt, string sapSystem)
        {
            var serviceSetting = SapServiceSettings.GetSettings(_sapConfig, sapSystem);

            var message = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{serviceSetting.BaseServerUrl}Attachments2({absEntry})/$value?filename='{fileName}.{fileExt}'"),
                Method = HttpMethod.Get
            };

            var cookies = await StartSession(serviceSetting);
            message.Headers.Add("Cookie", cookies.B1Session);
            message.Headers.Add("Cookie", cookies.RouteId);

            var client = _httpClientFactory.CreateClient();
            return await client.SendAsync(message);
        }

        private async Task<SapLoginCookies> StartSession(SapServiceConfig serviceSetting)
        {
            if (_sapCookies is null || DateTime.UtcNow > _sapCookies.SessionEndAt)
            {
                try
                {
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
            }

            return _sapCookies;
        }
    }
}
