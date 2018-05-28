﻿// <auto-generated />
using MathaCapital.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace MathaCapital.Migrations
{
    [DbContext(typeof(AuctionContext))]
    [Migration("20180524202630_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MathaCapital.Models.AuctionBid", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AmountBid");

                    b.Property<string>("BankName");

                    b.Property<string>("BatchRef");

                    b.Property<decimal>("CouponAmount");

                    b.Property<DateTime>("FwdDate");

                    b.Property<double>("FwdRate");

                    b.Property<decimal>("Pips");

                    b.HasKey("ID");

                    b.ToTable("AuctionBid");
                });
#pragma warning restore 612, 618
        }
    }
}
