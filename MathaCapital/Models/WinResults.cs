using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MathaCapital.Models
{
    public class WinResults
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FwdDate { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public AuctionBid AuctionBid { get; set; }
        public int AuctionBidID { get; set; }
        [Display(Name = "Forward Rate")]
        public double FwdRate { get; set; }

        [Display(Name = "Amount Bid")]
        [DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal AmountBid { get; set; }

        [Display(Name = "Coupon Amount")]
        [DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal CouponAmount { get; set; }

        [Display(Name = "Win Amount")]
        [DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal WinAmount { get; set; }

        public string BatchRef { get; set; }

    }

}
