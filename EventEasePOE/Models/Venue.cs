using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        [Display(Name = "Venue Name")]
        [StringLength(255)]
        public string VenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Venue location is required")]
        [Display(Name = "Location")]
        [StringLength(255)]
        public string VenueLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000")]
        [Display(Name = "Maximum Capacity")]
        public int Capacity { get; set; }

        [Display(Name = "Venue Image")]
        [StringLength(500)]
        public string? ImageURL { get; set; }

        // Navigation property
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}


