using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hallo.AspNetCore
{
    public static class HalJsonGenerator
    {
        public static async Task HalHandler(HttpContext context, Object halObject)
        {
            var json = await GenerateHalJson(context, halObject);
            if (json == null)
            {
                var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();
                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.Body, halObject, halObject.GetType(),
                    serializerOptions);
                await context.Response.Body.FlushAsync();
            }
            context.Response.ContentType = "application/hal+json";
            await context.Response.WriteAsync(json);
        }

        public static async Task<string> GenerateHalJson(HttpContext context, Object halObject,
            JsonSerializerOptions serializerOptions = null)
        {
            var representationGenerator = GetRepresentationGenerator(context.RequestServices, halObject.GetType());
            if (representationGenerator == null)
            {
                return null;
            }

            serializerOptions ??= context.RequestServices.GetRequiredService<JsonSerializerOptions>();
            var representation = await representationGenerator.RepresentationOfAsync(halObject);
            var json = JsonSerializer.Serialize(representation, serializerOptions);
            return json;
        }

        private static IHal GetRepresentationGenerator(IServiceProvider services, Type resourceType)
        {
            var representationType = typeof(Hal<>).MakeGenericType(resourceType);
            var representation = (IHal) services.GetService(representationType);
            return representation;
        }
    }
}