using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hallo.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hallo.Test.Serialization.Supporting
{
    internal static class TestResponseContentFormatter
    {
        public static async Task<HttpResponse> GetResponse<T>(T resource, IServiceCollection services, JsonSerializerOptions jsonSerializerOptions = null)
        {
            var httpContext = CreateHttpContext(services);
            var writeContext = new OutputFormatterWriteContext(httpContext, (stream, _) => new StreamWriter(stream), 
                                                               typeof(T), resource);

            var formatter = jsonSerializerOptions != null ? new HalJsonOutputFormatter(jsonSerializerOptions) : new HalJsonOutputFormatter();
            await formatter.WriteAsync(writeContext);

            return httpContext.Response;
        }
        
        public static async Task<string> FormatRaw<T>(T resource, IServiceCollection services, JsonSerializerOptions jsonSerializerOptions = null)
        {
            var response = await GetResponse(resource, services, jsonSerializerOptions);
            var body = await ReadHttpResponseBody(response);
            return body;
        }
        
        public static async Task<JObject> Format<T>(T resource, IServiceCollection services, JsonSerializerOptions jsonSerializerOptions = null)
        {
            var body = await FormatRaw(resource, services, jsonSerializerOptions);
            return JsonConvert.DeserializeObject<JObject>(body);
        }

        private static DefaultHttpContext CreateHttpContext(IServiceCollection services)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider()
            };

            httpContext.Response.Body = new MemoryStream();
            return httpContext;
        }
        
        private static async Task<string> ReadHttpResponseBody(HttpResponse response)
        {
            response.Body.Seek(0L, SeekOrigin.Begin);
            using var reader = new StreamReader(response.Body, Encoding.UTF8);
            
            return await reader.ReadToEndAsync();
        }
    }
}