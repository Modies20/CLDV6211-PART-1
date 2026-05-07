using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.Services;      //  Required for blob storage

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;   //  Added

        public VenuesController(ApplicationDbContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create (✅ UPDATED with Blob upload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,VenueLocation,Capacity,ImageFile")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var ext = System.IO.Path.GetExtension(venue.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("ImageFile", "Only image files (jpg, jpeg, png, gif) are allowed.");
                        return View(venue);
                    }

                    venue.ImageURL = await _blobService.UploadImageAsync(venue.ImageFile, "venue-images");
                }
                else
                {
                    // Default placeholder image
                    venue.ImageURL = "https://placehold.co/600x400?text=No+Image";
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Venue '{venue.VenueName}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venues/Edit/5 (✅ UPDATED with Blob replace)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,VenueLocation,Capacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (id != venue.VenueId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new image was uploaded, replace the old one
                    if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                    {
                        // Delete old image if it's not the placeholder
                        if (!string.IsNullOrEmpty(venue.ImageURL) && !venue.ImageURL.Contains("placehold.co"))
                        {
                            await _blobService.DeleteImageAsync(venue.ImageURL, "venue-images");
                        }
                        venue.ImageURL = await _blobService.UploadImageAsync(venue.ImageFile, "venue-images");
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Delete/5 ( includes existence check for bookings)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            // Check if venue has existing bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has existing bookings. Please remove or reassign the bookings first.";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // POST: Venues/Delete/5 ( includes blob deletion)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                // Double-check no bookings exist before deletion
                var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Cannot delete venue with existing bookings.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete image from blob storage if not placeholder
                if (!string.IsNullOrEmpty(venue.ImageURL) && !venue.ImageURL.Contains("placehold.co"))
                {
                    await _blobService.DeleteImageAsync(venue.ImageURL, "venue-images");
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}