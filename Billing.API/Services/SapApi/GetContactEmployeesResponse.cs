using System.Collections.Generic;

namespace Billing.API.Services.SapApi
{
    public class GetContactEmployeesResponse
    {
        public IList<SapContactEmployee> ContactEmployees { get; set; }
    }
}
