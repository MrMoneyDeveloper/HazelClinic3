using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HazelClinic3.Models
{
    public class Check
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PetId { get; set; }

        [DisplayName("Please Enter Booking ID:")]
        [Required(ErrorMessage = "Enter a valid Booking ID")]
        public int BookId { get; set; }
        

        [DisplayName("First Name:")]
        [Required(ErrorMessage = "Please enter a first name.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:")]
        [Required(ErrorMessage = "Please enter a last name.")]
        public string LastName { get; set; }

        [DisplayName("Contact Number:")]
        [Required(ErrorMessage = "Please enter a contact number.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must be 10 digits.")]
        public string ContactNumber { get; set; }

        [DisplayName("Emergency Contact Number:")]
        [Required(ErrorMessage = "Please enter an emergency contact number.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Emergency contact number must be 10 digits.")]
        public string EmergencyContactNumber { get; set; }

        [DisplayName("Kennel ID:")]
        [Required]
        public string KennelId { get; set; }




    }
}
