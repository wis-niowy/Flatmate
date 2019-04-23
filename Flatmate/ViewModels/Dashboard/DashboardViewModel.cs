using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;
using Flatmate.ViewModels.Dashboard;

namespace Flatmate.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<LiabilityExpenseViewModel> UserLiabilities { get; set; } // 'I owe'
        public IEnumerable<CredibilityExpenseViewModel> UserCredibilities { get; set; } // 'I am owed'
        public Dictionary<User,double> FlatmateBalances { get; set; } // balance with every flatmate
    }
}
