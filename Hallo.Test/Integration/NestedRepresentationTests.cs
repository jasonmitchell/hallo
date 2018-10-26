using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Test.Integration.TestApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hallo.Test.Integration
{
    public class NestedRepresentationTests : IClassFixture<WebApplicationFactory>
    {
        private readonly WebApplicationFactory _factory;

        public NestedRepresentationTests(WebApplicationFactory factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task Test()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/people");

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<JObject>(content);
            
            var embedded = json.Value<JObject>("_embedded");
            embedded.Should().NotBeNull();

            var items = embedded["items"].As<JArray>();
            items.Should().NotBeNull();
            items.Count.Should().BeGreaterThan(0);
            items[0]["_links"].Should().NotBeNull();
            items[0]["_embedded"].Should().BeNull();
            items[0]["id"].Should().NotBeNull();
        }
    }
}