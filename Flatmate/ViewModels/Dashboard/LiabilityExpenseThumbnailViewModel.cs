using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.Dashboard
{
    /// <summary>
    /// ViewModel for card item view of outgoing expenses on Dashboard
    /// </summary>
    public class LiabilityExpenseThumbnailViewModel
    {
        public double Value { get; set; }
        public int InitiatorId { get; set; }
        public string InitiatorFirstName { get; set; }
        public string InitiatorLastName { get; set; }
        public DateTime Date { get; set; }
        public string ExpenseSubject { get; set; }
    }
}
