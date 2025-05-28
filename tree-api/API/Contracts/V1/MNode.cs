namespace tree_api.API.Contracts.V1;

public class MNode
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required IReadOnlyCollection<MNode> Children { get; set; }
}
