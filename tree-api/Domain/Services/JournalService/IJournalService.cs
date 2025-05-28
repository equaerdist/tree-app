using tree_api.API.Contracts.V1;
using tree_api.Database.Models;

namespace tree_api.Domain.Services.JournalService;

internal interface IJournalService
{
    Task<(IReadOnlyCollection<LogEntry> Items, int Count)> GetJournals(int skip, int take, VJournalFilter? filter, CancellationToken token);
    Task<LogEntry> GetSingle(long id, CancellationToken token);
}
