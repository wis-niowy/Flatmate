using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.Dashboard
{
    public class SettlementViewModel
    {
        public List<SingleExpense> UserLiabilities { get; set; }
        public List<SingleExpense> UserCredibilities { get; set; }

        public class SingleExpense
        {
            public Tuple<int, string> UserInfo { get; set; }
            public Tuple<int, string> TeamInfo { get; set; }
            public int TotalExpenseId { get; set; }
            public double Value { get; set; }
            public DateTime FinalizationDate { get; set; }

        }
    }

}
