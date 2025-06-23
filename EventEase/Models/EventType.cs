namespace EventEase.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        public string EventTypeName { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
