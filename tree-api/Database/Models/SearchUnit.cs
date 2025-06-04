using Microsoft.EntityFrameworkCore;

namespace tree_api.Database.Models;

[Keyless]
public class SearchUnit
{
    public string EntityType { get; set; } = default!;
    public long Id { get; set; }
    public string Name { get; set; } = default!;
}
