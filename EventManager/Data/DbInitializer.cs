using EventManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        if (await context.Events.AnyAsync())
        {
            return;
        }

        var events = new List<Event>
        {
            new()
            {
                Title = "Routing Workshop",
                Description = "Learn ASP.NET Core attribute routing by building clear, resource-based URLs.",
                Date = new DateTime(2026, 9, 24, 18, 0, 0),
                Location = "Algonquin College - T Building",
                BannerUrl = "/uploads/default1.jpg",
                Attendees =
                [
                    new Attendee { Name = "Alice Smith", Email = "alice@example.com" },
                    new Attendee { Name = "Bob Jones", Email = "bob@example.com" }
                ]
            },
            new()
            {
                Title = "Tech Conference 2026",
                Description = "A full-day conference about cloud, AI, and enterprise applications.",
                Date = new DateTime(2026, 10, 15, 9, 0, 0),
                Location = "Shaw Centre, Ottawa",
                BannerUrl = "/uploads/default2.jpg",
                Attendees =
                [
                    new Attendee { Name = "Charlie Brown", Email = "charlie@example.com" },
                    new Attendee { Name = "Dana White", Email = "dana@example.com" }
                ]
            },
            new()
            {
                Title = "EF Core Bootcamp",
                Description = "A hands-on workshop covering Entity Framework Core, migrations, and Azure SQL.",
                Date = new DateTime(2026, 11, 7, 10, 0, 0),
                Location = "Online",
                BannerUrl = "/uploads/default3.jpg",
                Attendees =
                [
                    new Attendee { Name = "Evan Lee", Email = "evan@example.com" },
                    new Attendee { Name = "Fatima Noor", Email = "fatima@example.com" }
                ]
            }
        };

        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
    }
}
