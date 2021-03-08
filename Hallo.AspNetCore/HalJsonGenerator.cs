using System;
using System.Text.Json;
using System.Threading.Tasks;
using Hallo.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hallo.AspNetCore
{
    /// <summary>
    /// A utility for serializing HAL+JSON to the <see cref="HttpResponse"/>
    /// </summary>
    public static class HalJsonGenerator
    {
        /// <summary>
        /// Serializes the provided resource to the <see cref="HttpResponse"/> stream.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method will try to resolve an instance of <see cref="JsonSerializerOptions"/>
        /// from the <see cref="IServiceProvider"/>. If no instance is found then
        /// <see cref="HalJsonSerializer.DefaultSerializerOptions"/> will be used.
        /// </p>
        /// <p>
        /// When no HAL document generation has been implemented for the resource
        /// type being serialized it will fall back to basic JSON serialization using
        /// the configured <see cref="JsonSerializerOptions"/>
        /// </p>
        /// </remarks>
        /// <param name="context">The HttpContext to write the serialized object to</param>
        /// <param name="resource">The object to serialize</param>
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