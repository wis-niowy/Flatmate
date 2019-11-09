using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.Account
{
    public class UserDetailsViewModel
    {
        [Display(Name = "Name")]
        public string FullName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Display(Name = "Email")]
        public ICollection<UserTeamViewModel> TeamAssignments { get; set; }
    }
}
