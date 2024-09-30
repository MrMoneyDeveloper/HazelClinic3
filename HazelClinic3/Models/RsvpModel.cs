using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace HazelClinic3.Models
{
    public class RsvpModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Rsvpno { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please enter your full name.")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please enter your email address.")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$",
        ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }

        [Display(Name = "RSVP for Pet Talent Show")]
        public bool PetTalentShow { get; set; }

        [Display(Name = "RSVP for Pet Party")]
        public bool PetParty { get; set; }

        [Display(Name = "RSVP for Charity Auction")]
        public bool CharityAuction { get; set; }
    }
}
