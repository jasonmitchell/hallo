using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Test.AspNetCore.Mvc.Supporting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hallo.Test.AspNetCore.Mvc
{
    public class ContentNegotiationTests : IClassFixture<WebApplicationFactory>
    {
        private readonly WebApplicationFactory _factory;

        public ContentNegotiationTests(WebApplicationFactory factory)
        {
            _factory = factory;
        }
        
        [Fact]
        public async Task ProducesHalDocument()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/people/123");

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<JObject>(content);

            json.Value<int>("id").Should().Be(123);
            json.Value<string>("firstName").Should().Be("Test");
            json.Value<string>("lastName").Should().Be("User");
            
            var links = json.Value<JObject>("_links");
            links.Should().NotBeNull();
            var self = links["self"];
            self.Should().NotBeNull();
            self["href"].Value<string>().Should().Be("/people/123");
            self["title"].Value<string>().Should().Be("A Title");
            self["name"].Value<string>().Should().Be("a name");
            self["profile"].Value<string>().Should().Be("http://example.com/profile");
            self["deprecation"].Value<string>().Should().Be("http://example.com/deprecated");
            self["type"].Value<string>().Should().Be("application/hal+json");
            self["hreflang"].Value<string>().Should().Be("en-IE");

            
            var embedded = json.Value<JObject>("_embedded");
            embedded.Should().NotBeNull();
            embedded["contacts"].Should().NotBeNull();
        }

        [Fact]
        public async Task NegotiatesContentType()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/people/123");
            
            response.Content.Headers.ContentType.MediaType.Should().Be("application/hal+json");
        }
    }
}