using Microsoft.AspNetCore.TestHost;
using tree_api.Tests.ClientGenerator.Utils;

namespace tree_api.Specs.Contexts;

internal class TestContext
{
    public TestServer Server;

    public TestContext()
    {
        Server = SetupContext();
    }

    private TestServer SetupContext()
    {
#if DEBUG
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Local");
#endif

        Server = new CustomWebApplicationFactory()
            .WithWebHostBuilder(builder =>
            {
            })

        return Server;
    }
}

