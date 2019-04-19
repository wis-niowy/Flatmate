using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class RecurringBillDebitor
    {
        public int RecurringBillId { get; set; }
        public int DebitorId { get; set; }

        // relationships
        public RecurringBill RecurringBill { get; set; }
        public User Debitor { get; set; }
    }
}
