using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class UserTeam
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }        
        public User User { get; set; }
        public Team Team { get; set; }
        public ICollection<ScheduledEventUser> ScheduledEventUsers { get; set; }
        public ICollection<RecurringBillPerTeamMember> RecurringBillPerTeamMembers { get; set; }
        public ICollection<PartialExpense> PartialExpenses { get; set; }
        public ICollection<SCOUserTeamAssignment> SCOUserTeamAssignments { get; set; }
    }
}
