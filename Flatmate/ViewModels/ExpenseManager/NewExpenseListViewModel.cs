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
    public class NewExpenseListViewModel
    {
        public ICollection<NewSingleExpenseViewModel> ExtExpenseItems { get; set; }

        public NewExpenseListViewModel()
        {
            ExtExpenseItems = new List<NewSingleExpenseViewModel>();
        }
    }
}