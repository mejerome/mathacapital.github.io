using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace MathaCapital.Models
{
    public class BidBatchViewModel
    {
        public List<AuctionBid> bids;
        public SelectList batches;
        public string bidBatch { get; set; }

    }


}
