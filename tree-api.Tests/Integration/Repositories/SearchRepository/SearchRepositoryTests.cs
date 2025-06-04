using Microsoft.Extensions.DependencyInjection;
using tree_api.Domain.Repositories;
using tree_api.Domain.Services.NodeService;
using tree_api.Domain.Services.TreeService;
using tree_api.Tests.Setup;
using Xunit;

namespace tree_api.Tests.Integration.Repositories.SearchRepository;

public class SearchRepositoryTests : TestBase
{
    public SearchRepositoryTests(TestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Search_ReturnsEntitiesMatchingPattern()
    {
        // Arrange
        var pattern = Faker.Random.AlphaNumeric(6);
        var treeName = pattern + "_Tree_" + Faker.Random.AlphaNumeric(4);
        var nodeName = pattern + "_Node_" + Faker.Random.AlphaNumeric(4);
        var childNodeName = pattern + "_ChildNode_" + Faker.Random.AlphaNumeric(4);
        var searchPattern = pattern;

        using var scope = CreateScope();
        var treeService = scope.ServiceProvider.GetRequiredService<ITreeService>();
        var nodeService = scope.ServiceProvider.GetRequiredService<INodeService>();
        var repo = scope.ServiceProvider.GetRequiredService<ISearchRepository>();

        // Получаем или создаём дерево
        var tree = await treeService.GetOrCreateTreeAsync(treeName, CancellationToken);

        // Создаём узел (родительский)
        var node = await nodeService.Create(tree.Name, tree.Id, nodeName, CancellationToken);

        // Создаём дочерний узел
        var childNode = await nodeService.Create(tree.Name, node, childNodeName, CancellationToken);

        // Act
        var (trees, nodes) = await repo.Search(searchPattern, 10, CancellationToken);

        // Assert
        Assert.Contains(trees, t => t.Value == treeName);
        Assert.Contains(nodes, n => n.Value == nodeName);
        Assert.Contains(nodes, n => n.Value == childNodeName);
    }
}
