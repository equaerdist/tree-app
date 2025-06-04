namespace tree_api.Domain.Repositories;

public interface ISearchRepository
{
    Task<(IReadOnlyDictionary<long, string> Trees, IReadOnlyDictionary<long, string> Nodes)> Search(string searchTerm, int limit, CancellationToken token);
}
