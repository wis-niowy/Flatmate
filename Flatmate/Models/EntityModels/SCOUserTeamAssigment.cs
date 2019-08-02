using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class SCOUserTeamAssignment
    {
        public int SCOId { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public SingleComplexOrder SCO { get; set; }
        public UserTeam UserTeam { get; set; }
    }
}
