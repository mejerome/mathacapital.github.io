using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MathaCapital.Data;
using MathaCapital.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal AmountBid { get; set; }

        [Display(Name = "Coupon Amount")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal CouponAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Pips { get; set; }

        public string BatchRef { get; set; }

        [Display(Name = "Imported By")]
        public string UserName { get; set; }
        public MathaCapitalUser User { get; set; }

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
