using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.Data;
using EventEase.Models;
using EventEase.Models.ViewModels;

namespace EventEase4.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseContext _context;

        public BookingsController(EventEaseContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(int? venueId)
        {
            var bookings = await _context.Booking
                  .Include(g => g.Venue)
                  .Include(g => g.Event)
                  .ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(g => g.Venue)
                .Include(g => g.Event)
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
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName");
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,VenueId,EventId,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Check for double booking
                bool bookingExists = await _context.Booking.AnyAsync(b =>
                    b.VenueId == booking.VenueId &&
                    b.BookingDate.Date == booking.BookingDate.Date);

                if (bookingExists)
                {
                    ModelState.AddModelError("", "This venue is already booked on the selected date.");

                    // Refill dropdowns
                    ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
                    ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
                    return View(booking);
                }

                // No double booking, safe to save
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Refill dropdowns if ModelState is invalid
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "VenueName");
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName");
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,VenueId,EventId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
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
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(g => g.Venue)
                .Include(g => g.Event)
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
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }


        public async Task<IActionResult> BookingSearch(string bookingId, string eventName, DateTime? startDate, DateTime? endDate, int? eventTypeId)
        {
            var bookings = _context.Booking
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(bookingId) && int.TryParse(bookingId, out int id))
            {
                bookings = bookings.Where(b => b.BookingId == id);
            }
            else if (!string.IsNullOrEmpty(eventName))
            {
                bookings = bookings.Where(b => b.Event.EventName.Contains(eventName));
            }

            if (eventTypeId.HasValue)
            {
                bookings = bookings.Where(b => b.Event.EventTypeId == eventTypeId);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                bookings = bookings.Where(b => b.BookingDate.Date >= startDate.Value.Date &&
                                               b.BookingDate.Date <= endDate.Value.Date);
            }

            ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeId", "EventTypeName");

            var viewModel = await bookings.Select(b => new BookingSearchViewModel
            {
                BookingId = b.BookingId,
                BookingDate = b.BookingDate,
                EventName = b.Event.EventName,
                VenueName = b.Venue.VenueName,
                EventTypeName = b.Event.EventType.EventTypeName,
                Location = b.Venue.Location,
                Capacity = b.Venue.Capacity,
                Description = b.Event.Description
            }).ToListAsync();

            return View(viewModel);
        }

        public async Task<IActionResult> VenueAvailability(DateTime selectedDate)
        {
            var bookedVenueIds = await _context.Booking
                .Where(b => b.BookingDate.Date == selectedDate.Date)
                .Select(b => b.VenueId)
                .ToListAsync();

            var availableVenues = await _context.Venue
                .Where(v => !bookedVenueIds.Contains(v.VenueId))
                .ToListAsync();

            ViewBag.SelectedDate = selectedDate;
            return View(availableVenues);
        }
    }
}
