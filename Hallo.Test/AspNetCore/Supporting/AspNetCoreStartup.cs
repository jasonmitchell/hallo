using Hallo.AspNetCore;
using Hallo.Serialization;
using Hallo.Test.AspNetCore.Supporting.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hallo.Test.AspNetCore.Supporting
{
    public class AspNetCoreStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ContactLookup>();
            services.AddTransient<PersonRepresentation>();
            services.AddTransient<Hal<Person>, PersonRepresentation>();
            
            services.AddSingleton(HalJsonSerializer.DefaultSerializerOptions);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/people/{id:int}", async context =>
                {
                    var personId = int.Parse(context.Request.RouteValues["id"].ToString()!);
                    await HalJsonGenerator.HalHandler(context, new Person
                    {
                        Id = personId,
                        FirstName = "Test",
                        LastName = "User"
                    });
                });
                
                endpoints.MapGet("/people/{id:int}/address", async context =>
                {
                    await HalJsonGenerator.HalHandler(context, new Address
                    {
                        FirstLine = "Some First Line",
                        SecondLine = "Some Second Line"
                    });
                });
            });
        }
    }
}