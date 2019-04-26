using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;

namespace Flatmate.ViewModels.Dashboard
{
    /// <summary>
    /// ViewModel for card item view of incoming expenses on Dashboard
    /// </summary>
    public class CredibilityExpenseViewModel
    {
        public double Value { get; set; }
        public int InitiatorId { get; set; }
        public string InitiatorFirstName { get; set; }
        public string InitiatorLastName { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string ExpenseSubject { get; set; }
        public int DebitorId { get; set; }
        public string DebitorFirstName { get; set; }
        public string DebitorLastName { get; set; }

    }
}
