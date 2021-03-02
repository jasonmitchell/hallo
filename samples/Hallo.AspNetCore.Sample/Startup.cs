using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Hallo.AspNetCore.Sample.Data;
using Hallo.AspNetCore.Sample.Models;
using Hallo.AspNetCore.Sample.Models.Hypermedia;
using Hallo.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hallo.AspNetCore.Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<PeopleRepository>();

            services.AddTransient<PersonRepresentation>();
            services.AddTransient<AddressRepresentation>();

            services.AddTransient<Hal<Person>, PersonRepresentation>();
            services.AddTransient<Hal<Address>, AddressRepresentation>();

            services.AddTransient<Hal<PagedList<Person>>, PersonListRepresentation>();
            services.AddTransient<Hal<PagedList<Address>>, AddressListRepresentation>();

            services.AddHttpContextAccessor();

            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.Converters.Add(new HalRepresentationConverter());
            serializerOptions.Converters.Add(new LinksConverter());
            serializerOptions.PropertyNameCaseInsensitive = true;
            serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            services.AddSingleton(serializerOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/people", async context =>
                {
                    var peopleRepo = context.RequestServices.GetRequiredService<PeopleRepository>();
                    var model = peopleRepo.List(new Paging());
                    await HalJsonGenerator.HalHandler(context, model);
                });

                endpoints.MapGet("/people/{id:int}", async context =>
                {
                    var peopleRepo = context.RequestServices.GetRequiredService<PeopleRepository>();
                    var personId = int.Parse(context.Request.RouteValues["id"].ToString());
                    var model = peopleRepo.Get(personId);
                    await HalJsonGenerator.HalHandler(context, model);
                });

                endpoints.MapGet("/people/{id:int}/addresses", async context =>
                {
                    var peopleRepo = context.RequestServices.GetRequiredService<PeopleRepository>();
                    var personId = int.Parse(context.Request.RouteValues["id"].ToString());
                    var person = peopleRepo.Get(personId);
                    var addresses = person.Addresses;
                    var paging = new Paging();
                    var model = new PagedList<Address>
                    {
                        CurrentPage = paging.Page,
                        TotalItems = addresses.Length,
                        TotalPages = (int) Math.Ceiling(addresses.Length / (double) paging.PageSize),
                        Items = addresses
                    };
                    await HalJsonGenerator.HalHandler(context, model);
                });

                endpoints.MapGet("/people/{id:int}/addresses/{addressid:int}", async context =>
                {
                    var peopleRepo = context.RequestServices.GetRequiredService<PeopleRepository>();
                    var personId = int.Parse(context.Request.RouteValues["id"].ToString());
                    var person = peopleRepo.Get(personId);
                    var addressId = int.Parse(context.Request.RouteValues["addressid"].ToString());
                    var model = person.Addresses.FirstOrDefault(x => x.Id == addressId);

                    await HalJsonGenerator.HalHandler(context, model);
                });
            });
        }
    }
}