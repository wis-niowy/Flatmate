using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.IRepositories;
using Flatmate.Models.Repositories;

namespace Flatmate.Models
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FlatmateContext _context;
        public IUserRepository Users { get; private set; }
        public ITeamRepository Teams { get; private set; }
        public IExpenseRepository Expenses { get; private set; }

        public UnitOfWork(FlatmateContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Teams = new TeamRepository(_context);
            Expenses = new ExpenseRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

    }
}
