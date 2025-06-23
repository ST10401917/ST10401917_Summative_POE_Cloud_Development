using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Data
{
    public class EventEaseContext : DbContext
    {
        public EventEaseContext (DbContextOptions<EventEaseContext> options)
            : base(options)
        {
        }

        public DbSet<EventEase.Models.Venue> Venue { get; set; } = default!;
        public DbSet<EventEase.Models.Event> Event { get; set; } = default!;
        public DbSet<EventEase.Models.Booking> Booking { get; set; } = default!;
        public DbSet<EventEase.Models.EventType> EventTypes { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed EventTypes
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, EventTypeName = "Birthday" },
                new EventType { EventTypeId = 2, EventTypeName = "Meeting" },
                new EventType { EventTypeId = 3, EventTypeName = "Wedding" },
                new EventType { EventTypeId = 4, EventTypeName = "Casual-MeetUp" }
            );
        }
    }
}
