using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Hallo.Test.AspNetCore.Mvc.Supporting;
using Hallo.Test.AspNetCore.Supporting.Api;
using Xunit;

namespace Hallo.Test.AspNetCore.Mvc
{
    public class MvcApiTests : ApiTests<MvcStartup>, 
                               IClassFixture<WebApplicationFactory<MvcStartup>>
    {
        public MvcApiTests(WebApplicationFactory<MvcStartup> factory)
            : base(factory) { }

        [Theory]
        [InlineData("application/hal+json", "application/hal+json")]
        [InlineData(null, "application/json")]
        [InlineData("application/json", "application/json")]
        public async Task NegotiatesContentType(string accept, string expectedContentType)
        {
            var client = Factory.CreateClient();
            if (!string.IsNullOrWhiteSpace(accept))
            {
                client.DefaultRequestHeaders.Add("Accept", accept);
            }
            
            var response = await client.GetAsync("/people/123");
            response.IsSuccessStatusCode.Should().BeTrue();
            response.Content.Headers.ContentType.MediaType.Should().Be(expectedContentType);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task ProducesJsonDocumentWhenNegotiated()
        {
            var client = Factory.CreateClient();
            var response = await client.GetAsync("/people/123");
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content).RootElement;

            json.GetProperty("id").GetInt32().Should().Be(123);
            json.GetProperty("firstName").GetString().Should().Be("Test");
            json.GetProperty("lastName").GetString().Should().Be("User");
        }
    }
}