using System.ComponentModel.DataAnnotations;

namespace WorkshopRsvp.Models;

public class Rsvp
{
    public int Id { get; set; }
    [Required, StringLength(100), Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;
    [Required, StringLength(100), Display(Name = "Workshop")]
    public string WorkshopTitle { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Display(Name = "Needs accommodation")]
    public bool NeedsAccommodation { get; set; }
    [Display(Name = "Will attend")]
    public bool WillAttend { get; set; }
}
