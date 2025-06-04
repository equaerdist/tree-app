using Microsoft.EntityFrameworkCore;
using tree_api.Database.Models;
using tree_api.Domain.Repositories;

namespace tree_api.Database.Repositories;

internal class NodeRepository : INodeRepository
{
    private readonly DbCtx _ctx;

    public NodeRepository(DbCtx ctx)
    {
        _ctx = ctx;
    }

    public async Task<Node?> GetNodeAsync(string treeName, long nodeId, CancellationToken token)
    {
        return await _ctx.Nodes
            .FirstOrDefaultAsync(n => n.Id == nodeId && n.Tree!.Name == treeName, token);
    }

    public async Task<Tree?> GetTreeAsync(string treeName, long treeId, CancellationToken token)
    {
        return await _ctx.Trees
            .FirstOrDefaultAsync(t => t.Id == treeId && t.Name == treeName, token);
    }

    public async Task AddNodeAsync(Node node, CancellationToken token)
    {
        await _ctx.Nodes.AddAsync(node, token);
    }

    public async Task RemoveNodeAsync(Node node, CancellationToken token)
    {
        _ctx.Nodes.Remove(node);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken token)
    {
        await _ctx.SaveChangesAsync(token);
    }
}
