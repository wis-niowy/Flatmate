using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;

namespace Flatmate.ViewModels.Dashboard
{
    /// <summary>
    /// ViewModel for card item view of incoming expenses on Dashboard
    /// </summary>
    public class CredibilityExpenseThumbnailViewModel
    {
        public double Value { get; set; }
        public int InitiatorId { get; set; }
        public string InitiatorFirstName { get; set; }
        public string InitiatorLastName { get; set; }
        public DateTime Date { get; set; }
        public string ExpenseSubject { get; set; }
        public int DebitorId { get; set; }
        public string DebitorFirstName { get; set; }
        public string DebitorLastName { get; set; }

    }
}
