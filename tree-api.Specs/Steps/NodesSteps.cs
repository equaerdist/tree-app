using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using tree_api.Domain.Repositories;
using tree_api.Domain.Services.NodeService;
using tree_api.Domain.Services.TreeService;
using tree_api.Specs.Contexts;
using Xunit;

namespace tree_api.Specs.StepDefinitions;

[Binding]
internal class NodesSteps
{
    private readonly TestContext _testContext;

    private readonly Dictionary<string, long> _treeRoots = new();
    private readonly Dictionary<(string, string), long> _nodeIds = new();

    public NodesSteps(TestContext ctx)
    {
        _testContext = ctx;
    }

    [Given(@"существует дерево с именем ""(.*)""")]
    public async Task GivenTreeExists(string treeName)
    {
        var rootNodeId = await TreeService.GetOrCreateTreeAsync(treeName, CancellationToken.None);
        _treeRoots[treeName] = rootNodeId.Id;
    }

    [Given(@"существует узел с именем ""(.*)"" в дереве ""(.*)""")]
    public async Task GivenNodeExists(string nodeName, string treeName)
    {
        var parentId = _treeRoots.TryGetValue(treeName, out var rootId) ? rootId : 0;
        var nodeId = await NodeService.Create(treeName, parentId, nodeName, CancellationToken.None);
        _nodeIds[(treeName, nodeName)] = nodeId;
    }

    [When(@"я создаю узел с именем ""(.*)"" под ""(.*)"" в дереве ""(.*)""")]
    public async Task WhenICreateNodeUnderParent(string childNodeName, string parentNodeName, string treeName)
    {
        var parentKey = (treeName, parentNodeName);
        Assert.True(_nodeIds.ContainsKey(parentKey), $"Родительский узел '{parentNodeName}' не найден в дереве '{treeName}'");
        var parentId = _nodeIds[parentKey];
        var childId = await NodeService.Create(treeName, parentId, childNodeName, CancellationToken.None);
        _nodeIds[(treeName, childNodeName)] = childId;
    }

    [Then(@"узел ""(.*)"" должен существовать под ""(.*)"" в дереве ""(.*)""")]
    public async Task ThenNodeShouldExistUnderParent(string childNodeName, string parentNodeName, string treeName)
    {
        var childKey = (treeName, childNodeName);
        var parentKey = (treeName, parentNodeName);
        Assert.True(_nodeIds.ContainsKey(childKey), $"Узел '{childNodeName}' не найден в дереве '{treeName}'");
        Assert.True(_nodeIds.ContainsKey(parentKey), $"Родительский узел '{parentNodeName}' не найден в дереве '{treeName}'");
        var child = await NodeRepository.GetNodeAsync(treeName, _nodeIds[childKey], CancellationToken.None);
        Assert.NotNull(child);
        Assert.Equal(childNodeName, child.Name);
        Assert.Equal(_nodeIds[parentKey], child.ParentId);
    }

    [When(@"я удаляю узел ""(.*)"" в дереве ""(.*)""")]
    public async Task WhenIDeleteNode(string nodeName, string treeName)
    {
        var key = (treeName, nodeName);
        Assert.True(_nodeIds.ContainsKey(key), $"Узел '{nodeName}' не найден в дереве '{treeName}'");
        await NodeService.Delete(treeName, _nodeIds[key], CancellationToken.None);
    }

    [Then(@"узел ""(.*)"" не должен существовать в дереве ""(.*)""")]
    public async Task ThenNodeShouldNotExist(string nodeName, string treeName)
    {
        var key = (treeName, nodeName);
        if (_nodeIds.TryGetValue(key, out var nodeId))
        {
            var node = await NodeRepository.GetNodeAsync(treeName, nodeId, CancellationToken.None);
            Assert.Null(node);
        }
        // Если узла в словаре и так нет, то всё корректно
    }

    [When(@"я переименовываю узел ""(.*)"" в ""(.*)"" в дереве ""(.*)""")]
    public async Task WhenIRenameNode(string oldName, string newName, string treeName)
    {
        var key = (treeName, oldName);
        Assert.True(_nodeIds.ContainsKey(key), $"Узел '{oldName}' не найден в дереве '{treeName}'");
        var nodeId = _nodeIds[key];
        await NodeService.Rename(treeName, nodeId, newName, CancellationToken.None);
        _nodeIds.Remove(key);
        _nodeIds[(treeName, newName)] = nodeId;
    }

    [Then(@"узел ""(.*)"" должен существовать в дереве ""(.*)""")]
    public async Task ThenNodeShouldExist(string nodeName, string treeName)
    {
        var key = (treeName, nodeName);
        Assert.True(_nodeIds.ContainsKey(key), $"Узел '{nodeName}' не найден в дереве '{treeName}'");
        var node = await NodeRepository.GetNodeAsync(treeName, _nodeIds[key], CancellationToken.None);
        Assert.NotNull(node);
        Assert.Equal(nodeName, node.Name);
    }

    public INodeService NodeService => _testContext
        .Server
        .Services
        .GetRequiredService<INodeService>();

    public ITreeService TreeService => _testContext
        .Server
        .Services
        .GetRequiredService<ITreeService>();

    public INodeRepository NodeRepository => _testContext
        .Server
        .Services
        .GetRequiredService<INodeRepository>();
}