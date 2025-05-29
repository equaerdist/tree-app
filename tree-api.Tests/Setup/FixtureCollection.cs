using Xunit;

namespace tree_api.Tests.Setup;

[CollectionDefinition(FixtureCollectionName)]
public class FixtureCollection : ICollectionFixture<TestFixture>
{
    internal const string FixtureCollectionName = nameof(FixtureCollection);
}