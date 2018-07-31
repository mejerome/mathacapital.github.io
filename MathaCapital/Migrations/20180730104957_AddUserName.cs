using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MathaCapital.Migrations
{
    public partial class AddUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WinAmount",
                table: "WinResult",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "CouponAmount",
                table: "WinResult",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBid",
                table: "WinResult",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WinResult",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "WinResult",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Pips",
                table: "AuctionBid",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "CouponAmount",
                table: "AuctionBid",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBid",
                table: "AuctionBid",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AuctionBid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "AuctionBid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BankPercViewModel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    bidBatch = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankPercViewModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MathaCapitalUser",
                columns: table => new
                {
                    AccessFailedCount = table.Column<int>(nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MathaCapitalUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankPercent",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BankName = table.Column<string>(nullable: true),
                    Percent = table.Column<string>(nullable: true),
                    BankPercViewModelID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankPercent", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BankPercent_BankPercViewModel_BankPercViewModelID",
                        column: x => x.BankPercViewModelID,
                        principalTable: "BankPercViewModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WinResult_UserId",
                table: "WinResult",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionBid_UserId",
                table: "AuctionBid",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BankPercent_BankPercViewModelID",
                table: "BankPercent",
                column: "BankPercViewModelID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionBid_MathaCapitalUser_UserId",
                table: "AuctionBid",
                column: "UserId",
                principalTable: "MathaCapitalUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WinResult_MathaCapitalUser_UserId",
                table: "WinResult",
                column: "UserId",
                principalTable: "MathaCapitalUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionBid_MathaCapitalUser_UserId",
                table: "AuctionBid");

            migrationBuilder.DropForeignKey(
                name: "FK_WinResult_MathaCapitalUser_UserId",
                table: "WinResult");

            migrationBuilder.DropTable(
                name: "BankPercent");

            migrationBuilder.DropTable(
                name: "MathaCapitalUser");

            migrationBuilder.DropTable(
                name: "BankPercViewModel");

            migrationBuilder.DropIndex(
                name: "IX_WinResult_UserId",
                table: "WinResult");

            migrationBuilder.DropIndex(
                name: "IX_AuctionBid_UserId",
                table: "AuctionBid");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WinResult");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "WinResult");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AuctionBid");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "AuctionBid");

            migrationBuilder.AlterColumn<decimal>(
                name: "WinAmount",
                table: "WinResult",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CouponAmount",
                table: "WinResult",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBid",
                table: "WinResult",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Pips",
                table: "AuctionBid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CouponAmount",
                table: "AuctionBid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBid",
                table: "AuctionBid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
