using EventManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Attendee> Attendees => Set<Attendee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>()
            .HasMany(e => e.Attendees)
            .WithOne(a => a.Event)
            .HasForeignKey(a => a.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
