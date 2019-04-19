using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class ExpenseDebitor
    {
        public int ExpenseId { get; set; }
        public int DebitorId { get; set; }

        // relationships
        public Expense Expense { get; set; }
        public User Debitor { get; set; }
    }
}
