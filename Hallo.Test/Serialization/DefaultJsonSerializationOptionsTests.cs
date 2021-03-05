using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Serialization;
using Hallo.Test.Serialization.Supporting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hallo.Test.Serialization
{
    public class DefaultJsonSerializationOptionsTests
    {
        private readonly ServiceCollection _services = new ServiceCollection();

        [Fact]
        public async Task UsesRelaxedCharacterEncoding()
        {
            var json = await HalJsonSerializer.SerializeAsync(new DefaultRepresentation(), new DummyModel
            {
                Property = "+++"
            }, HalJsonSerializer.DefaultSerializerOptions);

            json.Should().Contain("+++", because: "the 'plus' unicode character should not be escaped");
        }
    }
}