using Flatmate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.BudgetManager
{
    public class ExpenseCreateViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public double Value { get; set; }
        public bool IsCovered { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int [] ParticipantIds { get; set; }
        public string [] ParticipantNames { get; set; }
        public double [] ParticipantsCharge { get; set; }
        public bool [] DidParticipantsPay { get; set; }
    }
}
