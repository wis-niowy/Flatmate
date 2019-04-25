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

        public Team GetUserTeamWithMembers(int userId)
        {
            var teamId = FlatmateContext.Users
                .Where(usr => usr.UserId == userId)
                .Select(usr => usr.TeamId)
                .FirstOrDefault();
            return GetTeamWithMembersById(teamId);
        }

        public List<User> GetUserFlatmates(int userId)
        {
            var team = GetUserTeamWithMembers(userId);
            return team.UsersCollection
                .Where(usr => usr.UserId != userId)
                .ToList();
        }
    }
}
