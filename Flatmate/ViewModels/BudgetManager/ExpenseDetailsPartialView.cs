using Flatmate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.BudgetManager
{
    public class ExpenseDetailsPartialView
    {
        public string Subject { get; set; }
        public string UserName { get; set; }
        public double Value { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public string GroupName { get; set; }
    }
}
