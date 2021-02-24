using System.Threading.Tasks;
using FluentAssertions;
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
            _services.AddTransient<Hal<DummyModel>, DefaultRepresentation>();

            var json = await TestResponseContentFormatter.FormatRaw(new DummyModel
            {
                Property = "+++"
            }, _services);

            json.Should().Contain("+++", because: "the 'plus' unicode character should not be escaped");
        }
    }
}