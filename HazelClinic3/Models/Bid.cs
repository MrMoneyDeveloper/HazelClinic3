using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public int AuctionItemId { get; set; }
        public virtual AuctionItem AuctionItem { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
        public string UserEmail { get; set; }
    }
}