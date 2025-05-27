using System.ComponentModel.DataAnnotations;

namespace tree_api.Database.Models;

internal class Tree
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Node> Nodes { get; set; } = new List<Node>();
}