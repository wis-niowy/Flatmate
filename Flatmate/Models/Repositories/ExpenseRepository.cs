using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flatmate.Models.IRepositories;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.Repositories
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        public FlatmateContext FlatmateContext
        {
            get
            {
                return Context as FlatmateContext;
            }
        }

        public ExpenseRepository(FlatmateContext context): base(context) { }

        /// <summary>
        /// Get all expenses that others owe user (which is expenses initiated by user)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Expense> GetUserCredibilities(int userId)
        {
            IEnumerable<Expense> expenseList = FlatmateContext.Expenses
                .Where(ex => ex.InitiatorId == userId)
                .Include(ex => ex.DebitorsCollection)
                .ToList();
            return expenseList;
        }

        /// <summary>
        /// Get all expenses that user owes others (which is expenses initiated by others)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Expense> GetUserLiabilities(int userId)
        {
            IEnumerable<Expense> expenseList = FlatmateContext.Expenses
                .Join(FlatmateContext.ExpenseDebitor, expense => expense.ExpenseId, expdeb => expdeb.ExpenseId, (expense, expdeb) => new
                {
                    ExpenseObject = expense,
                    DebitorId = expdeb.DebitorId
                })
                .Where(ex => ex.DebitorId == userId)
                .Select(a => a.ExpenseObject)
                .Include(exp => exp.DebitorsCollection)
                .ToList();
            //IEnumerable<Expense> expenseList = ;
            return expenseList;
        }
    }
}
