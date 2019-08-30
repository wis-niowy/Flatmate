using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class RecurringBillPerTeamMember
    {
        public int RecurringBillId { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public RecurringBill RecurringBill { get; set; }
        public UserTeam TeamMemberAssignment { get; set; }
    }
}
