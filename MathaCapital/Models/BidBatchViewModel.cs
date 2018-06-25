using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public class BankPercent
    {
        public int ID { get; set; }
        public string BankName { get; set; }
        public string Percent { get; set; }
    }

    public class BankPercViewModel
    {
        public BankPercViewModel()
        {
            perbank = new List<BankPercent>();
        }
        public int ID { get; set; }
        public string bidBatch { get; set; }
        public List<BankPercent> perbank { set; get; }
    }
}
