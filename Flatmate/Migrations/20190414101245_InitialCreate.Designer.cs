﻿// <auto-generated />
using Flatmate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Flatmate.Migrations
{
    [DbContext(typeof(FlatmateContext))]
    [Migration("20190414101245_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Flatmate.Models.Expense", b =>
                {
                    b.Property<int>("ExpenseId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int>("ExpenseCategory");

                    b.Property<string>("ExpenseSubject");

                    b.Property<int>("InitiatorId");

                    b.Property<int>("TeamId");

                    b.Property<double>("Value");

                    b.HasKey("ExpenseId");

                    b.HasIndex("InitiatorId");

                    b.HasIndex("TeamId");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("Flatmate.Models.ExpenseDebitor", b =>
                {
                    b.Property<int>("ExpenseId");

                    b.Property<int>("DebitorId");

                    b.HasKey("ExpenseId", "DebitorId");

                    b.HasIndex("DebitorId");

                    b.ToTable("ExpenseDebitor");
                });

            modelBuilder.Entity("Flatmate.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int>("InitiatorId");

                    b.Property<string>("OrderSubject");

                    b.Property<int>("TeamId");

                    b.HasKey("OrderId");

                    b.HasIndex("InitiatorId");

                    b.HasIndex("TeamId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Flatmate.Models.OrderDebitor", b =>
                {
                    b.Property<int>("OrderId");

                    b.Property<int>("DebitorId");

                    b.HasKey("OrderId", "DebitorId");

                    b.HasIndex("DebitorId");

                    b.ToTable("OrderDebitor");
                });

            modelBuilder.Entity("Flatmate.Models.RecurringBill", b =>
                {
                    b.Property<int>("RecurringBillId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BillSubject");

                    b.Property<double>("Frequency");

                    b.Property<int>("InitiatorId");

                    b.Property<DateTime>("LastEffectiveDate");

                    b.Property<DateTime>("NextEffectiveDate");

                    b.Property<int>("TeamId");

                    b.Property<double>("Value");

                    b.HasKey("RecurringBillId");

                    b.HasIndex("InitiatorId");

                    b.HasIndex("TeamId");

                    b.ToTable("RecurringBills");
                });

            modelBuilder.Entity("Flatmate.Models.RecurringBillDebitor", b =>
                {
                    b.Property<int>("RecurringBillId");

                    b.Property<int>("DebitorId");

                    b.HasKey("RecurringBillId", "DebitorId");

                    b.HasIndex("DebitorId");

                    b.ToTable("RecurringBillDebitor");
                });

            modelBuilder.Entity("Flatmate.Models.ScheduledEvent", b =>
                {
                    b.Property<int>("ScheduledEventId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Desription");

                    b.Property<int>("OwnerId");

                    b.Property<int>("TeamId");

                    b.Property<string>("Title");

                    b.HasKey("ScheduledEventId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("TeamId");

                    b.ToTable("ScheduledEvents");
                });

            modelBuilder.Entity("Flatmate.Models.ScheduledEventUser", b =>
                {
                    b.Property<int>("ScheduledEventId");

                    b.Property<int>("UserId");

                    b.HasKey("ScheduledEventId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ScheduledEventUser");
                });

            modelBuilder.Entity("Flatmate.Models.Team", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("TeamId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Flatmate.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EmailAddress");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Flatmate.Models.UserTeam", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("TeamId");

                    b.HasKey("UserId", "TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("UserTeam");
                });

            modelBuilder.Entity("Flatmate.Models.Expense", b =>
                {
                    b.HasOne("Flatmate.Models.User", "Initiator")
                        .WithMany("InitializedExpensesCollection")
                        .HasForeignKey("InitiatorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Flatmate.Models.Team", "Team")
                        .WithMany("ExpensesCollection")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Flatmate.Models.ExpenseDebitor", b =>
                {
                    b.HasOne("Flatmate.Models.User", "Debitor")
                        .WithMany("AttachedExpensesCollection")
                        .HasForeignKey("DebitorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Flatmate.Models.Expense", "Expense")
                        .WithMany("DebitorsCollection")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Flatmate.Models.Order", b =>
                {
                    b.HasOne("Flatmate.Models.User", "Initiator")
                        .WithMany("InitializedOrdersCollection")
                        .HasForeignKey("InitiatorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Flatmate.Models.Team", "Team")
                        .WithMany("OrdersCollection")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Flatmate.Models.OrderDebitor", b =>
                {
                    b.HasOne("Flatmate.Models.User", "Debitor")
                        .WithMany("AttachedOrdersCollection")
                        .HasForeignKey("DebitorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Flatmate.Models.Order", "Order")
                        .WithMany("DebitorsCollection")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Flatmate.Models.RecurringBill", b =>
                {
                    b.HasOne("Flatmate.Models.User", "Initiator")
                        .WithMany("InitializedRecurringBillsCollection")
                        .HasForeignKey("InitiatorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Flatmate.Models.Team", "Team")
                        .WithMany("RecurringBillsCollection")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Flatmate.Models.RecurringBillDebitor", b =>
                {
                    b.HasOne("Flatmate.Models.User", "Debitor")
                        .WithMany("AttachedRecurringBillsCollection")
                        .HasForeignKey("DebitorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Flatmate.Models.RecurringBill", "RecurringBill")
                        .WithMany("DebitorsCollection")
                        .HasForeignKey("RecurringBillId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Flatmate.Models.ScheduledEvent", b =>
                {
                    b.HasOne("Flatmate.Models.User", "Owner")
                        .WithMany("InitializedScheduledEvents")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Flatmate.Models.Team", "Team")
                        .WithMany("ScheduledEventsCollection")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Flatmate.Models.ScheduledEventUser", b =>
                {
                    b.HasOne("Flatmate.Models.ScheduledEvent", "ScheduledEvent")
                        .WithMany("AttachedUsersCollection")
                        .HasForeignKey("ScheduledEventId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Flatmate.Models.User", "User")
                        .WithMany("AttachedScheduledEvents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Flatmate.Models.UserTeam", b =>
                {
                    b.HasOne("Flatmate.Models.Team", "Team")
                        .WithMany("UsersCollection")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Flatmate.Models.User", "User")
                        .WithMany("TeamsCollection")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
