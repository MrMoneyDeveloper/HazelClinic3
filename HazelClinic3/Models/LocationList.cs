using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HazelClinic3.Models
{
    public class Location
    {
        [Key]  // Primary Key
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Foreign Key to User1
        [ForeignKey("User")]
        public int UserId { get; set; }

        // User email
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        [RegularExpression(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,4}\b", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        // Navigation Property
        public virtual User1 User { get; set; }

        // To store the QR code string or URL
        public string QRCode { get; set; }

        // New fields to track the status of the item
        public bool IsReadyForPickup { get; set; } = false;  // Default to false
        public bool IsPickedUp { get; set; } = false;  // Default to false
    }
}
