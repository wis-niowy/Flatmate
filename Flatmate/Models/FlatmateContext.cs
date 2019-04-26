using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;
using Flatmate.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Flatmate.Models
{
    public class FlatmateContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        //public DbSet<UserTeam> UserTeam { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDebitor> OrderDebitor { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseDebitor> ExpenseDebitor { get; set; }
        public DbSet<RecurringBill> RecurringBills { get; set; }
        public DbSet<RecurringBillDebitor> RecurringBillDebitor { get; set; }
        public DbSet<ScheduledEvent> ScheduledEvents { get; set; }
        public DbSet<ScheduledEventUser> ScheduledEventUser { get; set; }

        public FlatmateContext(DbContextOptions<FlatmateContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // set primary keys for many to many junction tables
            //modelBuilder.Entity<UserTeam>().HasKey(x => new { x.UserId, x.TeamId });
            modelBuilder.Entity<OrderDebitor>().HasKey(x => new { x.OrderId, x.DebitorId });
            modelBuilder.Entity<ExpenseDebitor>().HasKey(x => new { x.ExpenseId, x.DebitorId });
            modelBuilder.Entity<RecurringBillDebitor>().HasKey(x => new { x.RecurringBillId, x.DebitorId });
            modelBuilder.Entity<ScheduledEventUser>().HasKey(x => new { x.ScheduledEventId, x.UserId });

            // set one to many relationships between tables
            modelBuilder.Entity<User>()
                .HasOne(pt => pt.Team)
                .WithMany(p => p.UsersCollection)
                .HasForeignKey(pt => pt.TeamId);
            modelBuilder.Entity<Order>()
                .HasOne(pt => pt.Initiator)
                .WithMany(p => p.InitializedOrdersCollection)
                .HasForeignKey(pt => pt.InitiatorId);
            //modelBuilder.Entity<Order>()
            //    .HasOne(pt => pt.Team)
            //    .WithMany(p => p.OrdersCollection)
            //    .HasForeignKey(pt => pt.TeamId);
            modelBuilder.Entity<Expense>()
                .HasOne(pt => pt.Initiator)
                .WithMany(p => p.InitializedExpensesCollection)
                .HasForeignKey(pt => pt.InitiatorId);
            //modelBuilder.Entity<Expense>()
            //    .HasOne(pt => pt.Team)
            //    .WithMany(p => p.ExpensesCollection)
            //    .HasForeignKey(pt => pt.TeamId);
            modelBuilder.Entity<RecurringBill>()
                .HasOne(pt => pt.Initiator)
                .WithMany(p => p.InitializedRecurringBillsCollection)
                .HasForeignKey(pt => pt.InitiatorId);
            //modelBuilder.Entity<RecurringBill>()
            //    .HasOne(pt => pt.Team)
            //    .WithMany(p => p.RecurringBillsCollection)
            //    .HasForeignKey(pt => pt.TeamId);
            //modelBuilder.Entity<ScheduledEvent>()
            //    .HasOne(pt => pt.Team)
            //    .WithMany(p => p.ScheduledEventsCollection)
            //    .HasForeignKey(pt => pt.TeamId);
            modelBuilder.Entity<ScheduledEvent>()
                .HasOne(pt => pt.Owner)
                .WithMany(p => p.InitializedScheduledEvents)
                .HasForeignKey(pt => pt.OwnerId);

            // set many to many relationships between tables
            //modelBuilder.Entity<UserTeam>()
            //    .HasOne(pt => pt.User)
            //    .WithMany(p => p.TeamsCollection)
            //    .HasForeignKey(pt => pt.UserId)
            //    .OnDelete(DeleteBehavior.Restrict); ;
            //modelBuilder.Entity<UserTeam>()
            //    .HasOne(pt => pt.Team)
            //    .WithMany(p => p.UsersCollection)
            //    .HasForeignKey(pt => pt.TeamId)
            //    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderDebitor>()
                .HasOne(pt => pt.Order)
                .WithMany(p => p.DebitorsCollection)
                .HasForeignKey(pt => pt.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderDebitor>()
                .HasOne(pt => pt.Debitor)
                .WithMany(p => p.AttachedOrdersCollection)
                .HasForeignKey(pt => pt.DebitorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpenseDebitor>()
                .HasOne(pt => pt.Expense)
                .WithMany(p => p.DebitorsCollection)
                .HasForeignKey(pt => pt.ExpenseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExpenseDebitor>()
                .HasOne(pt => pt.Debitor)
                .WithMany(p => p.AttachedExpensesCollection)
                .HasForeignKey(pt => pt.DebitorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringBillDebitor>()
                .HasOne(pt => pt.RecurringBill)
                .WithMany(p => p.DebitorsCollection)
                .HasForeignKey(pt => pt.RecurringBillId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringBillDebitor>()
                .HasOne(pt => pt.Debitor)
                .WithMany(p => p.AttachedRecurringBillsCollection)
                .HasForeignKey(pt => pt.DebitorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ScheduledEventUser>()
                .HasOne(pt => pt.ScheduledEvent)
                .WithMany(p => p.AttachedUsersCollection)
                .HasForeignKey(pt => pt.ScheduledEventId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ScheduledEventUser>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.AttachedScheduledEvents)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // set value related with money to decimal(30,2)
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) && p.Name == "Value"))
            {
                property.Relational().ColumnType = "decimal(18, 2)";
            }

            // column data types conversions
            modelBuilder
                .Entity<Expense>()
                .Property(e => e.ExpenseCategory)
                .HasConversion(new EnumToStringConverter<ExpenseCategory>());
            modelBuilder
                .Entity<Order>()
                .Property(e => e.ExpenseCategory)
                .HasConversion(new EnumToStringConverter<ExpenseCategory>());
            modelBuilder
                .Entity<RecurringBill>()
                .Property(e => e.ExpenseCategory)
                .HasConversion(new EnumToStringConverter<ExpenseCategory>());
            modelBuilder
                .Entity<RecurringBill>()
                .Property(e => e.Frequency)
                .HasConversion(new EnumToStringConverter<Frequency>());


            // exclude properties from being mapped to columns
            //modelBuilder.Entity<Expense>()
            //    .Ignore(ex => ex.Debitor);

            PopulateInMemroyDB(modelBuilder);
        }

        /// <summary>
        /// Method used to populate database (in memory) with some seed data
        /// </summary>
        private void PopulateInMemroyDB(ModelBuilder mb)
        {
            mb.Entity<User>().HasData(
                new User ()
                {
                    UserId = 1,
                    TeamId = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    EmailAddress = "jan.kow@poczta.pl"
                },
                new User
                {
                    UserId = 2,
                    TeamId = 1,
                    FirstName = "Grzegorz",
                    LastName = "Kaczmarski",
                    EmailAddress = "grz.kacz@poczta.pl"
                },
                new User
                {
                    UserId = 3,
                    TeamId = 1,
                    FirstName = "Maciej",
                    LastName = "Nowak",
                    EmailAddress = "mac.now@poczta.pl"
                },
                new User
                {
                    UserId = 4,
                    TeamId = 1,
                    FirstName = "Krystian",
                    LastName = "Adamowicz",
                    EmailAddress = "krys.adam@poczta.pl"
                });
            mb.Entity<Team>().HasData(
                new Team
                {
                    TeamId = 1,
                    Name = "Best Ekipa"
                });
            mb.Entity<Expense>().HasData(
                new Expense
                {
                    ExpenseId = 1,
                    InitiatorId = 1,
                    //TeamId = 1,
                    ExpenseSubject = "Expense 1",
                    Date = new DateTime(2019, 04, 18),
                    Value = 15.50,
                    ExpenseCategory = ExpenseCategory.Shopping
                },
                new Expense
                {
                    ExpenseId = 2,
                    InitiatorId = 2,
                    //TeamId = 1,
                    ExpenseSubject = "Expense 2",
                    Date = new DateTime(2019, 04, 18),
                    Value = 12.00,
                    ExpenseCategory = ExpenseCategory.Shopping
                },
                new Expense
                {
                    ExpenseId = 3,
                    InitiatorId = 3,
                    //TeamId = 1,
                    ExpenseSubject = "Expense 3",
                    Date = new DateTime(2019, 04, 18),
                    Value = 125.00,
                    ExpenseCategory = ExpenseCategory.Shopping
                },
                new Expense
                {
                    ExpenseId = 4,
                    InitiatorId = 4,
                    //TeamId = 1,
                    ExpenseSubject = "Expense 4",
                    Date = new DateTime(2019, 04, 18),
                    Value = 78.20,
                    ExpenseCategory = ExpenseCategory.Shopping
                });
            mb.Entity<ExpenseDebitor>().HasData(
                new ExpenseDebitor
                {
                    ExpenseId = 1,
                    DebitorId = 2
                },
                new ExpenseDebitor
                {
                    ExpenseId = 1,
                    DebitorId = 3
                },
                new ExpenseDebitor
                {
                    ExpenseId = 1,
                    DebitorId = 4
                },
                new ExpenseDebitor
                {
                    ExpenseId = 2,
                    DebitorId = 1
                },
                new ExpenseDebitor
                {
                    ExpenseId = 2,
                    DebitorId = 3
                },
                new ExpenseDebitor
                {
                    ExpenseId = 2,
                    DebitorId = 4
                },
                new ExpenseDebitor
                {
                    ExpenseId = 3,
                    DebitorId = 1
                },
                new ExpenseDebitor
                {
                    ExpenseId = 3,
                    DebitorId = 2
                },
                new ExpenseDebitor
                {
                    ExpenseId = 3,
                    DebitorId = 4
                },
                new ExpenseDebitor
                {
                    ExpenseId = 4,
                    DebitorId = 1
                },
                new ExpenseDebitor
                {
                    ExpenseId = 4,
                    DebitorId = 2
                },
                new ExpenseDebitor
                {
                    ExpenseId = 4,
                    DebitorId = 3
                });
            mb.Entity<ScheduledEvent>().HasData(
                new ScheduledEvent {
                    ScheduledEventId = 1,
                    OwnerId = 1,
                    Title = "Wydarzenie 1",
                    Desription = "Opis wydarzenia 1",
                    StartDate = new DateTime(2019, 04, 25,18,30,00),
                    EndDate = new DateTime(2019, 04, 25,23,00,00),
                    IsFullDay = false
                },
                new ScheduledEvent
                {
                    ScheduledEventId = 2,
                    OwnerId = 2,
                    Title = "Wydarzenie 2",
                    Desription = "Opis wydarzenia 2",
                    StartDate = new DateTime(2019, 04, 26, 18, 30, 00),
                    EndDate = new DateTime(2019, 04, 26, 23, 00, 00),
                    IsFullDay = false
                },
                new ScheduledEvent
                {
                    ScheduledEventId = 3,
                    OwnerId = 3,
                    Title = "Wydarzenie 3",
                    Desription = "Opis wydarzenia 3",
                    StartDate = new DateTime(2019, 04, 27, 18, 30, 00),
                    EndDate = new DateTime(2019, 04, 27, 23, 00, 00),
                    IsFullDay = false
                });
            mb.Entity<ScheduledEventUser>().HasData(
                new ScheduledEventUser
                {
                    ScheduledEventId = 1,
                    UserId = 2
                },
                new ScheduledEventUser
                {
                    ScheduledEventId = 1,
                    UserId = 3
                },
                new ScheduledEventUser
                {
                    ScheduledEventId = 2,
                    UserId = 3
                },
                new ScheduledEventUser
                {
                    ScheduledEventId = 2,
                    UserId = 1
                },
                new ScheduledEventUser
                {
                    ScheduledEventId = 3,
                    UserId = 1
                });
        }
        //private void PopulateInMemroyDB()
        //{
        //    this.Users.Add(new User
        //    {
        //        UserId = 1,
        //        FirstName = "Jan",
        //        LastName = "Kowalski",
        //        EmailAddress = "jan.kow@poczta.pl"
        //    });
        //    this.Users.Add(new User
        //    {
        //        UserId = 2,
        //        FirstName = "Grzegorz",
        //        LastName = "Kaczmarski",
        //        EmailAddress = "grz.kacz@poczta.pl"
        //    });
        //    this.Users.Add(new User
        //    {
        //        UserId = 3,
        //        FirstName = "Maciej",
        //        LastName = "Nowak",
        //        EmailAddress = "mac.now@poczta.pl"
        //    });
        //    this.Users.Add(new User
        //    {
        //        UserId = 4,
        //        FirstName = "Krystian",
        //        LastName = "Adamowicz",
        //        EmailAddress = "krys.adam@poczta.pl"
        //    });
        //    this.Expenses.Add(new Expense
        //    {
        //        ExpenseId = 1,
        //        InitiatorId = 1,
        //        TeamId = 1,
        //        ExpenseSubject = "Expense 1",
        //        Date = new DateTime(2019,04,18),
        //        Value = 15.50,
        //        ExpenseCategory = 1
        //    });
        //    this.Expenses.Add(new Expense
        //    {
        //        ExpenseId = 2,
        //        InitiatorId = 2,
        //        TeamId = 1,
        //        ExpenseSubject = "Expense 2",
        //        Date = new DateTime(2019, 04, 18),
        //        Value = 12.00,
        //        ExpenseCategory = 2
        //    });
        //    this.Expenses.Add(new Expense
        //    {
        //        ExpenseId = 3,
        //        InitiatorId = 3,
        //        TeamId = 1,
        //        ExpenseSubject = "Expense 3",
        //        Date = new DateTime(2019, 04, 18),
        //        Value = 125.00,
        //        ExpenseCategory = 1
        //    });
        //    this.Expenses.Add(new Expense
        //    {
        //        ExpenseId = 4,
        //        InitiatorId = 4,
        //        TeamId = 1,
        //        ExpenseSubject = "Expense 4",
        //        Date = new DateTime(2019, 04, 18),
        //        Value = 78.20,
        //        ExpenseCategory = 1
        //    });
        //    this.ExpenseDebitor.Add(new ExpenseDebitor
        //    {
        //        ExpenseId = 1,
        //        DebitorId = 2
        //    });
        //    this.ExpenseDebitor.Add(new ExpenseDebitor
        //    {
        //        ExpenseId = 1,
        //        DebitorId = 3
        //    });
        //    this.ExpenseDebitor.Add(new ExpenseDebitor
        //    {
        //        ExpenseId = 1,
        //        DebitorId = 4
        //    });
        //}
    }

    

}
