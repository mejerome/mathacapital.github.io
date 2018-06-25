using MathaCapital.Models;
using Microsoft.EntityFrameworkCore;

namespace MathaCapital.Data
{
    public class AuctionContext : DbContext
    {
        public AuctionContext(DbContextOptions<AuctionContext> options) : base(options)
        {

        }

        public DbSet<AuctionBid> AuctionBids { get; set; }
        public DbSet<WinResults> WinResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuctionBid>().ToTable("AuctionBid");
            modelBuilder.Entity<WinResults>().ToTable("WinResult");
        }

        public DbSet<MathaCapital.Models.BankPercent> BankPercent { get; set; }
        public DbSet<MathaCapital.Models.BankPercViewModel> BankPercViewModel { get; set; }


     }

}
