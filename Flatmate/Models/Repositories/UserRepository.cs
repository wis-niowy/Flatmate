using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flatmate.Models.IRepositories;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public FlatmateContext FlatmateContext
        {
            get
            {
                return Context as FlatmateContext;
            }
        }

        public UserRepository(FlatmateContext context): base(context) { }

        public int GetUserTeamId(int userId)
        {
            return FlatmateContext.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.TeamId)
                .ToList()
                .FirstOrDefault();
        }

    }
}
