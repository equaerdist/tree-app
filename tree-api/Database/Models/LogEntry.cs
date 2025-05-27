using System.ComponentModel.DataAnnotations;

namespace tree_api.Database.Models;

public class LogEntry
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long EventId { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    public string? QueryParameters { get; set; }
    public string? BodyParameters { get; set; }

    [Required]
    public string StackTrace { get; set; } = string.Empty;
}
