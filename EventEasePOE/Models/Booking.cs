using global::EventEase.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Event selection is required")]
        [Display(Name = "Event")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Venue selection is required")]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Start date/time is required")]
        [Display(Name = "Start Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "End date/time is required")]
        [Display(Name = "End Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [Display(Name = "Customer Name")]
        [StringLength(255)]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Customer Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255)]
        public string? CustomerEmail { get; set; }

        [Display(Name = "Booking Status")]
        [StringLength(50)]
        public string BookingStatus { get; set; } = "Confirmed";

        // Navigation properties
        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; }

        [ForeignKey("VenueId")]
        public virtual Venue? Venue { get; set; }
    }
}