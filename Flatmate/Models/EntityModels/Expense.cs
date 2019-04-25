using Flatmate.Helpers;
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
        [CurrencyValidation(ErrorMessage = "Value must be floting value with none decimal places or one or two decimal places preceded by a comma")]
        public double Value { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }

        // properties not mapped as columns
        // public User Debitor { get; set; }

        // relationships
        public User Initiator { get; set; }
        //public Team Team { get; set; }
        public ICollection<ExpenseDebitor> DebitorsCollection { get; set; }
    }
}
