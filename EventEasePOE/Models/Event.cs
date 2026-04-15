using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [Display(Name = "Event Name")]
        [StringLength(255)]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Event description is required")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string EventDescription { get; set; } = string.Empty;

        [Display(Name = "Event Image")]
        [StringLength(500)]
        public string? ImageURL { get; set; }

        // Navigation property
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}