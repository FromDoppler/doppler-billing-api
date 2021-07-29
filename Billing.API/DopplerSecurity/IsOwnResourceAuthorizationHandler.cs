using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Billing.API.DopplerSecurity
{
    public class IsOwnResourceAuthorizationHandler : AuthorizationHandler<DopplerAuthorizationRequirement>
    {
        private readonly ILogger<IsOwnResourceAuthorizationHandler> _logger;

        public IsOwnResourceAuthorizationHandler(ILogger<IsOwnResourceAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DopplerAuthorizationRequirement requirement)
        {
            if (requirement.AllowOwnResource && IsOwnResource(context))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool IsOwnResource(AuthorizationHandlerContext context)
        {
            var tokenUserId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!TryGetRouteData(context, out var routeData))
            {
                _logger.LogWarning("Is not possible access to Resource information. Type of context.Resource: {ResourceType}", context.Resource?.GetType().Name ?? "null");
                return false;
            }

            if (!routeData.Values.TryGetValue("clientId", out var clientId) || clientId?.ToString() != tokenUserId)
            {
                _logger.LogWarning("The IdUser into the token is different that in the route. The user hasn't permissions.");
                return false;
            }
            // TODO: check token Issuer information, to validate right origin

            return true;
        }

        private static bool TryGetRouteData(AuthorizationHandlerContext context, out RouteData routeData)
        {
            // In my local environment with .NET 5
            if (context.Resource is HttpContext httpContext)
            {
                routeData = httpContext.GetRouteData();
                return true;
            }

            // ASP.NET Core 2?
            if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
            {
                routeData = authorizationFilterContext.RouteData;
                return true;
            }

            routeData = null;
            return false;
        }
    }
}
