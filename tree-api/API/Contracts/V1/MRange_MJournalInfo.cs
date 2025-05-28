namespace tree_api.API.Contracts.V1;

public class MRange_MJournalInfo
{
    public int Skip { get; set; }
    public int Count { get; set; }
    public required IReadOnlyCollection<MJournalInfo> Items { get; set; }
}
