using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class RescheduleViewModel
    {
        [Required]
        [Display(Name = "Booking ID:")]
        public int BookingId { get; set; }


        [Required(ErrorMessage = "Please Enter Your First Name")]
        [Display(Name = "First Name:")]
        public string Fname { get; set; }

       

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Enter Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Enter End Date")]
        public DateTime  EndDate { get; set; }
    }
}