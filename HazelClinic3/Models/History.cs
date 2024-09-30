using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HazelClinic3.Models
{
    public class History
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string Details { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required(ErrorMessage = "ID number is required")]
        [Display(Name = "ID Number:")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ID number must be exactly 13 digits.")]
        public string IdNumber { get; set; }

    }
}
