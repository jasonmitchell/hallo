using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Test.AspNetCore.Mvc.Supporting;
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
            var json = JsonDocument.Parse(content).RootElement;

            json.GetProperty("id").GetInt32().Should().Be(123);
            json.GetProperty("firstName").GetString().Should().Be("Test");
            json.GetProperty("lastName").GetString().Should().Be("User");

            var links = json.GetProperty("_links");
            links.Should().NotBeNull();
            var self = links.GetProperty("self");
            self.Should().NotBeNull();
            self.GetProperty("href").GetString().Should().Be("/people/123");
            self.GetProperty("title").GetString().Should().Be("A Title");
            self.GetProperty("name").GetString().Should().Be("a name");
            self.GetProperty("profile").GetString().Should().Be("http://example.com/profile");
            self.GetProperty("deprecation").GetString().Should().Be("http://example.com/deprecated");
            self.GetProperty("type").GetString().Should().Be("application/hal+json");
            self.GetProperty("hreflang").GetString().Should().Be("en-IE");

            var embedded = json.GetProperty("_embedded");
            embedded.Should().NotBeNull();
            embedded.GetProperty("contacts").GetArrayLength().Should().BePositive();
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