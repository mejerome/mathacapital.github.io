using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MathaCapital.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuctionBid",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AmountBid = table.Column<decimal>(nullable: false),
                    BankName = table.Column<string>(nullable: true),
                    BatchRef = table.Column<string>(nullable: true),
                    CouponAmount = table.Column<decimal>(nullable: false),
                    FwdDate = table.Column<DateTime>(nullable: false),
                    FwdRate = table.Column<double>(nullable: false),
                    Pips = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionBid", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionBid");
        }
    }
}
