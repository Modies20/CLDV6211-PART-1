using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.Lookups;
using EventEase.ViewModels;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Using public BookingViewModel in EventEasePOE.ViewModels for views

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings (simple)
        [NonAction]
        // This overload is not an MVC action to avoid ambiguous route matching. It delegates to the advanced Index.
        public async Task<IActionResult> Index(string searchTerm)
        {
            // Delegate to the advanced Index overload to keep a single code path
            return await Index(searchTerm, null, null, null, null);
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
        public IActionResult Create()
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

        // GET: Bookings/Calendar
        public async Task<IActionResult> Calendar()
        {
            var allBookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .Where(b => b.BookingStatus != "Cancelled")
                .Select(b => new EventEase.ViewModels.BookingViewModel
                {
                    BookingId = b.BookingId,
                    EventName = b.Event != null ? b.Event.EventName : "N/A",
                    VenueName = b.Venue != null ? b.Venue.VenueName : "N/A",
                    StartDateTime = b.StartDateTime,
                    EndDateTime = b.EndDateTime,
                    CustomerName = b.CustomerName,
                    BookingStatus = b.BookingStatus
                })
                .ToListAsync();
            return View(allBookings);
        }

        // GET: Bookings/MyBookings (Search by Customer)
        public async Task<IActionResult> MyBookings(string searchName)
        {
            ViewBag.SearchName = searchName;

            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .Select(b => new EventEase.ViewModels.BookingViewModel
                {
                    BookingId = b.BookingId,
                    EventName = b.Event != null ? b.Event.EventName : "N/A",
                    VenueName = b.Venue != null ? b.Venue.VenueName : "N/A",
                    StartDateTime = b.StartDateTime,
                    EndDateTime = b.EndDateTime,
                    CustomerName = b.CustomerName,
                    BookingStatus = b.BookingStatus
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                bookings = bookings.Where(b => b.CustomerName.Contains(searchName.Trim()));
            }

            var list = await bookings.OrderByDescending(b => b.StartDateTime).ToListAsync();
            ViewBag.SearchName = searchName;
            return View("SearchByCustomer", list);
        }

        // GET: Bookings/Upcoming
        public async Task<IActionResult> Upcoming()
        {
            var upcomingBookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .Where(b => b.StartDateTime > DateTime.Now && b.BookingStatus != "Cancelled")
                .Select(b => new EventEase.ViewModels.BookingViewModel
                {
                    BookingId = b.BookingId,
                    EventName = b.Event != null ? b.Event.EventName : "N/A",
                    VenueName = b.Venue != null ? b.Venue.VenueName : "N/A",
                    StartDateTime = b.StartDateTime,
                    EndDateTime = b.EndDateTime,
                    CustomerName = b.CustomerName,
                    BookingStatus = b.BookingStatus
                })
                .OrderBy(b => b.StartDateTime)
                .ToListAsync();
            return View(upcomingBookings);
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }

        public async Task<IActionResult> Index(string searchTerm, int? eventTypeId, DateTime? startDate, DateTime? endDate, bool? availableOnly)
        {
            ViewBag.SearchTerm = searchTerm;
            ViewBag.EventTypeId = eventTypeId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.AvailableOnly = availableOnly ?? false;

            // Populate EventType dropdown for the filter UI
            ViewBag.EventTypeList = new SelectList(_context.EventTypes, "EventTypeId", "CategoryName", eventTypeId);

            try
            {
                var bookingsQuery = _context.Bookings
                    .Include(b => b.Event)
                        .ThenInclude(e => e.EventType)
                    .Include(b => b.Venue)
                    .Select(b => new EventEase.ViewModels.BookingViewModel
                    {
                        BookingId = b.BookingId,
                        EventName = b.Event != null ? b.Event.EventName : "N/A",
                        EventDate = b.Event != null ? b.Event.EventDate : DateTime.MinValue,
                        EventType = b.Event != null && b.Event.EventType != null ? b.Event.EventType.CategoryName : "Not Set",
                        VenueName = b.Venue != null ? b.Venue.VenueName : "N/A",
                        VenueLocation = b.Venue != null ? b.Venue.VenueLocation : "N/A",
                        VenueCapacity = b.Venue != null ? b.Venue.Capacity : 0,
                        StartDateTime = b.StartDateTime,
                        EndDateTime = b.EndDateTime,
                        CustomerName = b.CustomerName,
                        CustomerEmail = b.CustomerEmail,
                        BookingStatus = b.BookingStatus,
                        BookingDate = b.BookingDate
                    })
                    .AsQueryable();

                // Search term filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.Trim();
                    bookingsQuery = bookingsQuery.Where(b =>
                        b.BookingId.ToString().Contains(searchTerm) ||
                        b.EventName.Contains(searchTerm) ||
                        b.CustomerName.Contains(searchTerm));
                }

                // Event Type filter
                if (eventTypeId.HasValue)
                {
                    var eventType = _context.EventTypes.FirstOrDefault(et => et.EventTypeId == eventTypeId.Value);
                    if (eventType != null && !string.IsNullOrEmpty(eventType.CategoryName))
                    {
                        bookingsQuery = bookingsQuery.Where(b => !string.IsNullOrEmpty(b.EventType) && b.EventType!.Contains(eventType.CategoryName));
                    }
                }

                // Date range filters
                if (startDate.HasValue)
                    bookingsQuery = bookingsQuery.Where(b => b.StartDateTime.Date >= startDate.Value.Date);
                if (endDate.HasValue)
                    bookingsQuery = bookingsQuery.Where(b => b.EndDateTime.Date <= endDate.Value.Date);

                // Venue availability filter (future bookings only)
                if (availableOnly.HasValue && availableOnly.Value)
                    bookingsQuery = bookingsQuery.Where(b => b.StartDateTime > DateTime.Now);

                var list = await bookingsQuery.OrderByDescending(b => b.StartDateTime).ToListAsync();
                ViewBag.Bookings = list;
                return View();
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetService(typeof(ILogger<BookingsController>)) as ILogger<BookingsController>;
                logger?.LogError(ex, "Failed to load bookings for Index view.");
                TempData["ErrorMessage"] = "Unable to load bookings. Check the logs for details.";
                TempData["ErrorDetails"] = ex.ToString();
                ViewBag.Bookings = new List<EventEase.ViewModels.BookingViewModel>();
                return View();
            }
        }
    }
}
