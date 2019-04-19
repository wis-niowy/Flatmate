using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class RecurringBill
    {
        public int RecurringBillId { get; set; }
        public int InitiatorId { get; set; }
        //public int TeamId { get; set; }
        public string BillSubject { get; set; }
        public double Value { get; set; }
        public double Frequency { get; set; } // TODO: enum type { Daily, Weekly, 2Wekly, 3Weekly, Monthly, Annually }
        public DateTime LastEffectiveDate { get; set; }
        public DateTime NextEffectiveDate { get; set; }

        // relationships
        public User Initiator { get; set; }
        //public Team Team { get; set; }
        public ICollection<RecurringBillDebitor> DebitorsCollection { get; set; }
    }
}
