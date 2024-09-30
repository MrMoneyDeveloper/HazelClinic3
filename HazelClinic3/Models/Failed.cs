using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{

    public class Failed
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please enter your full name.")]
        public string AdopterFName { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string AdopterNo { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Please enter your email address.")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$",
         ErrorMessage = "Please enter a valid email address.")]
        public string AdopterEmail { get; set; }


        [Display(Name = "Customer Unavailable")]
        public bool CustomerUnavailable { get; set; }

        [Display(Name = "Not able to provide OTP")]
        public bool NotAbleToProvideOTP { get; set; }

        [Display(Name = "Not at Address")]
        public bool NotAtAddress { get; set; }

        [Display(Name = "Vehicle Breakdown")]
        public bool Breakdown { get; set; }


        [Display(Name = "Other")]
        public string OtherReason { get; set; }
    }
}



