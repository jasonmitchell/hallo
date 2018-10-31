using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Serialization;
using Hallo.Test.Serialization.Supporting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hallo.Test.Serialization
{
    public class JsonHalOutputFormatterTests
    {
        private readonly ServiceCollection _services = new ServiceCollection();

        [Fact]
        public async Task SerializesDefaultState()
        {
            _services.AddTransient<Hal<DummyModel>, DefaultRepresentation>();

            var json = await Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.Should().ContainKeys("id", "property");
            json["id"].Value<int>().Should().Be(123);
            json["property"].Value<string>().Should().Be("test");
        }

        [Fact]
        public async Task SerializesModifiedState()
        {
            _services.AddTransient<Hal<DummyModel>, ModifiedStateRepresentation>();

            var json = await Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.Should().ContainKey("property");
            json.Should().NotContainKey("id");
        }


        [Fact]
        public async Task SerializesLinks()
        {
            _services.AddTransient<Hal<DummyModel>, LinkedRepresentation>();

            var json = await Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.Should().ContainKeys("id", "property", "_links");
            
            var links = json.Value<JObject>("_links");
            links.Should().ContainKeys("self", "another-resource");
            links["self"]["href"].Value<string>().Should().Be("/dummy-model/123");
            links["another-resource"]["href"].Value<string>().Should().Be("/dummy-model/another-resource");
        }
        
        [Fact]
        public async Task DoesNotOutputLinksWhenEmptyLinksCollectionGenerated()
        {
            _services.AddTransient<Hal<DummyModel>, EmptyLinksRepresentation>();
            
            var json = await Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.Should().NotContainKey("_links");
        }
        
        [Fact]
        public async Task DoesNotOutputLinksWhenNoGeneratorIsImplemented()
        {
            _services.AddTransient<Hal<DummyModel>, DefaultRepresentation>();
            
            var json = await Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.Should().NotContainKey("_links");
        }
        
        [Fact]
        public async Task SerializesEmbeddedResources()
        {
            _services.AddTransient<Hal<DummyModel>, EmbeddedRepresentation>();

            var json = await Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.Should().ContainKeys("id", "property", "_embedded");
            
            var embedded = json.Value<JObject>("_embedded");
            embedded.Should().ContainKey("magicNumber");
            embedded["magicNumber"].Value<int>().Should().Be(3);
        }
        
        [Fact]
        public async Task DoesNotOutputEmbeddedResourcesWhenNoneAreGenerated()
        {
            _services.AddTransient<Hal<DummyModel>, DefaultRepresentation>();
            
            var json = await Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.Should().NotContainKey("_embedded");
        }

        [Fact]
        public async Task SerializesRecursiveNestedRepresentations()
        {
            _services.AddTransient<IHalLinks<DummyModel>, LinkedRepresentation>();
            _services.AddTransient<Hal<PagedList<DummyModel>>, DummyModelListRepresentation>();

            var json = await Format(new PagedList<DummyModel>
            {
                CurrentPage = 1,
                TotalItems = 2,
                TotalPages = 1,
                Items = new[]
                {
                    new DummyModel { Id = 123, Property = "Test" },
                    new DummyModel { Id = 321, Property = "tseT" }
                }
            });

            var embedded = json.Value<JObject>("_embedded");
            embedded.Should().NotBeNull();

            var items = embedded["items"].As<JArray>();
            items.Should().NotBeNull();
            items.Count.Should().BeGreaterThan(0);
            items[0]["_links"].Should().NotBeNull();
            items[0]["_embedded"].Should().BeNull();
            items[0]["id"].Should().NotBeNull();
        }
        
        private async Task<JObject> Format<T>(T resource)
        {
            var httpContext = CreateHttpContext();
            var writeContext = new OutputFormatterWriteContext(httpContext, (stream, _) => new StreamWriter(stream), 
                                                              typeof(T), resource);

            var formatter = new JsonHalOutputFormatter();
            await formatter.WriteResponseBodyAsync(writeContext, Encoding.UTF8);

            var body = await ReadHttpResponseBody(httpContext);
            return JsonConvert.DeserializeObject<JObject>(body);
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
        
        private static async Task<string> ReadHttpResponseBody(HttpContext httpContext)
        {
            httpContext.Response.Body.Seek(0L, SeekOrigin.Begin);
            using (var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}