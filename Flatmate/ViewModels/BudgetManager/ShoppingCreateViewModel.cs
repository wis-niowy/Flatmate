using Flatmate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.BudgetManager
{
    public class ShoppingCreateViewModel
    {
        public string Subject { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string [] SingleElementTitles { get; set; }
        public double [] SingleElementAmounts { get; set; }
        public Unit [] SingleElementUnits { get; set; }
        public int [] ParticipantIds { get; set; }
        public string [] ParticipantNames { get; set; }
    }
}
