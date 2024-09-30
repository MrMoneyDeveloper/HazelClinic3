using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        [StringLength(100)]
        public string EventName { get; set; }

        [Required]
        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        [Display(Name = "Event Time")]
        [DataType(DataType.Time)]
        public TimeSpan EventTime { get; set; }

        [Required]
        [Display(Name = "Attendee Limit")]
        [Range(1, 1000)]
        public int LimitOfAttendees { get; set; }

        [Required]
        [Display(Name = "Price R :")]
        [Range(1, 10000)]
        public decimal EventPrice { get; set; }

        [Required]
        [Display(Name = "Pet Preference")]
        public bool ArePetsAllowed { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Location")]
        public string EventLocation { get; set; }

        public byte[] Image { get; set; }

        public virtual ICollection<AuctionItem> AuctionItems { get; set; }
        public virtual ICollection<EventDocument> EventDocuments { get; set; }

    }




}