namespace tree_api.API.Contracts.V1;

public class VJournalFilter
{
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public string? Search { get; set; }
}
