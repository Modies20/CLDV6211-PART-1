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
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .OrderBy(b => b.StartDateTime);

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");

            // Set default dates to help user
            ViewBag.DefaultStart = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddTHH:mm");
            ViewBag.DefaultEnd = DateTime.Now.AddDays(7).AddHours(2).ToString("yyyy-MM-ddTHH:mm");

            return View();
        }

        // Helper method to check venue availability
        private bool IsVenueAvailable(int venueId, DateTime startDateTime, DateTime endDateTime, int? excludeBookingId = null)
        {
            var conflictingBookings = _context.Bookings
                .Where(b => b.VenueId == venueId
                    && b.StartDateTime < endDateTime
                    && b.EndDateTime > startDateTime
                    && (excludeBookingId == null || b.BookingId != excludeBookingId))
                .Any();

            return !conflictingBookings;
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate,StartDateTime,EndDateTime,CustomerName,CustomerEmail,BookingStatus")] Booking booking)
        {
            // Validate dates
            if (booking.StartDateTime >= booking.EndDateTime)
            {
                ModelState.AddModelError("", "End date/time must be after start date/time.");
            }

            // Check for past dates
            if (booking.StartDateTime < DateTime.Now)
            {
                ModelState.AddModelError("StartDateTime", "Cannot book a venue for a past date/time.");
            }

            // Check for venue availability (prevents double bookings)
            if (ModelState.IsValid && !IsVenueAvailable(booking.VenueId, booking.StartDateTime, booking.EndDateTime))
            {
                ModelState.AddModelError("", "This venue is already booked for the selected time period. Please choose different dates/times.");
            }

            if (ModelState.IsValid)
            {
                // Set booking date to today if not provided
                if (booking.BookingDate == default)
                {
                    booking.BookingDate = DateTime.Today;
                }

                // Set default status if not provided
                if (string.IsNullOrEmpty(booking.BookingStatus))
                {
                    booking.BookingStatus = "Confirmed";
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Booking #{booking.BookingId} created successfully for {booking.CustomerName}.";
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns if validation fails
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate,StartDateTime,EndDateTime,CustomerName,CustomerEmail,BookingStatus")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            // Validate dates
            if (booking.StartDateTime >= booking.EndDateTime)
            {
                ModelState.AddModelError("", "End date/time must be after start date/time.");
            }

            // Check for venue availability (excluding current booking)
            if (ModelState.IsValid && !IsVenueAvailable(booking.VenueId, booking.StartDateTime, booking.EndDateTime, booking.BookingId))
            {
                ModelState.AddModelError("", "This venue is already booked for the selected time period. Please choose different dates/times.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Booking #{booking.BookingId} updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Booking #{id} cancelled successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Calendar (Bonus view for visual overview)
        public async Task<IActionResult> Calendar()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/MyBookings (Search by customer name)
        public async Task<IActionResult> MyBookings(string searchName)
        {
            ViewBag.SearchName = searchName;

            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .OrderBy(b => b.StartDateTime);

            if (!string.IsNullOrEmpty(searchName))
            {
                bookings = bookings.Where(b => b.CustomerName.Contains(searchName)) as IOrderedQueryable<Booking>;
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Upcoming (Show only upcoming bookings)
        public async Task<IActionResult> Upcoming()
        {
            var upcomingBookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .Where(b => b.StartDateTime > DateTime.Now)
                .OrderBy(b => b.StartDateTime)
                .ToListAsync();

            ViewBag.Count = upcomingBookings.Count;
            return View(upcomingBookings);
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
