using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.IRepositories
{
    public interface IScheduledEventRepository: IRepository<ScheduledEvent>
    {
        IReadOnlyCollection<ScheduledEvent> GetAllEvents(int userId);
    }
}
