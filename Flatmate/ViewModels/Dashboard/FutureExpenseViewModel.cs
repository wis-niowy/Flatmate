using Flatmate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.Dashboard
{
    public class FutureExpenseViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public double Value { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime NextOccurenceDate { get; set; }
        public Frequency Frequency { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
    }
}
