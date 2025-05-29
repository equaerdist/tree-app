using Microsoft.EntityFrameworkCore;
using Npgsql;
using tree_api.Database;
using tree_api.Database.Models;
using tree_api.Domain.Exceptions;

namespace tree_api.Domain.Services.NodeService;

internal class NodeService : INodeService
{
    private readonly DbCtx _ctx;

    public NodeService(DbCtx ctx)
    {
        _ctx = ctx;
    }

    public async Task<long> Create(string treeName, long parentNodeId, string nodeName, CancellationToken token)
    {
        var parent = await _ctx.Nodes.FirstOrDefaultAsync(n => n.Id == parentNodeId && n.Tree!.Name == treeName);
        Tree? treeParent = null;

        if (parent == null)
        {
            treeParent = await _ctx.Trees.FirstOrDefaultAsync(n => n.Id == parentNodeId && n.Name == treeName);
        }

        if (parent == null && treeParent == null)
        {
            throw new SecureException("Parent node does not exist in specified tree.");
        }

        var newNode = new Node
        {
            Name = nodeName,
            TreeId = parent?.TreeId ?? treeParent?.Id ?? throw new ArgumentException(),
            ParentId = parent?.Id
        };

        _ctx.Nodes.Add(newNode);

        try
        {
            await _ctx.SaveChangesAsync(token);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx &&
                                    pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new SecureException($"Node with the same name already exists among siblings.");
        }

        return newNode.Id;
    }

    public async Task Delete(string treeName, long nodeId, CancellationToken token)
    {
        var node = await _ctx.Nodes.FirstOrDefaultAsync(n => n.Id == nodeId && n.Tree!.Name == treeName);

        if (node == null)
        {
            throw new SecureException("Node not found in the specified tree");
        }

        _ctx.Nodes.Remove(node);
        await _ctx.SaveChangesAsync();
    }

    public async Task Rename(string treeName, long nodeId, string newNodeName, CancellationToken token)
    {
        var node = await _ctx.Nodes
            .FirstOrDefaultAsync(n => n.Id == nodeId && n.Tree!.Name == treeName);

        if (node == null)
        {
            throw new SecureException("Node not found in the specified tree");
        }

        node.Name = newNodeName;

        try
        {
            await _ctx.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
        {
            throw new SecureException("A sibling node with the same name already exists");
        }
    }
}
