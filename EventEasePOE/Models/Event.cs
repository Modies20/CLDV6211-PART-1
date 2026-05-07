using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(255)]
        [Display(Name = "Event Name")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string EventDescription { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [NotMapped]
        [Display(Name = "Event Image")]
        public IFormFile? ImageFile { get; set; }

        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}