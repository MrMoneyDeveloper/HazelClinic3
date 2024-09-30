using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class AuctionItem
    {
        public int Id { get; set; }


        public string Name { get; set; }
        public byte[] Image { get; set; }

        public int Event_Id { get; set; }
        public virtual Event Event { get; set; }
        public DateTime AuctionEndTime { get; set; }
        public bool IsEmailSent { get; set; }
    }
}