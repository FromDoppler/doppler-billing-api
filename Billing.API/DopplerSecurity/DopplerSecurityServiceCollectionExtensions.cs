using Billing.API.DopplerSecurity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DopplerSecurityServiceCollectionExtensions
    {
        public static IServiceCollection AddDopplerSecurity(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, IsOwnResourceAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, IsSuperUserAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, IsSignedPathAuthorizationHandler>();

            services.ConfigureOptions<ConfigureDopplerSecurityOptions>();

            services
                .AddOptions<AuthorizationOptions>()
                .Configure(o =>
                {
                    o.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .AddRequirements(new DopplerAuthorizationRequirement
                        {
                            AllowSuperUser = true,
                            AllowOwnResource = true
                        })
                        .RequireAuthenticatedUser()
                        .Build();

                    o.AddPolicy(DopplerSecurityDefaults.DEFAULT_OR_SIGNED_PATHS_POLICY, new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme) //TODO: Check which scheme to use
                        .AddRequirements(new DopplerAuthorizationRequirement
                        {
                            AllowSuperUser = true,
                            AllowOwnResource = true,
                            AllowSignedPaths = true
                        })
                        .Build());
                });

            services
                .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<DopplerSecurityOptions>>((o, securityOptions) =>
                {
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKeys = securityOptions.Value.SigningKeys,
                        ValidateIssuer = false,
                        ValidateLifetime = !securityOptions.Value.SkipLifetimeValidation,
                        ValidateAudience = false,
                    };
                });

            services.AddAuthentication().AddJwtBearer();

            services.AddAuthorization();

            return services;
        }
    }
}
