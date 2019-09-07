using Flatmate.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.Scheduler
{
    //TODO: remove inheritance
    public class EventDetails : ScheduledEvent
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int [] ParticipantIds { get; set; }
        public string [] ParticipantNames { get; set; }
    }
}
