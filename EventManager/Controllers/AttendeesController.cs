using EventManager.Data;
using EventManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Controllers;

[Route("events/{eventId:int}/attendees")]
public class AttendeesController(ApplicationDbContext context) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int eventId, CancellationToken cancellationToken)
    {
        var item = await context.Events.AsNoTracking().Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
        if (item is null) return NotFound();

        ViewBag.EventId = item.Id;
        ViewBag.EventTitle = item.Title;
        return View(item.Attendees.OrderBy(a => a.Name).ToList());
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(int eventId, CancellationToken cancellationToken)
    {
        var eventTitle = await GetEventTitleAsync(eventId, cancellationToken);
        if (eventTitle is null) return NotFound();

        ViewBag.EventTitle = eventTitle;
        return View(new Attendee { EventId = eventId });
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int eventId, [Bind("Name,Email")] Attendee attendee, CancellationToken cancellationToken)
    {
        var eventTitle = await GetEventTitleAsync(eventId, cancellationToken);
        if (eventTitle is null) return NotFound();

        attendee.EventId = eventId;
        if (!ModelState.IsValid)
        {
            ViewBag.EventTitle = eventTitle;
            return View(attendee);
        }

        attendee.Id = Guid.NewGuid().ToString();
        attendee.Name = attendee.Name.Trim();
        attendee.Email = attendee.Email.Trim();
        context.Attendees.Add(attendee);
        await context.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = $"{attendee.Name} was added to {eventTitle}.";
        return RedirectToAction(nameof(Index), new { eventId });
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(int eventId, string id, CancellationToken cancellationToken)
    {
        var attendee = await FindAttendeeAsync(eventId, id, true, cancellationToken);
        if (attendee is null) return NotFound();

        ViewBag.EventTitle = attendee.Event?.Title;
        return View(attendee);
    }

    [HttpPost("{id}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int eventId, string id, [Bind("Name,Email")] Attendee input, CancellationToken cancellationToken)
    {
        var attendee = await context.Attendees.Include(a => a.Event)
            .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId, cancellationToken);
        if (attendee is null) return NotFound();

        if (!ModelState.IsValid)
        {
            input.Id = id;
            input.EventId = eventId;
            ViewBag.EventTitle = attendee.Event?.Title;
            return View(input);
        }

        attendee.Name = input.Name.Trim();
        attendee.Email = input.Email.Trim();
        await context.SaveChangesAsync(cancellationToken);
        TempData["StatusMessage"] = $"{attendee.Name} was updated.";
        return RedirectToAction(nameof(Index), new { eventId });
    }

    [HttpGet("{id}/delete")]
    public async Task<IActionResult> Delete(int eventId, string id, CancellationToken cancellationToken)
    {
        var attendee = await FindAttendeeAsync(eventId, id, true, cancellationToken);
        return attendee is null ? NotFound() : View(attendee);
    }

    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int eventId, string id, CancellationToken cancellationToken)
    {
        var attendee = await context.Attendees
            .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId, cancellationToken);
        if (attendee is not null)
        {
            context.Attendees.Remove(attendee);
            await context.SaveChangesAsync(cancellationToken);
            TempData["StatusMessage"] = $"{attendee.Name} was removed.";
        }

        return RedirectToAction(nameof(Index), new { eventId });
    }

    private Task<string?> GetEventTitleAsync(int eventId, CancellationToken cancellationToken) =>
        context.Events.Where(e => e.Id == eventId).Select(e => e.Title).FirstOrDefaultAsync(cancellationToken);

    private Task<Attendee?> FindAttendeeAsync(int eventId, string id, bool includeEvent, CancellationToken cancellationToken)
    {
        var query = context.Attendees.AsNoTracking();
        if (includeEvent) query = query.Include(a => a.Event);
        return query.FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId, cancellationToken);
    }
}
