using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class AdoptionRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Adopterinterested { get; set; }

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

        [Display(Name = "Appointment Date")]
        [Required(ErrorMessage = "Please enter the appointment date.")]
        [DataType(DataType.Date)]
        public DateTime AppointDate { get; set; }

        [Display(Name = "Appointment Time")]
        [Required(ErrorMessage = "Please enter the appointment time.")]
        [DataType(DataType.Time)]
        public TimeSpan AppointTime { get; set; }



        [Display(Name = "Address")]
        public string Address { get; set; }

        public string AdopterMessage { get; set; }


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

        public AdoptionStatus TrackingStatus { get; set; }
        public bool IsInspectionComplete { get; set; }

        
        [DataType(DataType.Date)]
        public DateTime? InspectionDate { get; set; }

        public DateTime SubmittedDate { get; set; }


    }
    public enum AdoptionStatus
    {
        Submitted,
        AwaitingInspectorFeedback,
        Approved,
        OnDelivery,
        Delivered,
        Declined
    }
}