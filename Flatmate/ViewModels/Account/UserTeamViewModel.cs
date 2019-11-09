using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.Account
{
    public class UserTeamViewModel
    {
        [Display(Name = "User")]
        public string UserName { get; set; }

        [Display(Name = "Team")]
        public string TeamName { get; set; }
    }
}
