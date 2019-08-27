using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class ScheduledEvent
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsBlocking { get; set; }
        
        public ICollection<ScheduledEventUser> AttachedUsersCollection { get; set; }
    }
}
