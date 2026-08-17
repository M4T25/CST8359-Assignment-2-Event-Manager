using System.ComponentModel.DataAnnotations;

namespace EventManager.Models;

public class Event
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, DataType(DataType.DateTime)]
    public DateTime Date { get; set; }

    [Required, StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [StringLength(2048)]
    public string BannerUrl { get; set; } = string.Empty;

    public List<Attendee> Attendees { get; set; } = [];
}
