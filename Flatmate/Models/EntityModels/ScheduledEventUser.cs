using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class ScheduledEventUser
    {
        public int ScheduledEventId { get; set; }
        public int UserId { get; set; }

        // relationships
        public ScheduledEvent ScheduledEvent { get; set; }
        public User User { get; set; }
    }
}
