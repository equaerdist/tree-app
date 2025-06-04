using FluentAssertions;
using tree_api.Tests.Setup;
using Xunit;

namespace tree_api.Tests.Integration.Controllers.V1;

public class TreeControllerTests : TestBase
{
    public TreeControllerTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetOrCreateTree_WhenNotExists_ShouldCreate()
    {
        // Act and Assert
        var action = async () => await TreeAppClient.GetAsync(Faker.Random.String2(10), CancellationToken);
        var node = (await action.Should().NotThrowAsync()).Subject;
    }

    [Fact]
    public async Task GetOrCreateTree_WhenAlreadyExist_ShouldGet()
    {
        // Arrange
        var treeName = Faker.Random.String2(10);
        var createdNode = await TreeAppClient.GetAsync(treeName, CancellationToken);

        // Act
        var action = async () => await TreeAppClient.GetAsync(treeName, CancellationToken);
        var node = (await action.Should().NotThrowAsync()).Subject;

        // Assert
        node.Should().BeEquivalentTo(createdNode);
    }
}
