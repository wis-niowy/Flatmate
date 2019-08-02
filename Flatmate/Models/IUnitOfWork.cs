using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.IRepositories;

namespace Flatmate.Models
{
    public interface IUnitOfWork
    {
        //IUserRepository Users { get; }
        //ITeamRepository Teams { get; }
        //IExpenseRepository Expenses { get; }
        //IExpenseDebitorRepository ExpenseDebitor { get; }
        //IScheduledEventRepository ScheduledEvents { get; }
        //IRecurringBillRepository RecurringBills { get; }
        //IOrderRepository Orders { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
