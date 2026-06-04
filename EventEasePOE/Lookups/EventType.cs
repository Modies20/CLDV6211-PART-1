using System.ComponentModel.DataAnnotations;

namespace EventEase.Lookups
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}   
