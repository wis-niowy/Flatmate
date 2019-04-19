using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }

        // one to many
        public ICollection<User> UsersCollection { get; set; }
        //public ICollection<Order> OrdersCollection { get; set; }
        //public ICollection<Expense> ExpensesCollection { get; set; }
        //public ICollection<RecurringBill> RecurringBillsCollection { get; set; }
        //public ICollection<ScheduledEvent> ScheduledEventsCollection { get; set; }
        // many to many
        //public ICollection<UserTeam> UsersCollection { get; set; }
    }
}
