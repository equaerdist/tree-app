using Bogus;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using tree.api.Tests.ClientGenerator;
using Xunit;

namespace tree_api.Tests.Setup;

[Collection(FixtureCollection.FixtureCollectionName)]
public class TestBase
{
    private readonly TestFixture _fixture;

    protected TreeAppClient TreeAppClient => _fixture.TreeAppClient;
    protected Faker Faker = new();
    protected CancellationToken CancellationToken => new CancellationTokenSource(TimeSpan.FromSeconds(500)).Token;

    public TestBase(TestFixture fixture)
    {
        _fixture = fixture;
    }

    protected IServiceScope CreateScope() => _fixture.CreateScope();

    protected TestServer CreateServer(Action<IServiceCollection>? configureServices = null) => TestFixture.CreateServer(configureServices);
}
