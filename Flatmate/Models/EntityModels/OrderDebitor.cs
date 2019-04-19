using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class OrderDebitor
    {
        public int OrderId { get; set; }
        public int DebitorId { get; set; }

        // relationships
        public Order Order { get; set; }
        public User Debitor { get; set; }
    }
}
