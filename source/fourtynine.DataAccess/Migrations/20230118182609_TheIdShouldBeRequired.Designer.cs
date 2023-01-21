﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using fourtynine.DataAccess;

#nullable disable

namespace fourtynine.DataAccess.Migrations
{
    [DbContext(typeof(DbContext))]
    [Migration("20230118182609_TheIdShouldBeRequired")]
    partial class TheIdShouldBeRequired
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("fourtynine.DataAccess.AllowedAuthenticationScheme", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SchemeName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ProviderUserId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId", "SchemeName");

                    b.ToTable("AllowedAuthenticationScheme");
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationRoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUserLogin", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUserToken", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("fourtynine.DataAccess.Picture", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("PostingId")
                        .HasColumnType("bigint");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PostingId");

                    b.ToTable("Picture");
                });

            modelBuilder.Entity("fourtynine.DataAccess.Posting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<string>("ThumbnailUrl")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Postings");
                });

            modelBuilder.Entity("fourtynine.DataAccess.AllowedAuthenticationScheme", b =>
                {
                    b.HasOne("fourtynine.DataAccess.ApplicationUser", null)
                        .WithMany("AllowedAuthenticationSchemes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationRoleClaim", b =>
                {
                    b.HasOne("fourtynine.DataAccess.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUserClaim", b =>
                {
                    b.HasOne("fourtynine.DataAccess.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUserLogin", b =>
                {
                    b.HasOne("fourtynine.DataAccess.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUserRole", b =>
                {
                    b.HasOne("fourtynine.DataAccess.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("fourtynine.DataAccess.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUserToken", b =>
                {
                    b.HasOne("fourtynine.DataAccess.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("fourtynine.DataAccess.Picture", b =>
                {
                    b.HasOne("fourtynine.DataAccess.Posting", "Posting")
                        .WithMany("Pictures")
                        .HasForeignKey("PostingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Posting");
                });

            modelBuilder.Entity("fourtynine.DataAccess.Posting", b =>
                {
                    b.HasOne("fourtynine.DataAccess.ApplicationUser", "Author")
                        .WithMany("Postings")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("fourtynine.DataAccess.LocationPostingDetails", "Location", b1 =>
                        {
                            b1.Property<long>("PostingId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Address")
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("City")
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<double?>("Latitude")
                                .HasColumnType("float");

                            b1.Property<double?>("Longitude")
                                .HasColumnType("float");

                            b1.HasKey("PostingId");

                            b1.ToTable("Postings");

                            b1.WithOwner()
                                .HasForeignKey("PostingId");
                        });

                    b.OwnsOne("fourtynine.DataAccess.PricingPostingDetails", "Pricing", b1 =>
                        {
                            b1.Property<long>("PostingId")
                                .HasColumnType("bigint");

                            b1.Property<byte>("BargainKinds")
                                .HasColumnType("tinyint");

                            b1.Property<decimal?>("Price")
                                .HasColumnType("money");

                            b1.Property<decimal?>("PriceMax")
                                .HasColumnType("money");

                            b1.HasKey("PostingId");

                            b1.ToTable("Postings");

                            b1.WithOwner()
                                .HasForeignKey("PostingId");
                        });

                    b.OwnsOne("fourtynine.DataAccess.RealEstatePostingDetails", "RealEstate", b1 =>
                        {
                            b1.Property<long>("PostingId")
                                .HasColumnType("bigint");

                            b1.Property<decimal>("Area")
                                .HasColumnType("decimal(8,1)");

                            b1.Property<string>("Kind")
                                .IsRequired()
                                .HasColumnType("varchar(16)");

                            b1.Property<int>("Rooms")
                                .HasColumnType("int");

                            b1.Property<string>("SpacePurpose")
                                .IsRequired()
                                .HasColumnType("varchar(16)");

                            b1.HasKey("PostingId");

                            b1.ToTable("Postings");

                            b1.WithOwner()
                                .HasForeignKey("PostingId");
                        });

                    b.OwnsOne("fourtynine.DataAccess.VehiclePostingDetails", "Vehicle", b1 =>
                        {
                            b1.Property<long>("PostingId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Manufacturer")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("Model")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<int>("Year")
                                .HasColumnType("int");

                            b1.HasKey("PostingId");

                            b1.ToTable("Postings");

                            b1.WithOwner()
                                .HasForeignKey("PostingId");
                        });

                    b.Navigation("Author");

                    b.Navigation("Location");

                    b.Navigation("Pricing");

                    b.Navigation("RealEstate");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("fourtynine.DataAccess.ApplicationUser", b =>
                {
                    b.Navigation("AllowedAuthenticationSchemes");

                    b.Navigation("Postings");
                });

            modelBuilder.Entity("fourtynine.DataAccess.Posting", b =>
                {
                    b.Navigation("Pictures");
                });
#pragma warning restore 612, 618
        }
    }
}
