using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Serialization;
using Hallo.Test.Serialization.Supporting;
using Xunit;

namespace Hallo.Test.Serialization
{
    public class DefaultJsonSerializationOptionsTests
    {
        [Fact]
        public async Task UsesRelaxedCharacterEncoding()
        {
            var json = await HalJsonSerializer.SerializeAsync(new DefaultRepresentation(), new DummyModel
            {
                Property = "+++"
            }, HalJsonSerializer.DefaultSerializerOptions);

            json.Should().Contain("+++", because: "the 'plus' unicode character should not be escaped");
        }

        [Fact]
        public async Task UsesCamelCasePropertyNaming()
        {
            var rawJson = await HalJsonSerializer.SerializeAsync(new DefaultRepresentation(), new DummyModel
            {
                Id = 123,
                Property = "+++"
            }, HalJsonSerializer.DefaultSerializerOptions);

            var json = JsonDocument.Parse(rawJson).RootElement;
            json.TryGetProperty("Id", out _).Should().BeFalse();
            json.TryGetProperty("id", out _).Should().BeTrue();
            json.TryGetProperty("Property", out _).Should().BeFalse();
            json.TryGetProperty("property", out _).Should().BeTrue();
        }

        [Fact]
        public void IncludesHalConverters()
        {
            var converters = HalJsonSerializer.DefaultSerializerOptions.Converters;

            converters.Any(x => x.GetType() == typeof(HalRepresentationConverter)).Should().BeTrue();
            converters.Any(x => x.GetType() == typeof(LinksConverter)).Should().BeTrue();
        }
    }
}