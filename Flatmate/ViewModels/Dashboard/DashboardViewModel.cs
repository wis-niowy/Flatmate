using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;
using Flatmate.ViewModels.Dashboard;
using Flatmate.ViewModels.Scheduler;

namespace Flatmate.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public IEnumerable<EventDetails> UpcomingEventsList { get; set; }
        public IEnumerable<SingleComplexOrder> PlannedShoppingInformations { get; set; }
        public IEnumerable<SettlementViewModel.SingleExpense> UserCredibilities { get; set; }
        public IEnumerable<SettlementViewModel.SingleExpense> UserLiabilities { get; set; }
        public IEnumerable<RecurringBill> RecurringBills { get; set; }
    }
}
