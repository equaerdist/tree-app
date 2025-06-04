using Microsoft.AspNetCore.TestHost;
using tree_api.Tests.ClientGenerator.Utils;

namespace tree_api.Specs.Contexts;

internal sealed class TestContext : IDisposable
{
    public TestServer Server;

    public TestContext() => Server = SetupContext();

    public void Dispose() => Server.Dispose();

    private TestServer SetupContext()
    {
#if DEBUG
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Local");
#endif

        Server = new CustomWebApplicationFactory()
            .WithWebHostBuilder(_ =>
            {
            }).Server;

        return Server;
    }
}

