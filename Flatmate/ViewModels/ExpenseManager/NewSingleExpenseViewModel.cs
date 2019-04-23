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
    public class NewSingleExpenseViewModel
    {
        public string ExpenseSubject { get; set; }
        public DateTime Date { get; set; }
        [RegularExpression(@"^\d+\.\d{2}$")]
        [Range(0, 9999999999999999.99)]
        public double Value { get; set; }
        public int ExpenseCategory { get; set; } // TODO: enum type

        //public SelectList FlatmatesCollection { get; set; }
        public ICollection<SelectListItem> FlatmatesCollection { get; set; }
        public int[] DebitorsCollection { get; set; }

        public NewSingleExpenseViewModel() { }

        public NewSingleExpenseViewModel(ICollection<User> flatmatesCollection)
        {
            //FlatmatesCollection = new SelectList(flatmatesCollection, "UserId", "FirstName");
            FlatmatesCollection = flatmatesCollection
                .Select(user => new SelectListItem {
                    Text = (new StringBuilder())
                            .Append(user.FirstName)
                            .Append(" ")
                            .Append(user.LastName)
                            .ToString(),
                    Value = user.UserId.ToString()
                }).ToList();
        }
    }
}