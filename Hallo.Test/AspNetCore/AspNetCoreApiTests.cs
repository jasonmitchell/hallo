using Hallo.Test.AspNetCore.Supporting;
using Hallo.Test.AspNetCore.Supporting.Api;
using Xunit;

namespace Hallo.Test.AspNetCore
{
    public class AspNetCoreApiTests : ApiTests<AspNetCoreStartup>,
                                      IClassFixture<WebApplicationFactory<AspNetCoreStartup>>
    {
        public AspNetCoreApiTests(WebApplicationFactory<AspNetCoreStartup> factory)
            : base(factory)
        {
        }
    }
}