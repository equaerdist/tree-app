namespace tree_api.Domain.Services.NodeService;

public interface INodeService
{
    Task Create(string treeName, long parentNodeId, string nodeName, CancellationToken token);
    Task Delete(string treeName, long nodeId, CancellationToken token);
    Task Rename(string treeName, long nodeId, string newNodeName, CancellationToken token);
}
