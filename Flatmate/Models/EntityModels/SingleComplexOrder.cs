using Flatmate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class SingleComplexOrder
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<SCOUserTeamAssignment> SCOTeamMemberAssignments { get; set; }
        public ICollection<SingleOrderElement> OrderElements { get; set; }
    }
}
