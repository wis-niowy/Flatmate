using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.Services
{
    public class SchedulerService
    {
        public IUnitOfWork _repository;

        public SchedulerService(IUnitOfWork uow = null)
        {
            _repository = uow;
        }


        //public IEnumerable<ScheduledEvent> GetUserAllEventsForDisplay()
        //{
            
        //}
    }
}
