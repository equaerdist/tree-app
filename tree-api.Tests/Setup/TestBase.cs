using Xunit;

namespace tree_api.Tests.Setup;

[Collection(FixtureCollection.FixtureCollectionName)]
internal class TestBase
{
    private readonly TestFixture _fixture;

    public TestBase(TestFixture fixture)
    {
        _fixture = fixture;
    }
}
