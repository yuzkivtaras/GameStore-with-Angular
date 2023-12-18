﻿// <auto-generated />
using System;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(GameStoreDbContext))]
    [Migration("20231211073538_5")]
    partial class _5
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DataAccessLayer.Entities.Customer", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Game", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Discontinued")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Price")
                        .IsRequired()
                        .HasColumnType("decimal(5,2)");

                    b.Property<string>("PublisherId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("UnitInStock")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("Key");

                    b.HasIndex("PublisherId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.GameGenre", b =>
                {
                    b.Property<string>("GamesKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("GenresId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GamesKey", "GenresId");

                    b.HasIndex("GenresId");

                    b.ToTable("GameGenres");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.GamePlatform", b =>
                {
                    b.Property<string>("GamesKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PlatformsId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GamesKey", "PlatformsId");

                    b.HasIndex("PlatformsId");

                    b.ToTable("GamePlatforms");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Genre", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentGenreId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ParentGenreId");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Order", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PaidDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.OrderDetail", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Platform", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Platforms");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Publisher", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomePage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Game", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.Publisher", "Publisher")
                        .WithMany("Games")
                        .HasForeignKey("PublisherId");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.GameGenre", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.Game", "Game")
                        .WithMany("GameGenres")
                        .HasForeignKey("GamesKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entities.Genre", "Genre")
                        .WithMany("GameGenres")
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.GamePlatform", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.Game", "Game")
                        .WithMany("GamePlatforms")
                        .HasForeignKey("GamesKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entities.Platform", "Platform")
                        .WithMany("GamePlatforms")
                        .HasForeignKey("PlatformsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Platform");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Genre", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.Genre", "ParentGenre")
                        .WithMany()
                        .HasForeignKey("ParentGenreId");

                    b.Navigation("ParentGenre");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Order", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.OrderDetail", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Game", b =>
                {
                    b.Navigation("GameGenres");

                    b.Navigation("GamePlatforms");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Genre", b =>
                {
                    b.Navigation("GameGenres");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Platform", b =>
                {
                    b.Navigation("GamePlatforms");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Publisher", b =>
                {
                    b.Navigation("Games");
                });
#pragma warning restore 612, 618
        }
    }
}
