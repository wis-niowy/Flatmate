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
    public class EditOneTimeExpenseViewModel
    {
        public int ExpenseId { get; set; }
        public int InitiatorId { get; set; }
        public string ExpenseSubject { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [CurrencyValidation(ErrorMessage = "Value must be floting value with none decimal places or one or two decimal places preceded by a comma")]
        public double Value { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; }

        public ICollection<SelectListItem> FlatmatesCollection { get; set; }
        public int[] DebitorsCollection { get; set; }

        public EditOneTimeExpenseViewModel() { }

        public EditOneTimeExpenseViewModel(ICollection<User> flatmatesCollection)
        {
            AddFlatmates(flatmatesCollection);
        }

        public void AddFlatmates(ICollection<User> flatmatesCollection)
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