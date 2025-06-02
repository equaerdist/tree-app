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
    public async Task ƒопустим—уществуетƒерево—»менем(string treeName)
    {
        var rootNodeId = await TreeService.GetOrCreateTreeAsync(treeName, CancellationToken.None);
        _treeRoots[treeName] = rootNodeId.Id;
    }

    [Given(@"существует узел с именем ""(.*)"" в дереве ""(.*)""")]
    public async Task ƒопустим—уществует”зел—»менем¬ƒереве(string nodeName, string treeName)
    {
        // —оздаЄм узел под корнем
        var parentId = _treeRoots.TryGetValue(treeName, out var rootId) ? rootId : 0;
        var nodeId = await NodeService.Create(treeName, parentId, nodeName, CancellationToken.None);
        _nodeIds[(treeName, nodeName)] = nodeId;
    }

    [When(@"€ создаю узел с именем ""(.*)"" под ""(.*)"" в дереве ""(.*)""")]
    public async Task ≈слия—оздаю”зел—»менемѕод¬ƒереве(string childNodeName, string parentNodeName, string treeName)
    {
        var parentKey = (treeName, parentNodeName);
        Assert.True(_nodeIds.ContainsKey(parentKey), $"–одительский узел '{parentNodeName}' не найден в дереве '{treeName}'");
        var parentId = _nodeIds[parentKey];
        var childId = await NodeService.Create(treeName, parentId, childNodeName, CancellationToken.None);
        _nodeIds[(treeName, childNodeName)] = childId;
    }

    [Then(@"узел ""(.*)"" должен существовать под ""(.*)"" в дереве ""(.*)""")]
    public async Task “о”зелƒолжен—уществоватьѕод¬ƒереве(string childNodeName, string parentNodeName, string treeName)
    {
        var childKey = (treeName, childNodeName);
        var parentKey = (treeName, parentNodeName);
        Assert.True(_nodeIds.ContainsKey(childKey), $"”зел '{childNodeName}' не найден в дереве '{treeName}'");
        Assert.True(_nodeIds.ContainsKey(parentKey), $"–одительский узел '{parentNodeName}' не найден в дереве '{treeName}'");
        var child = await NodeRepository.GetNodeAsync(treeName, _nodeIds[childKey], CancellationToken.None);
        Assert.NotNull(child);
        Assert.Equal(childNodeName, child.Name);
        Assert.Equal(_nodeIds[parentKey], child.ParentId);
    }

    [When(@"€ удал€ю узел ""(.*)"" в дереве ""(.*)""")]
    public async Task ≈слия”дал€ю”зел¬ƒереве(string nodeName, string treeName)
    {
        var key = (treeName, nodeName);
        Assert.True(_nodeIds.ContainsKey(key), $"”зел '{nodeName}' не найден в дереве '{treeName}'");
        await NodeService.Delete(treeName, _nodeIds[key], CancellationToken.None);
    }

    [Then(@"узел ""(.*)"" не должен существовать в дереве ""(.*)""")]
    public async Task “о”зелЌеƒолжен—уществовать¬ƒереве(string nodeName, string treeName)
    {
        var key = (treeName, nodeName);
        if (_nodeIds.TryGetValue(key, out var nodeId))
        {
            var node = await NodeRepository.GetNodeAsync(treeName, nodeId, CancellationToken.None);
            Assert.Null(node);
        }
        // ≈сли нет в словаре Ч значит уже удалЄн, что тоже корректно
    }

    [When(@"€ переименовываю узел ""(.*)"" в ""(.*)"" в дереве ""(.*)""")]
    public async Task ≈слияѕереименовываю”зел¬¬ƒереве(string oldName, string newName, string treeName)
    {
        var key = (treeName, oldName);
        Assert.True(_nodeIds.ContainsKey(key), $"”зел '{oldName}' не найден в дереве '{treeName}'");
        var nodeId = _nodeIds[key];
        await NodeService.Rename(treeName, nodeId, newName, CancellationToken.None);
        _nodeIds.Remove(key);
        _nodeIds[(treeName, newName)] = nodeId;
    }

    [Then(@"узел ""(.*)"" должен существовать в дереве ""(.*)""")]
    public async Task “о”зелƒолжен—уществовать¬ƒереве(string nodeName, string treeName)
    {
        var key = (treeName, nodeName);
        Assert.True(_nodeIds.ContainsKey(key), $"”зел '{nodeName}' не найден в дереве '{treeName}'");
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
