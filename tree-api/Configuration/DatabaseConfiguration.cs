using System.ComponentModel.DataAnnotations;

namespace tree_api.Configuration;

internal sealed class DatabaseConfiguration
{
    [Required]
    public required string ConnectionString { get; set; }
}
