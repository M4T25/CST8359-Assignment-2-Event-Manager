using EventManager.Data;
using EventManager.Models;
using EventManager.Services;
using EventManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Controllers;

[Route("events")]
public class EventsController(
    ApplicationDbContext context,
    IBannerStorageService bannerStorage) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var events = await context.Events
            .AsNoTracking()
            .Include(e => e.Attendees)
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken);
        return View(events);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var item = await context.Events
            .AsNoTracking()
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return item is null ? NotFound() : View(item);
    }

    [HttpGet("create")]
    public IActionResult Create() => View(new EventFormViewModel());

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        EventFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.BannerImage is null)
        {
            ModelState.AddModelError(nameof(model.BannerImage), "A banner image is required.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var item = new Event
            {
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                Date = model.Date,
                Location = model.Location.Trim(),
                BannerUrl = await bannerStorage.SaveAsync(model.BannerImage!, cancellationToken)
            };

            context.Events.Add(item);
            await context.SaveChangesAsync(cancellationToken);
            TempData["StatusMessage"] = $"{item.Title} was created.";
            return RedirectToAction(nameof(Details), new { id = item.Id });
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(model.BannerImage), exception.Message);
            return View(model);
        }
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var item = await context.Events.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return View(new EventFormViewModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Date = item.Date,
            Location = item.Location,
            ExistingBannerUrl = item.BannerUrl
        });
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        EventFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        var item = await context.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.ExistingBannerUrl = item.BannerUrl;
            return View(model);
        }

        try
        {
            item.Title = model.Title.Trim();
            item.Description = model.Description.Trim();
            item.Date = model.Date;
            item.Location = model.Location.Trim();

            if (model.BannerImage is not null)
            {
                item.BannerUrl = await bannerStorage.SaveAsync(model.BannerImage, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
            TempData["StatusMessage"] = $"{item.Title} was updated.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (InvalidOperationException exception)
        {
            model.ExistingBannerUrl = item.BannerUrl;
            ModelState.AddModelError(nameof(model.BannerImage), exception.Message);
            return View(model);
        }
    }

    [HttpGet("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await context.Events.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var item = await context.Events.FindAsync([id], cancellationToken);
        if (item is not null)
        {
            context.Events.Remove(item);
            await context.SaveChangesAsync(cancellationToken);
        }

        TempData["StatusMessage"] = "The event and its attendees were deleted.";
        return RedirectToAction(nameof(Index));
    }
}
