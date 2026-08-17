using System.ComponentModel.DataAnnotations;

namespace EventManager.ViewModels;

public class EventFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, DataType(DataType.DateTime)]
    [Display(Name = "Date and time")]
    public DateTime Date { get; set; } = DateTime.Today.AddDays(7).AddHours(18);

    [Required, StringLength(200)]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Banner image")]
    public IFormFile? BannerImage { get; set; }

    public string ExistingBannerUrl { get; set; } = string.Empty;
}
