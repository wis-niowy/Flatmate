using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class User
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public Team Team { get; set; }

        //one to many
        public ICollection<Order> InitializedOrdersCollection { get; set; }
        public ICollection<Expense> InitializedExpensesCollection { get; set; }
        public ICollection<RecurringBill> InitializedRecurringBillsCollection { get; set; }
        public ICollection<ScheduledEvent> InitializedScheduledEvents { get; set; }
        // many to many
        //public ICollection<UserTeam> TeamsCollection { get; set; }
        public ICollection<OrderDebitor> AttachedOrdersCollection { get; set; }
        public ICollection<ExpenseDebitor> AttachedExpensesCollection { get; set; }
        public ICollection<RecurringBillDebitor> AttachedRecurringBillsCollection { get; set; }
        public ICollection<ScheduledEventUser> AttachedScheduledEvents { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
