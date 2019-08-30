using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;
using Flatmate.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Flatmate.Data
{
    public class FlatmateContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserTeam> UserPerTeams { get; set; }
        public DbSet<SingleComplexOrder> ComplexOrders { get; set; }
        public DbSet<SCOUserTeamAssignment> OrdersAssignments { get; set; }
        public DbSet<SingleOrderElement> OrderElements { get; set; }
        public DbSet<TotalExpense> TotalExpenses { get; set; }
        public DbSet<PartialExpense> PartialExpenses { get; set; }
        public DbSet<RecurringBill> RecurringBills { get; set; }
        public DbSet<RecurringBillPerTeamMember> RecurringBillAssignments { get; set; }
        public DbSet<ScheduledEvent> ScheduledEvents { get; set; }
        public DbSet<ScheduledEventUser> ScheduledEventUsers { get; set; }

        public FlatmateContext(DbContextOptions<FlatmateContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configuring relationships
            ConfigureUserTeamRelations(modelBuilder);
            ConfigureOrderRelations(modelBuilder);
            ConfigureRecurringBillRelations(modelBuilder);
            ConfigureExpenseRelations(modelBuilder);
            ConfigureEventRelations(modelBuilder);
            ConfigureUserexpensesRealtions(modelBuilder);

            //Configuring enum mapping
            ConfigureEnumMapping(modelBuilder);

            //Configuring database for money types
            ConfigureMoneyColumnTypes(modelBuilder);
        }
        private static void ConfigureUserTeamRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTeam>()
                .HasKey(x => new { x.UserId, x.TeamId });
            modelBuilder.Entity<UserTeam>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.TeamAssignments)
                .HasForeignKey(ut => ut.UserId);
            modelBuilder.Entity<UserTeam>()
                .HasOne(ut => ut.Team)
                .WithMany(t => t.UserAssignments)
                .HasForeignKey(ut => ut.TeamId);
        }
        private static void ConfigureOrderRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SCOUserTeamAssignment>()
                .HasKey(sa => new { sa.SCOId, sa.UserId, sa.TeamId });
            modelBuilder.Entity<SCOUserTeamAssignment>()
                .HasOne(sa => sa.SCO)
                .WithMany(sco => sco.SCOTeamMemberAssignments)
                .HasForeignKey(sa => sa.SCOId);
            modelBuilder.Entity<SCOUserTeamAssignment>()
                .HasOne(sa => sa.UserTeam)
                .WithMany(ut => ut.SCOUserTeamAssignments)
                .HasForeignKey(sa => new { sa.UserId, sa.TeamId });

            modelBuilder.Entity<SingleOrderElement>()
                .HasOne(soe => soe.SingleComplexOrder)
                .WithMany(sco => sco.OrderElements)
                .HasForeignKey(soe => soe.SCOId);
        }
        private static void ConfigureRecurringBillRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecurringBillPerTeamMember>()
                .HasKey(rbp => new { rbp.RecurringBillId, rbp.UserId, rbp.TeamId });
            modelBuilder.Entity<RecurringBillPerTeamMember>()
                .HasOne(rbp => rbp.RecurringBill)
                .WithMany(rb => rb.RecipientsCollection)
                .HasForeignKey(rbp => rbp.RecurringBillId);
            modelBuilder.Entity<RecurringBillPerTeamMember>()
                .HasOne(rbp => rbp.TeamMemberAssignment)
                .WithMany(ut => ut.RecurringBillPerTeamMembers)
                .HasForeignKey(rbp => new { rbp.UserId, rbp.TeamId });
        }
        private static void ConfigureExpenseRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartialExpense>()
                .HasKey(pe => new { pe.TotalExpenseId, pe.UserId, pe.TeamId });
            modelBuilder.Entity<PartialExpense>()
                .HasOne(pe => pe.TotalExpense)
                .WithMany(ut => ut.PartialExpenses)
                .HasForeignKey(pe => new { pe.TotalExpenseId });
            modelBuilder.Entity<PartialExpense>()
                .HasOne(pe => pe.TeamMemberAssignment)
                .WithMany(ut => ut.PartialExpenses)
                .HasForeignKey(pe => new { pe.UserId, pe.TeamId });
        }
        private static void ConfigureEventRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduledEventUser>()
                .HasKey(seu => new { seu.ScheduledEventId, seu.UserId, seu.TeamId });
            modelBuilder.Entity<ScheduledEventUser>()
                .HasOne(seu => seu.ScheduledEvent)
                .WithMany(se => se.AttachedUsersCollection)
                .HasForeignKey(seu => seu.ScheduledEventId);
            modelBuilder.Entity<ScheduledEventUser>()
                .HasOne(seu => seu.TeamMemberAssignment)
                .WithMany(ut => ut.ScheduledEventUsers)
                .HasForeignKey(seu => new { seu.UserId, seu.TeamId });
        }
        private static void ConfigureEnumMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecurringBill>()
                .Property(rb => rb.ExpenseCategory)
                .HasConversion<string>();
            modelBuilder.Entity<RecurringBill>()
                .Property(rb => rb.Frequency)
                .HasConversion<string>();

            modelBuilder.Entity<SingleComplexOrder>()
                .Property(rb => rb.ExpenseCategory)
                .HasConversion<string>();

            modelBuilder.Entity<TotalExpense>()
                .Property(rb => rb.ExpenseCategory)
                .HasConversion<string>();

            modelBuilder.Entity<SingleOrderElement>()
                .Property(soe => soe.Unit)
                .HasConversion<string>();
        }
        private static void ConfigureMoneyColumnTypes(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) && p.Name == "Value"))
            {
                type.Relational().ColumnType = "decimal(18, 2)";
            }
        }
        private static void ConfigureUserexpensesRealtions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TotalExpense>()
                .HasOne(te => te.Owner)
                .WithMany(u => u.TotalExpenses)
                .HasForeignKey(ut => ut.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
