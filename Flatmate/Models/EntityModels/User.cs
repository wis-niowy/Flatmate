using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class User : IdentityUser<int>
    {
        //public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get { return Email; } set { Email = value; } }
        public virtual ICollection<TotalExpense> TotalExpenses { get; set; }
        public virtual ICollection<UserTeam> TeamAssignments { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
