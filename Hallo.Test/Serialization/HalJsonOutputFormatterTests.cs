using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Test.Serialization.Supporting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hallo.Test.Serialization
{
    public class HalJsonOutputFormatterTests
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        private readonly ServiceCollection _services = new ServiceCollection();

        [Fact]
        public async Task SerializesDefaultState()
        {
            _services.AddTransient<Hal<DummyModel>, DefaultRepresentation>();

            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.Should().ContainKeys("id", "property");
            json["id"].Value<int>().Should().Be(123);
            json["property"].Value<string>().Should().Be("test");
        }

        [Fact]
        public async Task SerializesModifiedState()
        {
            _services.AddTransient<Hal<DummyModel>, ModifiedStateRepresentation>();

            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.Should().ContainKey("property");
            json.Should().NotContainKey("id");
        }


        [Fact]
        public async Task SerializesLinks()
        {
            _services.AddTransient<Hal<DummyModel>, LinkedRepresentation>();

            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.Should().ContainKeys("id", "property", "_links");
            
            var links = json.Value<JObject>("_links");
            links.Should().ContainKeys("self", "another-resource");
            links["self"]["href"].Value<string>().Should().Be("/dummy-model/123");
            links["another-resource"]["href"].Value<string>().Should().Be("/dummy-model/another-resource");
        }

        [Fact]
        public async Task SupportsMultipleLinksPerRelation()
        {
            _services.AddTransient<Hal<DummyModel>, MultiLinkRelationRepresentation>();

            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            }, _services, _jsonSerializerOptions);
            
            json.Should().ContainKeys("id", "property", "_links");
            var links = json.Value<JObject>("_links");
            links.Should().ContainKeys("self");

            links["self"].As<JArray>().Should().HaveCount(2);
        }
        
        [Fact]
        public async Task DoesNotOutputLinksWhenEmptyLinksCollectionGenerated()
        {
            _services.AddTransient<Hal<DummyModel>, EmptyLinksRepresentation>();
            
            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.Should().NotContainKey("_links");
        }
        
        [Fact]
        public async Task DoesNotOutputLinksWhenNoGeneratorIsImplemented()
        {
            _services.AddTransient<Hal<DummyModel>, DefaultRepresentation>();
            
            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.Should().NotContainKey("_links");
        }
        
        [Fact]
        public async Task SerializesEmbeddedResources()
        {
            _services.AddTransient<Hal<DummyModel>, EmbeddedRepresentation>();

            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.Should().ContainKeys("id", "property", "_embedded");
            
            var embedded = json.Value<JObject>("_embedded");
            embedded.Should().ContainKey("magicNumber");
            embedded["magicNumber"].Value<int>().Should().Be(3);
        }
        
        [Fact]
        public async Task DoesNotOutputEmbeddedResourcesWhenNoneAreGenerated()
        {
            _services.AddTransient<Hal<DummyModel>, DefaultRepresentation>();
            
            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 123,
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.Should().NotContainKey("_embedded");
        }

        [Fact]
        public async Task SerializesRecursiveNestedRepresentations()
        {
            _services.AddTransient<IHalLinks<DummyModel>, LinkedRepresentation>();
            _services.AddTransient<Hal<PagedList<DummyModel>>, DummyModelListRepresentation>();

            var json = await TestResponseContentFormatter.Format(new PagedList<DummyModel>
            {
                CurrentPage = 1,
                TotalItems = 2,
                TotalPages = 1,
                Items = new[]
                {
                    new DummyModel { Id = 123, Property = "Test" },
                    new DummyModel { Id = 321, Property = "tseT" }
                }
            }, _services, _jsonSerializerOptions);

            var embedded = json.Value<JObject>("_embedded");
            embedded.Should().NotBeNull();

            var items = embedded["items"].As<JArray>();
            items.Should().NotBeNull();
            items.Count.Should().BeGreaterThan(0);
            items[0]["_links"].Should().NotBeNull();
            items[0]["_embedded"].Should().BeNull();
            items[0]["id"].Should().NotBeNull();
        }

        [Fact]
        public async Task OutputsStandardJsonWhenHalRepresentationIsMissing()
        {
            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 1, 
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.Should().ContainKeys("id", "property");
            json.Value<int>("id").Should().Be(1);
            json.Value<string>("property").Should().Be("test");
            json.Should().NotContainKeys("_links", "_embedded");
        }
    }
}