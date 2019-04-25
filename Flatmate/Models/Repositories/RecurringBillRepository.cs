using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flatmate.Models.IRepositories;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.Repositories
{
    public class RecurringBillRepository : Repository<RecurringBill>, IRecurringBillRepository
    {
        public FlatmateContext FlatmateContext
        {
            get
            {
                return Context as FlatmateContext;
            }
        }

        public RecurringBillRepository(FlatmateContext context): base(context) { }

        
    }
}
