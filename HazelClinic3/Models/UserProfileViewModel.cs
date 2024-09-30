using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HazelClinic3.Models;

namespace HazelClinic3.Models
{
    public class UserProfileViewModel
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public IEnumerable<Booking> Bookings { get; set; }
        public List<AdoptionRequest> Adoptions { get; set; }
        public List<Volunteer> Volunteers { get; set; }
    }
}