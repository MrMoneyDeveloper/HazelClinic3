using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class OngoingDriver
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "Full Name")]
        public string AdopterFName { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string AdopterNo { get; set; }

        [Display(Name = "Email Address ")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$",
         ErrorMessage = "Please enter a valid email address.")]
        public string AdopterEmail { get; set; }

        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        public int RescheduleCount { get; set; }

    }
}