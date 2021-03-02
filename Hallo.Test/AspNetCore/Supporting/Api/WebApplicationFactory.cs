using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Hallo.Test.AspNetCore.Supporting.Api
{
    public class WebApplicationFactory<TStartup> : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<TStartup>();
        }
    }
}