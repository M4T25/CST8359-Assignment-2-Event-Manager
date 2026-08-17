using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopRsvp.Data;

namespace WorkshopRsvp.Controllers;

public class WorkshopsController(ApplicationDbContext context) : Controller
{
    public IActionResult Index() => View();
    public async Task<IActionResult> Registrations() =>
        View(await context.Rsvps.AsNoTracking().OrderBy(r => r.Id).ToListAsync());
}
