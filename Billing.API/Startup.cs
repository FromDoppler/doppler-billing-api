using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing.API.DopplerSecurity;
using Billing.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            services.AddDopplerSecurity();
            services.AddInvoiceService();
            // TODO: move this line inside AddDopplerSecurity
            services.AddSingleton<IAuthorizationHandler, IsSuperUserHandler>();
            services.AddCors();
            //services.AddControllers()
            //    .AddJsonOptions(options =>
            //    {
            //        options.JsonSerializerOptions.WriteIndented = true;
            //        options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
            //        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //    });
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

            app.UseMvc();
        }
    }
}
