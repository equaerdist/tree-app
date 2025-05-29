using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using tree.api.Tests.ClientGenerator;
using tree_api.Tests.ClientGenerator.Utils;

namespace tree_api.Tests.Setup;

public class TestFixture
{
    private readonly TestServer _server;

    public readonly TreeAppClient TreeAppClient;

    public TestFixture()
    {
        var factory = new CustomWebApplicationFactory().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Local");
        });
        _server = factory.Server;

        var client = _server.CreateClient();
        TreeAppClient = new(client.BaseAddress!.ToString(), client);

        RunMigrations().GetAwaiter().GetResult();
    }

    private async Task RunMigrations()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Local");
        await Program.Main(["--migrate"]);
    }
}
