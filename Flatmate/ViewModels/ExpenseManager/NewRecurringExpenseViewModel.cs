﻿using Flatmate.Helpers;
using Flatmate.Models.EntityModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flatmate.ViewModels.ExpenseManager
{
    public class NewRecurringExpenseViewModel
    {
        public string BillSubject { get; set; }
        [CurrencyValidation(ErrorMessage = "Value must be floting value with none decimal places or one or two decimal places preceded by a comma")]
        public double Value { get; set; }
        public Frequency Frequency { get; set; }
        public DateTime StartDate { get; set; }

        public ICollection<SelectListItem> FlatmatesCollection { get; set; }
        public int[] DebitorsCollection { get; set; }

        public NewRecurringExpenseViewModel() { }

        public NewRecurringExpenseViewModel(ICollection<User> flatmatesCollection)
        {
            FlatmatesCollection = flatmatesCollection
                .Select(user => new SelectListItem
                {
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
