using Hallo.AspNetCore.Mvc;
using Hallo.Test.AspNetCore.Supporting.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hallo.Test.AspNetCore.Mvc.Supporting
{
    public class MvcStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.OutputFormatters.Add(new HalJsonOutputFormatter());
            });

            services.AddTransient<ContactLookup>();
            services.AddTransient<PersonRepresentation>();
            services.AddTransient<Hal<Person>, PersonRepresentation>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}