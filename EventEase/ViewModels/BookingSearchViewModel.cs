namespace EventEase.Models.ViewModels
{
    public class BookingSearchViewModel
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }

        public string EventName { get; set; } = string.Empty;
        public string? EventType { get; set; }

        public string VenueName { get; set; } = string.Empty;
        public string? EventTypeName { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Description { get; set; }
    }
}
