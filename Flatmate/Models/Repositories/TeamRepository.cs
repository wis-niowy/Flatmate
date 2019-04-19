using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flatmate.Models.IRepositories;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public FlatmateContext FlatmateContext
        {
            get
            {
                return Context as FlatmateContext;
            }
        }

        public TeamRepository(FlatmateContext context): base(context) { }

        public Team GetTeamWithMembersById(int teamId)
        {
            return FlatmateContext.Teams
                .Where(t => t.TeamId == teamId)
                .Include(t => t.UsersCollection)
                .ToList()
                .FirstOrDefault();
        }
    }
}
