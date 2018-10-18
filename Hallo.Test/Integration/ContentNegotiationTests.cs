using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Test.Integration.TestApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hallo.Test.Integration
{
    public class ContentNegotiationTests : IClassFixture<WebApplicationFactory>
    {
        private readonly WebApplicationFactory _factory;

        public ContentNegotiationTests(WebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task NegotiatesContentType()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/people/123");
            
            response.Content.Headers.ContentType.MediaType.Should().Be("application/hal+json");
        }

        [Fact]
        public async Task SerializesState()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/people/123");

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<JObject>(content);

            json.Value<int>("id").Should().Be(123);
            json.Value<string>("firstName").Should().Be("Test");
            json.Value<string>("lastName").Should().Be("User");
        }

        [Fact]
        public async Task SerializesLinks()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/people/123");

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<JObject>(content);

            var links = json.Value<JObject>("_links");
            links.Should().NotBeNull();
            links["self"].Should().NotBeNull();
            links["self"]["href"].Value<string>().Should().Be("/people/123");
        }

        [Fact]
        public async Task SerializesEmbeddedState()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/people/123");

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<JObject>(content);

            var embedded = json.Value<JObject>("_embedded");
            embedded.Should().NotBeNull();
            embedded["contacts"].Should().NotBeNull();
        }
    }
}