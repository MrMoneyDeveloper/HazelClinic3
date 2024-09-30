using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class ReturnPolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReturnRequestID { get; set; }

        [Required]
        public int AdoptionID { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        public DateTime ScheduledReturnDate { get; set; }

        [Required]
        [StringLength(20)]
        public string ReturnStatus { get; set; }

    }

}