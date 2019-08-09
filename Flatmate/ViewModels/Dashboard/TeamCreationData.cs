using Flatmate.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.Dashboard
{
    public class TeamCreationData
    {
        public TeamCreationData()
        {
            InvitationEmails = new string[numberOfInvitations];
        }
        private const int numberOfInvitations = 4;
        public string TeamName { get; set; }
        public string [] InvitationEmails { get; set; }
    }
}
