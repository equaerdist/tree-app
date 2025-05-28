namespace tree_api.API.Contracts.V1;

public class MJournal
{
    public string Text { get; set; } = string.Empty;
    public long Id { get; set; }
    public long EventId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
