using System;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Serialization;
using Hallo.Test.Serialization.Supporting;
using Xunit;

namespace Hallo.Test.Serialization
{
    public class HalJsonSerializerTests
    {
        private static async Task<JsonElement> Serialize(IHal representationGenerator, object resource)
        {
            var json = await HalJsonSerializer.SerializeAsync(representationGenerator, resource, HalJsonSerializer.DefaultSerializerOptions);
            return JsonDocument.Parse(json).RootElement;
        }
        
        [Fact]
        public async Task SerializesDefaultState()
        {
            var json = await Serialize(new DefaultRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.GetProperty("id").GetInt32().Should().Be(123);
            json.GetProperty("property").GetString().Should().Be("test");
        }
        
        [Fact]
        public async Task SerializesModifiedState()
        {
            var json = await Serialize(new ModifiedStateRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.TryGetProperty("property", out _).Should().BeTrue();
            json.TryGetProperty("id", out _).Should().BeFalse();
        }
        
        [Fact]
        public async Task SerializesLinks()
        {
            var json = await Serialize(new LinkedRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            var links = json.GetProperty("_links");
            links.GetProperty("self").GetProperty("href").GetString().Should().Be("/dummy-model/123");
            links.GetProperty("another-resource").GetProperty("href").GetString().Should().Be("/dummy-model/another-resource");
        }
        
        [Fact]
        public async Task SupportsMultipleLinksPerRelation()
        {
            var json = await Serialize(new MultiLinkRelationRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "test"
            });
            
            var links = json.GetProperty("_links");
            links.TryGetProperty("self", out var self).Should().BeTrue();

            self.GetArrayLength().Should().Be(2);
        }
        
        [Fact]
        public async Task DoesNotOutputLinksWhenEmptyLinksCollectionGenerated()
        {
            var json = await Serialize(new EmptyLinksRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.TryGetProperty("_links", out _).Should().BeFalse();
        }
        
        [Fact]
        public async Task DoesNotOutputLinksWhenNoGeneratorIsImplemented()
        {
            var json = await Serialize(new DefaultRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.TryGetProperty("_links", out _).Should().BeFalse();
        }
        
        [Fact]
        public async Task SerializesEmbeddedResources()
        {
            var json = await Serialize(new EmbeddedRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            var embedded = json.GetProperty("_embedded");
            embedded.TryGetProperty("magicNumber", out var magicNumber).Should().BeTrue();
            magicNumber.GetInt32().Should().Be(3);
        }
        
        [Fact]
        public async Task DoesNotOutputEmbeddedResourcesWhenNoneAreGenerated()
        {
            var json = await Serialize(new DefaultRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "test"
            });

            json.TryGetProperty("_embedded", out _).Should().BeFalse();
        }
        
        [Fact]
        public async Task SerializesRecursiveNestedRepresentations()
        {
            var json = await Serialize(new DummyModelListRepresentation(new LinkedRepresentation()), new PagedList<DummyModel>
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

            var embedded = json.GetProperty("_embedded");
            embedded.Should().NotBeNull();

            embedded.TryGetProperty("items", out var items).Should().BeTrue();
            items.GetArrayLength().Should().BeGreaterThan(0);
            items[0].TryGetProperty("_links", out _).Should().BeTrue();
            items[0].TryGetProperty("_embedded", out _).Should().BeFalse();
            items[0].TryGetProperty("id", out _).Should().BeTrue();
        }

        [Fact]
        public void GuardsAgainstNulls()
        {
            Func<Task<string>> nullGeneratorAction = async () => await HalJsonSerializer.SerializeAsync(null!, new DummyModel(), HalJsonSerializer.DefaultSerializerOptions);
            Func<Task<string>> nullResourceAction = async () => await HalJsonSerializer.SerializeAsync(new DefaultRepresentation(), null!, HalJsonSerializer.DefaultSerializerOptions);
            Func<Task<string>> nullJsonSerializerOptionsAction = async () => await HalJsonSerializer.SerializeAsync(new DefaultRepresentation(), new DummyModel(), null!);

            nullGeneratorAction.Should().Throw<ArgumentNullException>();
            nullResourceAction.Should().Throw<ArgumentNullException>();
            nullJsonSerializerOptionsAction.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void RequiresHalConvertersInJsonSerializerOptions()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            Func<Task<string>> action = async () => await HalJsonSerializer.SerializeAsync(new DefaultRepresentation(), new DummyModel(), jsonSerializerOptions);

            action.Should().Throw<InvalidJsonSerializerOptionsException>();
        }
    }
}