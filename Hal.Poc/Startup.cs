using Hal.Poc.Data;
using Hal.Poc.Hypermedia;
using Hal.Poc.Hypermedia.Serialization;
using Hal.Poc.Models;
using Hal.Poc.Models.Hypermedia;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hal.Poc
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
            services.AddMvc(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.OutputFormatters.Add(new JsonHalOutputFormatter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddSingleton<PeopleRepository>();
            services.AddTransient<Hal<Person>, PersonRepresentation>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}