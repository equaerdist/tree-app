using System.ComponentModel.DataAnnotations;

namespace tree_api.Database.Models;

public class Tree
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Node> Nodes { get; set; } = new List<Node>();
}