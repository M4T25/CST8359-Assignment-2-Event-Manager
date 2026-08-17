using WorkshopRsvp.Models;

namespace WorkshopRsvp.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        if (context.Rsvps.Any()) return;
        context.Rsvps.AddRange(
            new Rsvp { FullName = "Mateusz Gumienny", WorkshopTitle = "Routing Basics", Email = "gumi0002@algonquinlive.com", NeedsAccommodation = false, WillAttend = true },
            new Rsvp { FullName = "Sarah Ahmed", WorkshopTitle = "EF Core Introduction", Email = "sarah@example.com", NeedsAccommodation = true, WillAttend = true },
            new Rsvp { FullName = "John Smith", WorkshopTitle = "Razor Essentials", Email = "john@example.com", NeedsAccommodation = false, WillAttend = false });
        context.SaveChanges();
    }
}
