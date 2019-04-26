using Flatmate.Models.EntityModels;
using Flatmate.Models.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.Repositories
{
    public class ExpenseDebitorRepository : Repository<ExpenseDebitor>, IExpenseDebitorRepository
    {
        public FlatmateContext FlatmateContext
        {
            get
            {
                return Context as FlatmateContext;
            }
        }

        public ExpenseDebitorRepository(FlatmateContext context) : base(context) { }
    }
}
