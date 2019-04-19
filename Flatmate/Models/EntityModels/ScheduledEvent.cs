using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class ScheduledEvent
    {
        public int ScheduledEventId { get; set; }
        //public int TeamId { get; set; }
        public int OwnerId { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Desription { get; set; }

        // relationships
        //public Team Team { get; set; }
        public User Owner { get; set; }
        public ICollection<ScheduledEventUser> AttachedUsersCollection { get; set; }
    }
}
