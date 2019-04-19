using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class Order
    {
        public int OrderId { get; set; }
        public int InitiatorId { get; set; }
        //public int TeamId { get; set; }
        public string OrderSubject { get; set; }
        public DateTime Date { get; set; }

        // relationships
        public User Initiator { get; set; }
        //public Team Team { get; set; }
        public ICollection<OrderDebitor> DebitorsCollection { get; set; }
    }
}
