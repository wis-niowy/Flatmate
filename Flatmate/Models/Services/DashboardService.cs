using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models.IRepositories;
using Flatmate.Models.Repositories;
using Flatmate.Models.EntityModels;

namespace Flatmate.Models.Services
{
    public class DashboardService
    {
        //public IUnitOfWork _repository;

        //public DashboardService(IUnitOfWork uow = null)
        //{
        //    _repository = uow;
        //}

        ///// <summary>
        ///// Get personal balance between user and given creditor (another user)
        ///// </summary>
        ///// <param name="userId">User for whom balance is calcualted</param>
        ///// <param name="creditorId">Creditor user</param>
        ///// <returns>Current balance between user and reditor user based on all user's liabilities and credibilities</returns>
        //public double GetBalanceBetweenUserAndCreditor(int creditorId,
        //                                               IEnumerable<Expense> userLiabilities,
        //                                               IEnumerable<Expense> userCredibilities)
        //{
        //    var minusDebit = userLiabilities
        //        .Where(exp => exp.InitiatorId == creditorId)
        //        .Select(exp => exp.Value / exp.DebitorsCollection.Count()).Sum();
        //    var plusDebit = userCredibilities
        //        .Where(exp => exp.DebitorsCollection.Where(expdeb => expdeb.DebitorId == creditorId).Any())
        //        .Select(exp => exp.Value / exp.DebitorsCollection.Count()).Sum();
        //    return plusDebit - minusDebit;
        //}

        ///// <summary>
        ///// Get balances for all flatmates belonging to user's team
        ///// </summary>
        ///// <param name="userId">Current user id</param>
        ///// <param name="teamMemebrs">All members of user's team</param>
        ///// <param name="userLiabilities">Expenses that user is attached to as debtor</param>
        ///// <param name="userCredibilities">Expenses that user has initiated</param>
        ///// <returns>Dictionary containing all flatmates as keys and relevant balances as values</returns>
        //public Dictionary<User,double> GetAllFlatmatesBalances(int userId,
        //                                                       ICollection<User> teamMemebrs,
        //                                                       IEnumerable<Expense> userLiabilities,
        //                                                       IEnumerable<Expense> userCredibilities)
        //{
        //    var returnDictionary = new Dictionary<User, double>();
        //    var currentUser = teamMemebrs.Where(u => u.UserId == userId).FirstOrDefault();
        //    teamMemebrs.Remove(currentUser);
        //    foreach (var flatmate in teamMemebrs)
        //    {
        //        returnDictionary.Add(flatmate, GetBalanceBetweenUserAndCreditor(flatmate.UserId,
        //                                                                        userLiabilities,
        //                                                                        userCredibilities));
        //    }
        //    return returnDictionary;
        //}

        ///// <summary>
        ///// Get models for viewmodels with expences that user personally owes to somebody 
        ///// </summary>
        ///// <param name="userId">User ID</param>
        ///// <param name="userLiabilities">Expenses which user is attached to as debitor</param>
        ///// <returns>Expenses with user's personal owings - calculated value etc</returns>
        //public IEnumerable<Expense> GetUserSpecificLiabilityThumbnailModels(int userId,
        //                                                                    IEnumerable<Expense> userLiabilities)
        //{
        //    return userLiabilities
        //        .Select(exp => new Expense {
        //            Initiator = exp.Initiator,
        //            Date = exp.Date,
        //            Value = exp.Value / exp.DebitorsCollection.Count(),
        //            ExpenseSubject = exp.ExpenseSubject,
        //            ExpenseCategory = exp.ExpenseCategory
        //        })
        //        .ToList();
        //}

        //public IEnumerable<ExpenseDebitor> GetUserSpecificCredibilityThumbnailModels(int userId,
        //                                                                    IEnumerable<Expense> userCredibilities)
        //{
        //    return userCredibilities
        //        .SelectMany(exp => exp.DebitorsCollection)
        //        .Select(expdeb => new ExpenseDebitor {
        //            Expense = new Expense {
        //                ExpenseId = expdeb.Expense.ExpenseId,
        //                InitiatorId = expdeb.Expense.InitiatorId,
        //                Initiator = expdeb.Expense.Initiator,
        //                Date = expdeb.Expense.Date,
        //                Value = expdeb.Expense.Value / expdeb.Expense.DebitorsCollection.Count(),
        //                ExpenseSubject = expdeb.Expense.ExpenseSubject,
        //                ExpenseCategory = expdeb.Expense.ExpenseCategory,
        //                DebitorsCollection = expdeb.Expense.DebitorsCollection
        //            },
        //            Debitor = expdeb.Debitor
        //        })
        //        .ToList();
        //}
    }
}
