using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [RegularExpression(@"^\d+\.\d{2}$")] // validate that the value has precision of two decimal places
        [Range(0, 9999999999999999.99)]
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
