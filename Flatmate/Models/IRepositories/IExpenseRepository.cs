using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.IRepositories
{
    public interface IExpenseRepository: IRepository<Expense>
    {
        IEnumerable<Expense> GetUserLiabilities(int userId); // 'I owe'
        IEnumerable<Expense> GetUserCredibilities(int userId); // 'I am owed'
        Expense GetExpenseWithDebitors(int expenseId);
    }
}
