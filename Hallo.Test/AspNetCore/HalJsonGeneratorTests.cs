using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.AspNetCore;
using Hallo.Test.Serialization.Supporting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hallo.Test.AspNetCore
{
    public class HalJsonGeneratorTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        [Fact]
        public async Task ProducesHalJsonDocument()
        {
            _services.AddTransient<Hal<DummyModel>, FullRepresentation>();
            
            var httpContext = CreateHttpContext();
            await HalJsonGenerator.HalHandler(httpContext, new DummyModel
            {
                Id = 123,
                Property = "Test"
            });

            httpContext.Response.ContentType.Should().Be("application/hal+json");

            var rawJson = await ReadHttpResponseBody(httpContext.Response);
            var json = JsonDocument.Parse(rawJson).RootElement;

            json.TryGetProperty("_links", out _).Should().BeTrue();
            json.TryGetProperty("_embedded", out _).Should().BeTrue();
        }
        
        [Fact]
        public async Task ProducesJsonDocumentWhenHalRepresentationIsNotAvailable()
        {
            var httpContext = CreateHttpContext();
            await HalJsonGenerator.HalHandler(httpContext, new DummyModel
            {
                Id = 123,
                Property = "Test"
            });

            httpContext.Response.ContentType.Should().Be("application/json");
            
            var rawJson = await ReadHttpResponseBody(httpContext.Response);
            rawJson.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UsesJsonSerializerOptionsFromRequestServices()
        {
            // JsonSerializerOptions uses PascalCase by default, Hallo default is camelCase
            _services.AddSingleton(new JsonSerializerOptions());
            
            var httpContext = CreateHttpContext();
            await HalJsonGenerator.HalHandler(httpContext, new DummyModel
            {
                Id = 123,
                Property = "Test"
            });

            var rawJson = await ReadHttpResponseBody(httpContext.Response);
            var json = JsonDocument.Parse(rawJson).RootElement;

            json.TryGetProperty("Id", out _).Should().BeTrue();
            json.TryGetProperty("Property", out _).Should().BeTrue();
        }
        
        private DefaultHttpContext CreateHttpContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _services.BuildServiceProvider()
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