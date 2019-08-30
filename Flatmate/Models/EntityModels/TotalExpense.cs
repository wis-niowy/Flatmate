using Flatmate.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class TotalExpense
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime FinalizationDate { get; set; }
        [CurrencyValidation(ErrorMessage = "Value must be floating value with none decimal places or one or two decimal places preceded by a comma.")]
        public double Value { get; set; }
        public bool Covered { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public ICollection<PartialExpense> PartialExpenses { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }

    }
}
