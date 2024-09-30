using System.ComponentModel.DataAnnotations;

namespace HazelClinic3.Models
{
    public class EventReg
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [Display(Name = "Full Name:")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Contact Number:")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact Number must be exactly 10 digits.")]
        public string ContactNumber { get; set; }

        [Required]
        [Display(Name = "Email:")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must agree to the terms.")]
        [Display(Name = "I hereby agree that tickets are non-refundale")]
        public bool IsNotRefundable { get; set; }

        [Required]
        [Range(1, 200, ErrorMessage = "Please select at least one ticket")]
        [Display(Name = "Quantity:")]
        public int Quantity { get; set; }

        public decimal TotalCost { get; set; }

        public string TicketNumber { get; set; }

        // New field for checking in
        public bool CheckedIn { get; set; }

        public virtual Event Event { get; set; }
    }
}
