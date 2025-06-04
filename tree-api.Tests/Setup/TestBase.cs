using Bogus;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using tree.api.Tests.ClientGenerator;
using Xunit;

namespace tree_api.Tests.Setup;

[Collection(FixtureCollection.FixtureCollectionName)]
public class TestBase
{
    protected readonly Faker Faker;
    private readonly TestFixture _fixture;

    public TestBase(TestFixture fixture)
    {
        _fixture = fixture;
        Faker = new();
    }

    protected static CancellationToken CancellationToken => new CancellationTokenSource(TimeSpan.FromSeconds(500)).Token;

    protected TreeAppClient TreeAppClient => _fixture.TreeAppClient;

    protected static TestServer CreateServer(Action<IServiceCollection>? configureServices = null) => TestFixture.CreateServer(configureServices);

    protected IServiceScope CreateScope() => _fixture.CreateScope();
}
