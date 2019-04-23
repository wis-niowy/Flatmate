using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flatmate.Models.IRepositories;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.Repositories
{
    public class ScheduledEventRepository : Repository<ScheduledEvent>, IScheduledEventRepository
    {
        public FlatmateContext FlatmateContext
        {
            get
            {
                return Context as FlatmateContext;
            }
        }

        public ScheduledEventRepository(FlatmateContext context): base(context) { }


        public IReadOnlyCollection<ScheduledEvent> GetAllEvents(int userId)
        {
            var initiatedEvents = FlatmateContext.ScheduledEvents
                .Where(se => se.OwnerId == userId)
                .Include(se => se.AttachedUsersCollection)
                .ToList();
            var attachedEvents = FlatmateContext.ScheduledEventUser
                .Where(seu => seu.UserId == userId)
                .Select(seu => seu.ScheduledEvent)
                .Include(se => se.AttachedUsersCollection)
                .ToList();

            return initiatedEvents;//.Concat(attachedEvents).ToList();

            //var attachedEvents2 = FlatmateContext.ScheduledEvents
            //    .Join(FlatmateContext.ScheduledEventUser, se => se.ScheduledEventId, seu => seu.ScheduledEventId, (se, seu) => new {
                    
            //    })
        }
    }
}
