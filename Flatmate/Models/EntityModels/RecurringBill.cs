using Flatmate.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class RecurringBill
    {
        public int RecurringBillId { get; set; }
        public int InitiatorId { get; set; }
        //public int TeamId { get; set; }
        public string BillSubject { get; set; }
        [CurrencyValidation(ErrorMessage = "Value must be floting value with none decimal places or one or two decimal places preceded by a comma")]
        public double Value { get; set; }
        public Frequency Frequency { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public DateTime LastEffectiveDate { get; set; }
        public DateTime NextEffectiveDate { get; set; }

        // relationships
        public User Initiator { get; set; }
        //public Team Team { get; set; }
        public ICollection<RecurringBillDebitor> DebitorsCollection { get; set; }
    }
}
