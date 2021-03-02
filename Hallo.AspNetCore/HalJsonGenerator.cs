using System.Text.Json;
using System.Threading.Tasks;
using Hallo.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hallo.AspNetCore
{
    public static class HalJsonGenerator
    {
        public static async Task HalHandler(HttpContext context, object resource)
        {
            var services = context.RequestServices;
            var serializerOptions = context.RequestServices.GetService<JsonSerializerOptions>() ?? HalJsonSerializer.DefaultSerializerOptions;
            var representationGenerator = services.GetRepresentationGenerator(resource.GetType());
            
            if (representationGenerator == null)
            {
                context.Response.ContentType = "application/json";
                
                await JsonSerializer.SerializeAsync(context.Response.Body, resource, resource.GetType(), serializerOptions);
                await context.Response.Body.FlushAsync();
                
                return;
            }

            context.Response.ContentType = "application/hal+json";

            var json = await HalJsonSerializer.SerializeAsync(representationGenerator, resource, serializerOptions);
            await context.Response.WriteAsync(json);
            await context.Response.Body.FlushAsync();
        }
    }
}