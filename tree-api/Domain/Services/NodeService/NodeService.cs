using Microsoft.EntityFrameworkCore;
using Npgsql;
using tree_api.Database.Models;
using tree_api.Domain.Exceptions;
using tree_api.Domain.Repositories;

namespace tree_api.Domain.Services.NodeService;

internal class NodeService : INodeService
{
    private readonly INodeRepository _repo;

    public NodeService(INodeRepository repo)
    {
        _repo = repo;
    }

    public async Task<long> Create(string treeName, long parentNodeId, string nodeName, CancellationToken token)
    {
        var parent = await _repo.GetNodeAsync(treeName, parentNodeId, token);
        Tree? treeParent = null;

        if (parent == null)
        {
            treeParent = await _repo.GetTreeAsync(treeName, parentNodeId, token);
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

        await _repo.AddNodeAsync(newNode, token);

        try
        {
            await _repo.SaveChangesAsync(token);
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
        var node = await _repo.GetNodeAsync(treeName, nodeId, token);

        if (node == null)
        {
            throw new SecureException("Node not found in the specified tree");
        }

        await _repo.RemoveNodeAsync(node, token);
        await _repo.SaveChangesAsync(token);
    }

    public async Task Rename(string treeName, long nodeId, string newNodeName, CancellationToken token)
    {
        var node = await _repo.GetNodeAsync(treeName, nodeId, token);

        if (node == null)
        {
            throw new SecureException("Node not found in the specified tree");
        }

        node.Name = newNodeName;

        try
        {
            await _repo.SaveChangesAsync(token);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
        {
            throw new SecureException("A sibling node with the same name already exists");
        }
    }
}
