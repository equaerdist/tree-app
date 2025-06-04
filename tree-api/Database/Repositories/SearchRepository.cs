using Microsoft.EntityFrameworkCore;

using tree_api.Database.Models;
using tree_api.Domain.Repositories;

namespace tree_api.Database.Repositories;

internal class SearchRepository : ISearchRepository
{
    internal const string Tree = nameof(Tree);
    internal const string Node = nameof(Node);

    private readonly DbCtx _db;

    public SearchRepository(DbCtx db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyDictionary<long, string> Trees, IReadOnlyDictionary<long, string> Nodes)> Search(string searchTerm, int limit, CancellationToken token)
    {
        var result = await _db.Set<SearchUnit>()
            .FromSqlRaw(
                "SELECT * FROM search_trees_and_nodes({0}, {1})",
                searchTerm, limit)
            .ToListAsync(token);

        return (
            result.Where(s => s.EntityType == Tree).ToDictionary(s => s.Id, s => s.Name),
            result.Where(s => s.EntityType == Node).ToDictionary(s => s.Id, s => s.Name));
    }
}

