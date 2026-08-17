using System.ComponentModel.DataAnnotations;

namespace EventManager.Models;

public class Attendee
{
    [StringLength(36)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(254), EmailAddress]
    public string Email { get; set; } = string.Empty;

    public int EventId { get; set; }
    public Event? Event { get; set; }
}
