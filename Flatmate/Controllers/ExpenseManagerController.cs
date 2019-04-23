using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Flatmate.Models;
using Flatmate.Models.EntityModels;
using Flatmate.ViewModels.ExpenseManager;
using Microsoft.AspNetCore.Mvc;

namespace Flatmate.Controllers
{
    public class ExpenseManagerController : Controller
    {
        public IUnitOfWork _repository;
        public IMapper _mapper;

        public ExpenseManagerController(IUnitOfWork uow, IMapper mapper)
        {
            _repository = uow;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewSingleExpense()
        {
            int userId = 1;
            var teamId = _repository.Users.GetUserTeamId(userId);
            var team = _repository.Teams.GetTeamWithMembersById(teamId);
            var flatmates = team.UsersCollection.Where(el => el.UserId != userId).ToList();
            var viewModel = new NewSingleExpenseViewModel(flatmates);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewSingleExpense([Bind("ExpenseSubject,Date,Value,ExpenseCategory,DebitorsCollection")] NewSingleExpenseViewModel expenseViewModel)
        {
            int userId = 1;
            if (ModelState.IsValid)
            {
                Expense newExpense = _mapper.Map<Expense>(expenseViewModel);
                newExpense.InitiatorId = userId;
                _repository.Expenses.Add(newExpense);
                await _repository.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult NewExpenseList()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewExpenseList([Bind("ExpenseSubject,Date,Value,ExpenseCategory,DebitorsCollection")] NewSingleExpenseViewModel expenseViewModel)
        {
            int userId = 1;
            if (ModelState.IsValid)
            {
                
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}