using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Get associated bookings for this event
            var bookings = await _context.Bookings
                .Include(b => b.Venue)
                .Where(b => b.EventId == id)
                .ToListAsync();

            ViewBag.Bookings = bookings;
            ViewBag.BookingCount = bookings.Count;

            return View(eventItem);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventDate,EventDescription,ImageURL")] Event eventItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventItem);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,EventDescription,ImageURL")] Event eventItem)
        {
            if (id != eventItem.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventItem.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(eventItem);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventItem == null)
            {
                return NotFound();
            }

            // Check if event has any existing bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it has existing bookings. Please remove or reassign the bookings first.";
                return RedirectToAction(nameof(Index));
            }

            return View(eventItem);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                // Double-check no bookings exist before deletion
                var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Cannot delete event with existing bookings.";
                    return RedirectToAction(nameof(Index));
                }
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
