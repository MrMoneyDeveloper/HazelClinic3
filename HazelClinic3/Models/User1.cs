using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class User1
    {
        [Key]
        public int UserId { get; set; }

        [Display(Name = "Second ID")]
        public string IDnum { get; set; }

        [Required]
        [Display(Name = "Pet Name")]

        public string Username { get; set; }


        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]

        public string Password { get; set; }


        [Required]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        [RegularExpression(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,4}\b", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Cell Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Cell Number must be a 10-digit number.")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "City/Postal Code")]
        public string PostalCode { get; set; }

    }
}