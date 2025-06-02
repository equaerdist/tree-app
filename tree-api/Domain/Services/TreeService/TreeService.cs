using Microsoft.EntityFrameworkCore;
using tree_api.Database;
using tree_api.Database.Models;

namespace tree_api.Domain.Services.TreeService;

internal class TreeService : ITreeService
{
    private readonly DbCtx _ctx;

    public TreeService(DbCtx ctx)
    {
        _ctx = ctx;
    }

    public async Task<Tree> GetOrCreateTreeAsync(string treeName, CancellationToken token)
    {
        var tree = await _ctx.Trees
            .Include(t => t.Nodes)
            .FirstOrDefaultAsync(t => t.Name == treeName);

        if (tree == null)
        {
            tree = new Tree { Name = treeName };
            _ctx.Trees.Add(tree);
            await _ctx.SaveChangesAsync(token);
        }

        return tree;
    }
}
