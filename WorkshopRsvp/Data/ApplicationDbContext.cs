using Microsoft.EntityFrameworkCore;
using WorkshopRsvp.Models;

namespace WorkshopRsvp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Rsvp> Rsvps => Set<Rsvp>();
}
