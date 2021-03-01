using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Test.Serialization.Supporting;
using Microsoft.Extensions.DependencyInjection;
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

            json.GetProperty("id").GetInt32().Should().Be(123);
            json.GetProperty("property").GetString().Should().Be("test");
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

            json.TryGetProperty("property", out _).Should().BeTrue();
            json.TryGetProperty("id", out _).Should().BeFalse();
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

            var links = json.GetProperty("_links");
            links.GetProperty("self").GetProperty("href").GetString().Should().Be("/dummy-model/123");
            links.GetProperty("another-resource").GetProperty("href").GetString().Should().Be("/dummy-model/another-resource");
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
            
            var links = json.GetProperty("_links");
            links.TryGetProperty("self", out var self).Should().BeTrue();

            self.GetArrayLength().Should().Be(2);
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

            json.TryGetProperty("_links", out _).Should().BeFalse();
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

            json.TryGetProperty("_links", out _).Should().BeFalse();
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

            var embedded = json.GetProperty("_embedded");
            embedded.TryGetProperty("magicNumber", out var magicNumber).Should().BeTrue();
            magicNumber.GetInt32().Should().Be(3);
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

            json.TryGetProperty("_embedded", out _).Should().BeFalse();
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

            var embedded = json.GetProperty("_embedded");
            embedded.Should().NotBeNull();

            embedded.TryGetProperty("items", out var items).Should().BeTrue();
            items.GetArrayLength().Should().BeGreaterThan(0);
            items[0].TryGetProperty("_links", out _).Should().BeTrue();
            items[0].TryGetProperty("_embedded", out _).Should().BeFalse();
            items[0].TryGetProperty("id", out _).Should().BeTrue();
        }

        [Fact]
        public async Task OutputsStandardJsonWhenHalRepresentationIsMissing()
        {
            var json = await TestResponseContentFormatter.Format(new DummyModel
            {
                Id = 1, 
                Property = "test"
            }, _services, _jsonSerializerOptions);

            json.GetProperty("id").GetInt32().Should().Be(1);
            json.GetProperty("property").GetString().Should().Be("test");
            json.TryGetProperty("_links", out _).Should().BeFalse();
            json.TryGetProperty("_embedded", out _).Should().BeFalse();
        }

        [Fact]
        public async Task SetsJsonContentTypeWhenHalRepresentationIsMissing()
        {
            var response = await TestResponseContentFormatter.GetResponse(new DummyModel
            {
                Id = 1,
                Property = "test"
            }, _services, _jsonSerializerOptions);

            response.ContentType.Should().Be("application/json");
        }
    }
}