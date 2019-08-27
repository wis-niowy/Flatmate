using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Flatmate.Helpers;
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

        #region OneTimeExpenseControllers
        public IActionResult NewSingleExpense()
        {
            //int userId = 1;
            //var flatmates = _repository.Teams.GetUserFlatmates(userId);
            //var viewModel = new NewSingleExpenseViewModel(flatmates);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewSingleExpense([Bind("ExpenseSubject,Date,Value,ExpenseCategory,DebitorsCollection")] NewSingleExpenseViewModel expenseViewModel)
        {
            //int userId = 1;
            //if (ModelState.IsValid)
            //{
            //    Expense newExpense = _mapper.Map<Expense>(expenseViewModel);
            //    newExpense.InitiatorId = userId;
            //    _repository.Expenses.Add(newExpense);
            //    await _repository.CompleteAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            return View();
        }

        public IActionResult NewExpenseList()
        {
            //int userId = 1;
            //var teamId = _repository.Users.GetUserTeamId(userId);
            //var team = _repository.Teams.GetTeamWithMembersById(teamId);
            //var flatmates = team.UsersCollection.Where(el => el.UserId != userId).ToList();
            var viewModel = new NewExpenseListViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewExpenseList([Bind("ExtExpenseItems")] NewExpenseListViewModel expenseViewModel)
        {
            //int userId = 1;
            //if (ModelState.IsValid)
            //{
            //    var expenses = expenseViewModel.ExtExpenseItems.Select(el => _mapper.Map<Expense>(el)).ToList();
            //    foreach (var exp in expenses)
            //    {
            //        exp.InitiatorId = userId;
            //    }
            //    _repository.Expenses.AddRange(expenses);
            //    await _repository.CompleteAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            return View();
        }

        //public IActionResult GetExpenseListItemPartialView()
        //{
        //    //var flatmates = TempData["flatmates"];
        //    //var viewModel = new NewExpenseListViewModel(flatmates as List<User>);
        //    int userId = 1;
        //    //var flatmates = _repository.Teams.GetUserFlatmates(userId);
        //    //var viewModel = new NewSingleExpenseViewModel(flatmates);
        //    return PartialView("ExpenseListItemPartialView", viewModel);
        //}

        //public IActionResult ViewOneTimeExpenses()
        //{
        //    int userId = 1;
        //    var userExpenses = _repository.Expenses.Find(exp => exp.InitiatorId == userId);
        //    //var flatmates = _repository.Teams.GetUserFlatmates(userId);
        //    var viewModel = userExpenses.Select(exp => _mapper.Map<ExistingSingleExpenseViewModel>(exp));
        //    return View(viewModel);
        //}

        //public IActionResult EditOneTimeExpense(int expenseId)
        //{
        //    int userId = 1;
        //    var expense = _repository.Expenses.GetExpenseWithDebitors(expenseId);
        //    var viewModel = _mapper.Map<EditOneTimeExpenseViewModel>(expense);
        //    var flatmates = _repository.Teams.GetUserFlatmates(userId);
        //    viewModel.AddFlatmates(flatmates);
        //    return View(viewModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOneTimeExpense([Bind("ExpenseId,InitiatorId,ExpenseSubject,Date,Value,ExpenseCategory,DebitorsCollection")] EditOneTimeExpenseViewModel expenseViewModel)
        {
            //int userId = 1;
            if (ModelState.IsValid)
            {
                //Expense newExpense = _mapper.Map<Expense>(expenseViewModel);
                //// temporary loop - TODO: do it in automapper
                ////foreach (var el in newExpense.DebitorsCollection)
                ////{
                ////    el.ExpenseId = newExpense.ExpenseId;
                ////}
                //var oldDebitorsCollection = _repository.ExpenseDebitor.Find(expdeb => expdeb.ExpenseId == newExpense.ExpenseId).ToList();
                //var newDebitorsCollection = newExpense.DebitorsCollection;
                //// manual diff detection is required as ef core will not find elements deleted from DebitorsCollection nav property to remove from database itself
                //var deletedDebitors = oldDebitorsCollection.Except(newDebitorsCollection,
                //    new GenericComparer<ExpenseDebitor>((el1, el2) => el1.DebitorId == el2.DebitorId,
                //                                        el => 100)).ToList();
                //if (deletedDebitors.Count > 0)
                //{
                //    _repository.ExpenseDebitor.RemoveRange(deletedDebitors);
                //}
                //_repository.Expenses.Update(newExpense); // here ExpenseId is set in every DebitorsCollection navigation property element
                //await _repository.CompleteAsync();
                //return RedirectToAction(nameof(ViewOneTimeExpenses));
            }
            return View();
        }

        //public IActionResult DeleteOneTimeExpense()
        //{
        //    int userId = 1;
        //    var flatmates = _repository.Teams.GetUserFlatmates(userId);
        //    var viewModel = new NewSingleExpenseViewModel(flatmates);
        //    return View(viewModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOneTimeExpense([Bind("ExpenseSubject,Date,Value,ExpenseCategory,DebitorsCollection")] NewSingleExpenseViewModel expenseViewModel)
        {
            //int userId = 1;
            if (ModelState.IsValid)
            {
                //Expense newExpense = _mapper.Map<Expense>(expenseViewModel);
                //newExpense.InitiatorId = userId;
                //_repository.Expenses.Add(newExpense);
                //await _repository.CompleteAsync();
                //return RedirectToAction(nameof(Index));
            }
            return View();
        }
        #endregion

        #region RecurringExpenseControlers
        public IActionResult NewRecurringExpense()
        {
            //int userId = 1;
            //var flatmates = _repository.Teams.GetUserFlatmates(userId);
            //var viewModel = new NewRecurringExpenseViewModel(flatmates);
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> NewRecurringExpense([Bind("BillSubject,Value,Frequency,StartDate,DebitorsCollection")] NewRecurringExpenseViewModel recExpenseViewModel)
        //{
        //    int userId = 1;
        //    if (ModelState.IsValid)
        //    {
        //        RecurringBill newRecExpense = _mapper.Map<RecurringBill>(recExpenseViewModel);
        //        newRecExpense.InitiatorId = userId;
        //        _repository.RecurringBills.Add(newRecExpense);
        //        await _repository.CompleteAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}
        #endregion

        #region OrderControlers
        //public IActionResult NewOrder()
        //{
        //    int userId = 1;
        //    var flatmates = _repository.Teams.GetUserFlatmates(userId);
        //    var viewModel = new NewOrderViewModel(flatmates);
        //    return View(viewModel);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> NewOrder([Bind("OrderSubject,ExpenseCategory,Date,DebitorsCollection")] NewOrderViewModel orderViewModel)
        //{
        //    int userId = 1;
        //    if (ModelState.IsValid)
        //    {
        //        Order newOrder = _mapper.Map<Order>(orderViewModel);
        //        newOrder.InitiatorId = userId;
        //        _repository.Orders.Add(newOrder);
        //        await _repository.CompleteAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}
        #endregion
    }
}