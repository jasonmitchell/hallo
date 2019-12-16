using System.Text.Json;
using Hallo.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hallo.Test.Integration.TestApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                var jsonOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                options.OutputFormatters.Add(new HalJsonOutputFormatter(jsonOptions));
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