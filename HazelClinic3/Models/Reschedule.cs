using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class Reschedule
    {

        [Key]
        public int AppointmentId { get; set; }

        
        [Display(Name = "ID Number:")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ID number must be exactly 13 digits.")]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "Select appointment date")]
        [Display(Name = "Select Appointment date:")]
        [DataType(DataType.Date)]
        public DateTime AppoinmentDate { get; set; }

        [Required(ErrorMessage = "Select appointment time")]
        [Display(Name = "Select Appointment Time:")]
        [DataType(DataType.Time)]
        public DateTime AppointmentTime { get; set; }

         
    [Display(Name = "OTP:")]
    public string OTP { get; set; }
    }
}