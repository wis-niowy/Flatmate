using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class PartialExpense
    {
        public int TotalExpenseId { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public double Value { get; set; }
        public DateTime? SettlementDate{ get; set; }
        public bool Covered { get; set; }
        public TotalExpense TotalExpense { get; set; }
        public UserTeam TeamMemberAssignment { get; set; }
    }
}
