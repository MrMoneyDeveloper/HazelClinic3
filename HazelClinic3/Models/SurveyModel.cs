using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class SurveyModel
    {
        [Required]
        [EmailAddress]
        [Key]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required]
        [Range(1, 5)]
        [Display(Name = "Engagement Rating")]
        public int ERating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Venue Rating")]
        public int VRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Likelihood of Returning")]
        public int RRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Overall Rating")]
        public int ORating { get; set; }
    }
}