using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MathaCapital.Data;

namespace MathaCapital.Models
{
    public class AuctionBid
    {
        public int ID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FwdDate { get; set; }
        [Display(Name ="Bank Name")]
        public string BankName { get; set; }
        [Display(Name ="Forward Rate")]
        public double FwdRate { get; set; }
        [Display(Name = "Amount Bid")]
        public decimal AmountBid { get; set; }
        [Display(Name = "Coupon Amount")]
        public decimal CouponAmount { get; set; }
        public decimal Pips { get; set; }
        public string BatchRef { get; set; }

        public static implicit operator AuctionBid(AuctionContext v)
        {
            throw new NotImplementedException();
        }

        internal void SaveChanges()
        {
            throw new NotImplementedException();
        }

        internal void AddRange(List<AuctionBid> auctionBids)
        {
            throw new NotImplementedException();
        }
    }
}
