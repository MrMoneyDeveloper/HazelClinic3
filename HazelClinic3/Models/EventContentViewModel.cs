using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HazelClinic3.Models
{
    public class EventContentViewModel
    {
        public IEnumerable<AuctionItem> AuctionItems { get; set; }
        public IEnumerable<EventDocument> Documents { get; set; }
    }
}