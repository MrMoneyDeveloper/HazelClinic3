using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class DashboardViewModel
    {
        public List<User1> Pets { get; set; }
        public string PetName { get; internal set; }
        public string Email { get; internal set; }
       
    }
    
}