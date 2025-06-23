using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Please enter a venue name")]
        public string? VenueName { get; set; }



        [Required(ErrorMessage = "Please enter a venue location")]
        public string? Location { get; set; }



        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }


        public string? ImageUrl { get; set; }


        public ICollection<Booking>? Booking { get; set; } = new List<Booking>();

    }
}
