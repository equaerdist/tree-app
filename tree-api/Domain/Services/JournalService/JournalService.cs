using Microsoft.EntityFrameworkCore;
using tree_api.API.Contracts.V1;
using tree_api.Database;
using tree_api.Database.Models;

namespace tree_api.Domain.Services.JournalService;

internal class JournalService : IJournalService
{
    private readonly DbCtx _ctx;

    public JournalService(DbCtx ctx)
    {
        _ctx = ctx;
    }

    public async Task<(IReadOnlyCollection<LogEntry> Items, int Count)> GetJournals(
        int skip,
        int take,
        VJournalFilter? filter, CancellationToken token)
    {
        var query = _ctx.LogJournal.AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(x =>
                    x.StackTrace.Contains(filter.Search) ||
                    (x.QueryParameters != null && x.QueryParameters.Contains(filter.Search)) ||
                    (x.BodyParameters != null && x.BodyParameters.Contains(filter.Search)));
            }

            if (filter.From.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= filter.From.Value);
            }

            if (filter.To.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= filter.To.Value);
            }
        }

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (items, items.Count);
    }

    public async Task<LogEntry?> GetSingle(long id, CancellationToken token)
    {
        var entry = await _ctx.LogJournal
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entry;
    }
}
