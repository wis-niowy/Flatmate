using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flatmate.Models;
using Flatmate.ViewModels.Dashboard;
using Flatmate.Models.EntityModels;
using Flatmate.Data;
using Flatmate.ViewModels;
using Microsoft.EntityFrameworkCore;
using Flatmate.ViewModels.Scheduler;

namespace Flatmate.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlatmateContext _context;
        public HomeController(FlatmateContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListUpcomingEvents(int? displayedDays)
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var displayedNumberOfDays = displayedDays ?? 3;
            var upcomingEventsList = GenerateEventDetails(currentUserId, displayedNumberOfDays);
            return PartialView("_upcomingEventsPartial", upcomingEventsList);
        }
        public IActionResult ListShoppingInformation()
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var plannedShoppingInformations = GenerateShoppingInformation(currentUserId);
            return PartialView("_plannedShoppingHomePartial", plannedShoppingInformations);
        }
        public IActionResult ListSettlementInformation()
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var userCredibilities = GenerateUserCredibilities(currentUserId);
            var userLiabilities = GenerateUserLiabilities(currentUserId);

            var settlementVM = new SettlementViewModel
            {
                UserCredibilities = userCredibilities,
                UserLiabilities = userLiabilities
            };
            return PartialView("_settlementPartial", settlementVM);
        }
        public IActionResult ListRecurringBills()
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var recurringBillsInfo = GenerateRecurringBills(currentUserId);            
            return PartialView("_futurePaymentsPartial", recurringBillsInfo);
        }

        private List<SingleComplexOrder> GenerateShoppingInformation(int currentUserId)
        {
            var plannedShoppingInformations = _context.ComplexOrders
                .Include(co => co.OrderElements)
                .Where(sca => sca.SCOTeamMemberAssignments.Any(sa => sa.UserId == currentUserId))
                .AsNoTracking()
                .ToList();

            return plannedShoppingInformations;
        }
        private List<EventDetails> GenerateEventDetails(int currentUserId, int displayedNumberOfDays)
        {            
            var upcomingEventsInfo = _context.ScheduledEvents
                .Include(se => se.AttachedUsersCollection)
                .Where(se => se.AttachedUsersCollection.Any(auc => auc.UserId == currentUserId) && DateTime.Compare(se.StartDate, DateTime.Now.AddDays(displayedNumberOfDays)) <= 0)
                .AsNoTracking();

            var usersInfo = _context.Users
                .Where(u => upcomingEventsInfo.Any(uei => uei.AttachedUsersCollection.Any(auc => auc.UserId == u.Id)))
                .Select(u => new { u.Id, u.FullName })
                .AsNoTracking()
                .ToDictionary(u => u.Id, u => u.FullName);

            var groupsInfo = _context.Teams
                .Where(t => upcomingEventsInfo.Any(uei => uei.AttachedUsersCollection.Any(auc => auc.TeamId == t.Id)))
                .Select(t => new { t.Id, t.Name })
                .AsNoTracking()
                .ToDictionary(t => t.Id, t => t.Name);

            var eventDetails = new List<EventDetails>();
            foreach (var uei in upcomingEventsInfo)
            {
                var groupId = uei.AttachedUsersCollection.First().TeamId;
                Tuple<int, string> groupInfo = new Tuple<int, string>(groupId, groupsInfo[groupId]);
                List<Tuple<int,string>> participantsInfo = new List<Tuple<int,string>>();
                foreach (var auc in uei.AttachedUsersCollection)
                {
                    participantsInfo.Add(new Tuple<int, string>( auc.UserId, usersInfo[auc.UserId]));
                }
                var eventDetail = new EventDetails
                {
                    Id = uei.Id,
                    Title = uei.Title,
                    Description = uei.Description,
                    StartDate = uei.StartDate,
                    EndDate = uei.EndDate,
                    GroupId = groupInfo.Item1,
                    GroupName = groupInfo.Item2,
                    IsBlocking = uei.IsBlocking,
                    ParticipantIds = participantsInfo.Select(pi => pi.Item1).ToArray(),
                    ParticipantNames = participantsInfo.Select(pi => pi.Item2).ToArray()
                };
                eventDetails.Add(eventDetail);
            }
            eventDetails.Sort((a, b) => { return DateTime.Compare(a.StartDate, b.StartDate); });
            return eventDetails;
        }
        private List<SettlementViewModel.SingleExpense> GenerateUserCredibilities(int currentUserId)
        {
            var userTotalExpenses = _context.TotalExpenses
                .Include(te => te.PartialExpenses)
                .Where(te => te.OwnerId == currentUserId)
                .AsNoTracking();

            //Users with the logged in user
            var userDetails = _context.Users
                .Where(u => userTotalExpenses.Any(ute => ute.PartialExpenses.Any(pe => pe.UserId == u.Id)))
                .AsNoTracking()
                .ToList();

            var teamDetails = _context.Teams
                .Where(t => userTotalExpenses.Any(ute => ute.PartialExpenses.Any(pe => pe.TeamId == t.Id)))
                .AsNoTracking()
                .ToList();

            var userCredibilities = new List<SettlementViewModel.SingleExpense>();

            foreach (var te in userTotalExpenses)
            {
                foreach(var pe in te.PartialExpenses)
                {
                    if(pe.UserId != currentUserId && !pe.Covered)
                    {
                        var singleCredibility = new SettlementViewModel.SingleExpense
                        {
                            UserInfo = new Tuple<int, string>(pe.UserId, userDetails.Find(u => u.Id == pe.UserId).FullName),
                            TeamInfo = new Tuple<int, string>(pe.TeamId, teamDetails.Find(t => t.Id == pe.TeamId).Name),
                            TotalExpenseId = pe.TotalExpenseId,
                            Value = pe.Value,
                            FinalizationDate = te.FinalizationDate
                        };
                        userCredibilities.Add(singleCredibility);
                    }
                }
            }

            return userCredibilities;
        }
        private List<SettlementViewModel.SingleExpense> GenerateUserLiabilities(int currentUserId)
        {
            var userPartialExpenses = _context.PartialExpenses
                .Where(pe => pe.UserId == currentUserId)
                .AsNoTracking()
                .ToList();

            var userTotalExpenses = _context.TotalExpenses
                .Where(te => userPartialExpenses.Any(pe => pe.TotalExpenseId == te.Id))
                .AsNoTracking()
                .ToList();

            //Users with the logged in user
            var userDetails = _context.Users
                .Where(u => userPartialExpenses.Any(pe => pe.UserId == u.Id))
                .AsNoTracking()
                .ToList();

            var teamDetails = _context.Teams
                .Where(t => userPartialExpenses.Any(pe => pe.TeamId == t.Id))
                .AsNoTracking()
                .ToList();

            var userLiabilities = new List<SettlementViewModel.SingleExpense>();

            foreach (var pe in userPartialExpenses)
            {
                TotalExpense totalExpense = userTotalExpenses.Find(te => te.Id == pe.TotalExpenseId);
                var singleExpenseVM = new SettlementViewModel.SingleExpense
                {
                    UserInfo = new Tuple<int, string>(pe.UserId, userDetails.Find(u => u.Id == pe.UserId).FullName),
                    TeamInfo = new Tuple<int, string>(pe.TeamId, teamDetails.Find(t => t.Id == pe.TeamId).Name),
                    Value = pe.Value,
                    FinalizationDate = totalExpense.FinalizationDate,
                    TotalExpenseId = totalExpense.Id
                };
                userLiabilities.Add(singleExpenseVM);
            }

            return userLiabilities;
        }
        private List<FutureExpenseViewModel> GenerateRecurringBills(int currentUserId)
        {
            var recurringBillIds = _context.RecurringBillAssignments
                .Where(rba => rba.UserId == currentUserId)
                .Select(rba => rba.RecurringBillId);

            var recurringBills = _context.RecurringBills
                .Where(rb => recurringBillIds.Any(rbi => rbi == rb.Id))
                .AsNoTracking()
                .ToList();

            var futureExpenseInfo = new List<FutureExpenseViewModel>();
            foreach(var rb in recurringBills)
            {
                var displayedNumberOfDays = 7;
                var nextOccurenceDate = CalculateNextOccurenceDate(rb);
                if(nextOccurenceDate.HasValue && (nextOccurenceDate.Value - DateTime.Now).TotalDays <= displayedNumberOfDays)
                {
                    var singleFutureExpense = new FutureExpenseViewModel
                    {
                        Id = rb.Id,
                        ExpirationDate = rb.ExpirationDate,
                        ExpenseCategory = rb.ExpenseCategory,
                        Frequency = rb.Frequency,
                        Subject = rb.Subject,
                        Value = rb.Value,
                        NextOccurenceDate = nextOccurenceDate.Value
                    };
                    futureExpenseInfo.Add(singleFutureExpense);
                }

            }

            return futureExpenseInfo;
        }
        private DateTime? CalculateNextOccurenceDate(RecurringBill rb)
        {

            if(DateTime.Compare(DateTime.Now, rb.StartDate) <= 0)
            {
                return rb.StartDate;
            }
            else
            {
                if (rb.LastOccurenceDate == null)
                {
                    rb.LastOccurenceDate = DateTime.Now.AddHours(-1);
                }

                //We assume that there was at least one occurence, so LastOccurenceDate is not null
                int weekDaysNumber = 7;
                int numberOfDaysToAdd = 0, numberOfWeeksToAdd = 0, numberOfMonthsToAdd = 0;
                switch (rb.Frequency)
                {
                    case Helpers.Frequency.EveryDay:
                        numberOfDaysToAdd = 1;
                        break;
                    case Helpers.Frequency.EveryWeek:
                        numberOfWeeksToAdd = 1;
                        break;
                    case Helpers.Frequency.Every2Weeks:
                        numberOfWeeksToAdd = 2;
                        break;
                    case Helpers.Frequency.Every3Weeks:
                        numberOfWeeksToAdd = 3;
                        break;
                    case Helpers.Frequency.EveryMonth:
                        numberOfMonthsToAdd = 1;
                        break;
                    case Helpers.Frequency.Every2Months:
                        numberOfMonthsToAdd = 2;
                        break;
                    case Helpers.Frequency.Every3Months:
                        numberOfMonthsToAdd = 3;
                        break;
                    case Helpers.Frequency.Every6Months:
                        numberOfMonthsToAdd = 6;
                        break;
                    case Helpers.Frequency.EveryYear:
                        numberOfMonthsToAdd = 12;
                        break;
                }
                DateTime nextOccurenceDate = rb.LastOccurenceDate.Value.AddDays(numberOfDaysToAdd + weekDaysNumber * numberOfWeeksToAdd).AddMonths(numberOfMonthsToAdd);
                return (DateTime.Compare(nextOccurenceDate, rb.ExpirationDate) <= 0) ? (DateTime?)nextOccurenceDate : null;
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
