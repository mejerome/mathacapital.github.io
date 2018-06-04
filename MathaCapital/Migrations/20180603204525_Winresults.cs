using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MathaCapital.Migrations
{
    public partial class Winresults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WinResult",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AmountBid = table.Column<decimal>(nullable: false),
                    AuctionBidID = table.Column<int>(nullable: false),
                    BankName = table.Column<string>(nullable: true),
                    CouponAmount = table.Column<decimal>(nullable: false),
                    FwdDate = table.Column<DateTime>(nullable: false),
                    FwdRate = table.Column<double>(nullable: false),
                    WinAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WinResult", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WinResult_AuctionBid_AuctionBidID",
                        column: x => x.AuctionBidID,
                        principalTable: "AuctionBid",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WinResult_AuctionBidID",
                table: "WinResult",
                column: "AuctionBidID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WinResult");
        }
    }
}
