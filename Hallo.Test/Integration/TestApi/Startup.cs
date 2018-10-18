using Hallo.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Hallo.Test.Integration.TestApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.OutputFormatters.Add(new JsonHalOutputFormatter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<ContactLookup>();
            services.AddTransient<Hal<Person>, PersonRepresentation>();
            services.AddTransient<Hal<PagedList<Person>>, PersonListRepresentation>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}