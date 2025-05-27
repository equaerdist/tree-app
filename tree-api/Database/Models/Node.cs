using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tree_api.Database.Models;

internal class Node
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [ForeignKey(nameof(Tree))]
    public Guid TreeId { get; set; }
    public Tree? Tree { get; set; }

    public long? ParentId { get; set; }
    public Node? Parent { get; set; }

    public ICollection<Node> Children { get; set; } = new List<Node>();
}
