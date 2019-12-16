using Hallo.Sample.Data;
using Hallo.Sample.Models;
using Hallo.Sample.Models.Hypermedia;
using Hallo.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hallo.Sample
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
            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.OutputFormatters.Add(new HalJsonOutputFormatter());
            });

            services.AddSingleton<PeopleRepository>();
            
            services.AddTransient<PersonRepresentation>();
            services.AddTransient<Hal<Person>, PersonRepresentation>();
            services.AddTransient<Hal<PagedList<Person>>, PersonListRepresentation>();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
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
            
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}