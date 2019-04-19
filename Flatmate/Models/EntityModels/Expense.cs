using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public int InitiatorId { get; set; }
        //public int TeamId { get; set; }
        public string ExpenseSubject { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public int ExpenseCategory { get; set; } // TODO: enum type

        // properties not mapped as columns
        // public User Debitor { get; set; }

        // relationships
        public User Initiator { get; set; }
        //public Team Team { get; set; }
        public ICollection<ExpenseDebitor> DebitorsCollection { get; set; }
    }
}
