using tree_api.Database.Models;

namespace tree_api.Domain.Repositories;

    public interface INodeRepository
    {
        Task<Node?> GetNodeAsync(string treeName, long nodeId, CancellationToken token);
        Task<Tree?> GetTreeAsync(string treeName, long treeId, CancellationToken token);
        Task AddNodeAsync(Node node, CancellationToken token);
        Task RemoveNodeAsync(Node node, CancellationToken token);
        Task SaveChangesAsync(CancellationToken token);
    }
