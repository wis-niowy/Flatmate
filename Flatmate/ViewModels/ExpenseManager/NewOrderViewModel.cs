using Flatmate.Helpers;
using Flatmate.Models.EntityModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.ExpenseManager
{
    public class NewOrderViewModel
    {
        public string OrderSubject { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public ICollection<SelectListItem> FlatmatesCollection { get; set; }
        public int[] DebitorsCollection { get; set; }

        public NewOrderViewModel() { }

        public NewOrderViewModel(ICollection<User> flatmatesCollection)
        {
            FlatmatesCollection = flatmatesCollection
                .Select(user => new SelectListItem
                {
                    Text = (new StringBuilder())
                            .Append(user.FirstName)
                            .Append(" ")
                            .Append(user.LastName)
                            .ToString(),
                    Value = user.Id.ToString()
                }).ToList();
        }
    }
}
