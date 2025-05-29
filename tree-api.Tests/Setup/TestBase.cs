using Bogus;
using tree.api.Tests.ClientGenerator;
using Xunit;

namespace tree_api.Tests.Setup;

[Collection(FixtureCollection.FixtureCollectionName)]
public class TestBase
{
    private readonly TestFixture _fixture;

    protected TreeAppClient TreeAppClient => _fixture.TreeAppClient;
    protected Faker Faker = new();
    protected CancellationToken CancellationToken => new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

    public TestBase(TestFixture fixture)
    {
        _fixture = fixture;
    }
}
