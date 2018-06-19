using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MathaCapital.Models
{
    public class BidBatchViewModel
    {
        public List<AuctionBid> bids;
        public SelectList batches;
        public SelectList banks;
        public List<WinResults> wins;
        public string bidBatch { get; set; }
        public string sortOrder { get; set; }
        public string bankName { get; set; }
        public string auctionType { get; set; }
            }
}
