using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public ICollection<UserTeam> UserAssignments { get; set; }
    }
}
