using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        [StringLength(255)]
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(255)]
        [Display(Name = "Location")]
        public string VenueLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000")]
        [Display(Name = "Maximum Capacity")]
        public int Capacity { get; set; }

        // Now we store the blob URL instead of a manual URL string
        [Display(Name = "Image URL")]
        public string ImageURL { get; set; } = string.Empty;

        // For file upload (not stored in database)
        [NotMapped]
        [Display(Name = "Venue Image")]
        public IFormFile? ImageFile { get; set; }

        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}
