using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Please enter a event name")]
        public string? EventName { get; set; }
        

        public DateTime EventDate { get; set; }


        [Required(ErrorMessage = "Please enter a event description")]
        public string? Description { get; set; }


        public int EventTypeId { get; set; }

        [ForeignKey("EventTypeId")]
        public EventType? EventType { get; set; }
        

        public ICollection<Booking>? Booking { get; set; } = new List<Booking>();

    }
}
