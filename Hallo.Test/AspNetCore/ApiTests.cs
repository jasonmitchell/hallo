using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Test.AspNetCore.Supporting.Api;
using Xunit;

namespace Hallo.Test.AspNetCore
{
    public abstract class ApiTests<TStartup> where TStartup : class
    {
        protected readonly WebApplicationFactory<TStartup> Factory;

        protected ApiTests(WebApplicationFactory<TStartup> factory)
        {
            Factory = factory;
        }
        
        [Fact]
        public async Task ProducesHalDocument()
        {
            var client = Factory.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", "application/hal+json");
            
            var response = await client.GetAsync("/people/123");
            response.Content.Headers.ContentType.MediaType.Should().Be("application/hal+json");
            
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content).RootElement;

            json.GetProperty("id").GetInt32().Should().Be(123);
            json.GetProperty("firstName").GetString().Should().Be("Test");
            json.GetProperty("lastName").GetString().Should().Be("User");

            json.TryGetProperty("_links", out var links).Should().BeTrue();
            links.TryGetProperty("self", out var self).Should().BeTrue();

            self.GetProperty("href").GetString().Should().Be("/people/123");
            self.GetProperty("title").GetString().Should().Be("A Title");
            self.GetProperty("name").GetString().Should().Be("a name");
            self.GetProperty("profile").GetString().Should().Be("http://example.com/profile");
            self.GetProperty("deprecation").GetString().Should().Be("http://example.com/deprecated");
            self.GetProperty("type").GetString().Should().Be("application/hal+json");
            self.GetProperty("hreflang").GetString().Should().Be("en-IE");

            json.TryGetProperty("_embedded", out var embedded).Should().BeTrue();            
            embedded.GetProperty("contacts").GetArrayLength().Should().BePositive();
        }

        [Fact]
        public async Task ProducesJsonDocumentWhenRepresentationIsMissing()
        {
            var client = Factory.CreateClient();
            var response = await client.GetAsync("/people/123/address");
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content).RootElement;

            json.GetProperty("firstLine").GetString().Should().NotBeEmpty();
            json.GetProperty("secondLine").GetString().Should().NotBeEmpty();

            json.TryGetProperty("_links", out _).Should().BeFalse();
            json.TryGetProperty("_embedded", out _).Should().BeFalse();
        }
    }
}