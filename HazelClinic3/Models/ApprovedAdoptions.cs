using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class ApprovedAdoptions
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Adopterinterested { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please enter your full name.")]
        public string AdopterFName { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string AdopterNo { get; set; }

        [Display(Name = "Email Address ")]
        [Required(ErrorMessage = "Please enter your email address.")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$",
            ErrorMessage = "Please enter a valid email address.")]
        public string AdopterEmail { get; set; }

        [Display(Name = "Any Special Requests")]
        public string AdopterMessage { get; set; }


        [Required(ErrorMessage = "Please Enter Your Residential Address")]
        [Display(Name = "Address")]
        public string Address { get; set; }



        public int InspectorId { get; set; }

        public string Status { get; set; }

        [Display(Name = "Safety Checked")]
        public bool SafetyChecked { get; set; }

        [Display(Name = "Cleanliness Checked")]
        public bool CleanlinessChecked { get; set; }

        [Display(Name = "Adequate Pet Living Space")]
        public bool SpaceChecked { get; set; }

        [Display(Name = "Pet Provisions Available")]
        public bool ProvisionsChecked { get; set; }

        [Display(Name = "Interactive Family")]
        public bool InteractiveFamilyChecked { get; set; }

        [Display(Name = "Fenced Property")]
        public bool FencedChecked { get; set; }




        [NotMapped]
        [Display(Name = "Delivery Date")] 
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

    }
}