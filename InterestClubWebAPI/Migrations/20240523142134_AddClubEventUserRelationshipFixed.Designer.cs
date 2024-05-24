﻿// <auto-generated />
using System;
using InterestClubWebAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InterestClubWebAPI.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240523142134_AddClubEventUserRelationshipFixed")]
    partial class AddClubEventUserRelationshipFixed
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("InterestClubWebAPI.Models.Club", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.ClubEvent", b =>
                {
                    b.Property<Guid>("ClubId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.HasKey("ClubId", "EventId");

                    b.HasIndex("EventId");

                    b.ToTable("ClubEvents");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.EventMember", b =>
                {
                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("EventId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("EventMembers");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Fatherland")
                        .HasColumnType("text");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.UserClub", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ClubId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "ClubId");

                    b.HasIndex("ClubId");

                    b.ToTable("UserClubs");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.ClubEvent", b =>
                {
                    b.HasOne("InterestClubWebAPI.Models.Club", "Club")
                        .WithMany("ClubEvents")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterestClubWebAPI.Models.Event", "Event")
                        .WithMany("ClubEvents")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.EventMember", b =>
                {
                    b.HasOne("InterestClubWebAPI.Models.Event", "Event")
                        .WithMany("EventMembers")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterestClubWebAPI.Models.User", "User")
                        .WithMany("EventMembers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.UserClub", b =>
                {
                    b.HasOne("InterestClubWebAPI.Models.Club", "Club")
                        .WithMany("UserClubs")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterestClubWebAPI.Models.User", "User")
                        .WithMany("UserClubs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");

                    b.Navigation("User");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.Club", b =>
                {
                    b.Navigation("ClubEvents");

                    b.Navigation("UserClubs");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.Event", b =>
                {
                    b.Navigation("ClubEvents");

                    b.Navigation("EventMembers");
                });

            modelBuilder.Entity("InterestClubWebAPI.Models.User", b =>
                {
                    b.Navigation("EventMembers");

                    b.Navigation("UserClubs");
                });
#pragma warning restore 612, 618
        }
    }
}
