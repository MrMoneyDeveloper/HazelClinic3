using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HazelClinic3.Models
{
    public class PetSitting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SittingId { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter your full name:")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter your cellphone number:")]
        [MinLength(10)]
        [StringLength(10)]
        public string CellNo { get; set; }
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter emergency contacts full name:")]
        public string EmergencyContactName { get; set; }
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter emergency contacts cellphone number:")]
        [MinLength(10)]
        [StringLength(10)]
        public string EmergencyContactCellNo { get; set; }
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter your residential address:")]
        [StringLength(30)]
        public string ResAddress { get; set; }
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Enter your pets name:")]
        [StringLength(15)]
        public string PetName { get; set; }
        public string PetType { get; set; }

        [Required(ErrorMessage = "Select a date")]
        [Display(Name = "Select start date:")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Select a date")]
        [Display(Name = "Select end date:")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Display(Name = "Enter pets special requests, e.g. dietary restrictions, allergies, medications, behavioral issues.")] public string SpecialRequests { get; set; }
    }
}