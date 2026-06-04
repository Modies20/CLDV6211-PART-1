using EventEase.Lookups;
using EventEase.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent deletion of Venue if it has existing bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VenueId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevents cascade delete

            // Prevent deletion of Event if it has existing bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevents cascade delete

            // Unique constraint to prevent double bookings for same venue overlapping times
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.VenueId, b.StartDateTime, b.EndDateTime })
                .IsUnique()
                .HasDatabaseName("IX_UniqueVenueBooking");

            // Seed sample data with placeholder images
            modelBuilder.Entity<Venue>().HasData(
                new Venue { VenueId = 1, VenueName = "Grand Conference Hall", VenueLocation = "123 Main St, Downtown", Capacity = 500, ImageURL = "https://picsum.photos/id/20/400/300" },
                new Venue { VenueId = 2, VenueName = "Sunset Garden", VenueLocation = "456 Park Ave, Uptown", Capacity = 200, ImageURL = "https://picsum.photos/id/15/400/300" },
                new Venue { VenueId = 3, VenueName = "Skyline Rooftop", VenueLocation = "789 High St, City Center", Capacity = 150, ImageURL = "https://picsum.photos/id/30/400/300" }
            );

            modelBuilder.Entity<Event>().HasData(
                new Event { EventId = 1, EventName = "Annual Tech Summit", EventDate = new DateTime(2026, 5, 15), EventDescription = "Annual technology conference featuring industry leaders", ImageUrl = "https://picsum.photos/id/0/400/300", EventTypeId = 1 },
                new Event { EventId = 2, EventName = "Wedding Expo", EventDate = new DateTime(2026, 6, 10), EventDescription = "Showcase of wedding vendors and services", ImageUrl = "https://picsum.photos/id/26/400/300", EventTypeId = 1 },
                new Event { EventId = 3, EventName = "Corporate Gala", EventDate = new DateTime(2026, 7, 20), EventDescription = "Annual charity fundraising gala", ImageUrl = "https://picsum.photos/id/29/400/300", EventTypeId = 1 }
            );

            // Seed Event Types
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, CategoryName = "Conference", Description = "Business and tech conferences" },
                new EventType { EventTypeId = 2, CategoryName = "Wedding", Description = "Wedding ceremonies and receptions" },
                new EventType { EventTypeId = 3, CategoryName = "Concert", Description = "Live music performances" },
                new EventType { EventTypeId = 4, CategoryName = "Corporate", Description = "Company meetings and galas" },
                new EventType { EventTypeId = 5, CategoryName = "Private Party", Description = "Private celebrations and parties" }
);
        }
    }
}
