using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.IRepositories
{
    public interface ITeamRepository: IRepository<Team>
    {
        Team GetTeamWithMembersById(int teamId);
        Team GetUserTeamWithMembers(int userId);
        List<User> GetUserFlatmates(int userId);
    }
}
