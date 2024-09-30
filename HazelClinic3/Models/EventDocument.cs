using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class EventDocument
    {
        public int Id { get; set; }

        
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }

        public int Event_Id { get; set; }
        public virtual Event Event { get; set; }
    }
}