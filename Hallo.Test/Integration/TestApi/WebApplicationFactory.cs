using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Hallo.Test.Integration.TestApi
{
    public class WebApplicationFactory : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();
        }

        protected override void ConfigureClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/hal+json");
            base.ConfigureClient(client);
        }
    }
}