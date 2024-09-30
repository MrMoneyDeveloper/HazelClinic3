using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HazelClinic3.Models
{
    public class Grooming
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int GroomingId { get; set; }

        [Required(ErrorMessage = "Owner name is required")]
        [Display(Name = " Pet owner's full name:")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Street address is required")]
        [Display(Name = " Street address:")]
        public string Address { get; set; }


        [Required(ErrorMessage = "City is required")]
        [Display(Name = " City:")]
        public string City { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [Display(Name = " Postal code:")]
        public string PostalCode { get; set; }


        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number:")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pet name is required")]
        [Display(Name = "Pet's Name:")]
        public string PetName { get; set; }

        [Required(ErrorMessage = "Pet type is required")]
        [Display(Name = "Select pet type:")]
        public string PetType { get; set; }

        [Required(ErrorMessage = "Pet breed is required")]
        [Display(Name = "Enter pet breed:")]
        public string Breed { get; set; }


        [Required(ErrorMessage = "Pet hair length is required")]
        [Display(Name = "Select pet hair length:")]
        public string HairLength { get; set; }

        [Display(Name = "Medical Issues:")]
        public string MedicalIssues { get; set; }

        [Required(ErrorMessage = "Select appointment date ")]
        [DataType(DataType.Date)]
        public DateTime? GroomingAppoinmentDate { get; set; }








    }
}