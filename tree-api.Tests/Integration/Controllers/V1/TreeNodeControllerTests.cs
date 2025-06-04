using FluentAssertions;
using tree_api.Tests.Setup;
using tree.api.Tests.ClientGenerator;
using Xunit;

namespace tree_api.Tests.Integration.Controllers.V1;

public class TreeNodeControllerTests : TestBase
{
    public TreeNodeControllerTests(TestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateNode_WhenNotUniqueAcrossSiblings_ShouldThrow()
    {
        // Arrange
        var treeName = Faker.Random.String2(10);
        var tree = await TreeAppClient.GetAsync(treeName, CancellationToken);
        var nodeName = Faker.Random.String2(10);

        // Прикрепимся к дереву сначала (первая нода в нем)
        var response = await TreeAppClient.CreateAsync(treeName, tree.Id, nodeName, CancellationToken);
        await TreeAppClient.CreateAsync(treeName, response.Id, nodeName, CancellationToken);

        var action = async () => await TreeAppClient.CreateAsync(treeName, response.Id, nodeName, CancellationToken);

        // Act and Assert
        (await action.Should().ThrowAsync<ApiException>()).And.Message.Should().Contain("Node with the same name already exists among siblings");
    }

    [Fact]
    public async Task RenameNode_WhenAlreadyExistsTheSame_ShouldThrow()
    {
        // Arrange
        var treeName = Faker.Random.String2(10);
        var tree = await TreeAppClient.GetAsync(treeName, CancellationToken);
        var nodeName = Faker.Random.String2(10);
        var secondNodeName = Faker.Random.String2(10);

        // Прикрепимся к дереву сначала (первая нода в нем)
        var response = await TreeAppClient.CreateAsync(treeName, tree.Id, nodeName, CancellationToken);
        await TreeAppClient.CreateAsync(treeName, response.Id, nodeName, CancellationToken);
        var secondChildResponse = await TreeAppClient.CreateAsync(treeName, response.Id, secondNodeName, CancellationToken);

        var action = async () => await TreeAppClient.RenameAsync(treeName, secondChildResponse.Id, nodeName, CancellationToken);

        // Act and Assert
        (await action.Should().ThrowAsync<ApiException>()).And.Message.Should().Contain("A sibling node with the same name already exists");
    }

    [Fact]
    public async Task Delete_Test()
    {
        // Arrange
        var treeName = Faker.Random.String2(10);
        var tree = await TreeAppClient.GetAsync(treeName, CancellationToken);
        var nodeName = Faker.Random.String2(10);

        // Прикрепимся к дереву сначала (первая нода в нем)
        var response = await TreeAppClient.CreateAsync(treeName, tree.Id, nodeName, CancellationToken);

        var action = async () => await TreeAppClient.DeleteAsync(treeName, response.Id, CancellationToken);

        // Act and Assert
        await action.Should().NotThrowAsync<ApiException>();
    }

    [Fact]
    public async Task Delete_WhenNotExists_ShouldThrow()
    {
        // Arrange
        var action = async () => await TreeAppClient.DeleteAsync(Faker.Random.String2(10), Faker.Random.Long(), CancellationToken);

        // Act and Assert
        (await action.Should().ThrowAsync<ApiException>()).And.Message.Should().Contain("Node not found in the specified tree");
    }
}