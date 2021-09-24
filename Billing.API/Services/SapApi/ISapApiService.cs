using System.Net.Http;
using System.Threading.Tasks;

namespace Billing.API.Services
{
    public interface ISapApiService
    {
        Task<HttpResponseMessage> GetAttachmentPdf(int absEntry, string fileName, string fileExt, string sapSystem);
    }
}
