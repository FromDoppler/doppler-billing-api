using Billing.API.DopplerSecurity;
using Billing.API.Services.SapApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Billing.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SapConfig>(Configuration.GetSection(nameof(SapConfig)));
            services.AddDopplerSecurity();
            services.AddHttpClient("", c => { })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    UseCookies = false
                });
            services.AddInvoiceService();

            services.AddTransient<CryptoHelper>();

            services.AddCors();
            // TODO: configure JSON to indent the results
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            // Removed the condition IsDevelopment so the errors are thrown directly to the client
            // and it is easier to debug, consider restoring the old behavior in the future.
            app.UseDeveloperExceptionPage();

            app.UseCors(policy => policy
                .SetIsOriginAllowed(isOriginAllowed: _ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
