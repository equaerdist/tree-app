using Microsoft.AspNetCore.TestHost;

namespace tree_api.Tests.Setup;

internal class TestFixture
{
    private readonly TestServer _server;

    public TestFixture()
    {
        var factory = new CustomWebApplicationFactory();
        _server = factory.Server;
    }
}
