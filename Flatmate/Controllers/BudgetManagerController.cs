using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Flatmate.Data;
using Flatmate.Models.EntityModels;
using Flatmate.ViewModels.BudgetManager;
using Flatmate.Helpers;
using Flatmate.ViewModels.Dashboard;

namespace Flatmate.Controllers
{
    public class BudgetManagerController : Controller
    {
        private readonly FlatmateContext _context;

        public BudgetManagerController(FlatmateContext context)
        {
            _context = context;
        }

        // GET: BudgetManager
        public async Task<IActionResult> Index()
        {
            var flatmateContext = _context.TotalExpenses.Include(t => t.Owner);
            return View(await flatmateContext.ToListAsync());
        }
        public IActionResult ShoppingFinalization(int orderId)
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var plannedShoppingInformations = GenerateFinalizationModel(orderId, currentUserId);
            return PartialView("_finalizeShoppingPartial", plannedShoppingInformations);
        }

        private ShoppingFinalizationViewModel GenerateFinalizationModel(int orderId, int currentUserId)
        {
            var complexOrderInfo = _context.ComplexOrders
                .Where(co => co.Id == orderId)
                .Include(co => co.SCOTeamMemberAssignments)
                .Include(co => co.OrderElements)
                .AsNoTracking()
                .First();

            var groupInfo = _context.Teams
                .Where(t => t.Id == complexOrderInfo.SCOTeamMemberAssignments.First().TeamId)
                .AsNoTracking()
                .First();

            var singleOrderElementDescriptions = new List<string>();
            foreach (var singleElement in complexOrderInfo.OrderElements)
            {
                singleOrderElementDescriptions.Add(singleElement.Title + " - " + singleElement.Amount + singleElement.Unit);
            }

            var userInformation = _context.Users
                .Where(u => complexOrderInfo.SCOTeamMemberAssignments.Any(sco => sco.UserId == u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToArray();

            List<int> userIds = new List<int>();
            List<string> userNames = new List<string>();

            foreach (var userInfo in userInformation)
            {
                userIds.Add(userInfo.Id);
                userNames.Add(userInfo.FullName);
            }

            double initialValue = 0.00;
            var finalizationModel = new ShoppingFinalizationViewModel
            {
                Id = complexOrderInfo.Id,
                ExpenseCategory = complexOrderInfo.ExpenseCategory,
                Subject = complexOrderInfo.Subject,
                Value = initialValue,
                SingleElementDescriptions = singleOrderElementDescriptions.ToArray(),
                GroupId = groupInfo.Id,
                GroupName = groupInfo.Name,
                ParticipantIds = userIds.ToArray(),
                ParticipantNames = userNames.ToArray(),
                DidParticipantsPay = new bool[userNames.Count],
                IsCovered = false
            };

            return finalizationModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalizeShopping([Bind("Id, Subject, Value, IsCovered, ExpenseCategory, GroupId, ParticipantIds, ParticipantsCharge, DidParticipantsPay")] ShoppingFinalizationViewModel sfvm)
        {
            //TODO:check validation after adding it
            if (ModelState.IsValid)
            {
                //TODO: change to currentuserId
                var currentUserId = 1;

                bool isCovered = sfvm.IsCovered ? true : sfvm.DidParticipantsPay.All(pc => pc == true);

                var totalExpense = new TotalExpense
                {
                    Covered = isCovered,
                    FinalizationDate = DateTime.Now,
                    OwnerId = currentUserId,
                    ExpenseCategory = sfvm.ExpenseCategory,
                    Subject = sfvm.Subject,
                    Value = sfvm.Value
                };

                _context.TotalExpenses.Add(totalExpense);
                _context.SaveChanges();

                var partialExpenses = new List<PartialExpense>();

                for (int i = 0; i < sfvm.ParticipantIds.Length; i++)
                {
                    partialExpenses.Add(new PartialExpense
                    {
                        Covered = sfvm.DidParticipantsPay[i],
                        SettlementDate = sfvm.DidParticipantsPay[i] ? DateTime.Now : (DateTime?)null,
                        TeamId = sfvm.GroupId,
                        TotalExpenseId = totalExpense.Id,
                        UserId = sfvm.ParticipantIds[i],
                        Value = sfvm.ParticipantsCharge[i]
                    });
                }

                _context.PartialExpenses.AddRange(partialExpenses);
                _context.SaveChanges();

                _context.ComplexOrders.Remove(new SingleComplexOrder { Id = sfvm.Id });
                _context.SaveChanges();

                return RedirectToAction("Index", "BudgetManager", new { id = currentUserId });
            }

            return PartialView("_finalizeShoppingPartial", sfvm);
        }
        public IActionResult ShoppingRemoval(int orderId)
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var plannedShoppingInformations = GenerateShoppingRemovalModel(orderId, currentUserId);
            return PartialView("_deleteShoppingPartial", plannedShoppingInformations);
        }

        private ShoppingRemovalViewModel GenerateShoppingRemovalModel(int orderId, int currentUserId)
        {
            var complexOrderInfo = _context.ComplexOrders
                .Where(co => co.Id == orderId)
                .Include(co => co.SCOTeamMemberAssignments)
                .Include(co => co.OrderElements)
                .AsNoTracking()
                .First();

            var groupInfo = _context.Teams
                .Where(t => t.Id == complexOrderInfo.SCOTeamMemberAssignments.First().TeamId)
                .AsNoTracking()
                .First();

            var singleOrderElementDescriptions = new List<string>();
            foreach (var singleElement in complexOrderInfo.OrderElements)
            {
                singleOrderElementDescriptions.Add(singleElement.Title + " - " + singleElement.Amount + singleElement.Unit);
            }

            var userInformation = _context.Users
                .Where(u => complexOrderInfo.SCOTeamMemberAssignments.Any(sco => sco.UserId == u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToArray();

            List<int> userIds = new List<int>();
            List<string> userNames = new List<string>();

            foreach (var userInfo in userInformation)
            {
                userIds.Add(userInfo.Id);
                userNames.Add(userInfo.FullName);
            }

            var removalModel = new ShoppingRemovalViewModel
            {
                Id = complexOrderInfo.Id,
                ExpenseCategory = complexOrderInfo.ExpenseCategory,
                Subject = complexOrderInfo.Subject,
                SingleElementDescriptions = singleOrderElementDescriptions.ToArray(),
                GroupId = groupInfo.Id,
                GroupName = groupInfo.Name,
                ParticipantIds = userIds.ToArray(),
                ParticipantNames = userNames.ToArray()
            };

            return removalModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteShopping([Bind("Id")] ShoppingRemovalViewModel srvm)
        {
            //TODO:check validation after adding it
            if (ModelState.IsValid)
            {
                var dbSingleElements = _context.OrderElements
                    .Where(oe => oe.SCOId == srvm.Id)
                    .ToList();

                //TODO: check if multiple savechanges can be removed
                _context.OrderElements.RemoveRange(dbSingleElements);
                _context.SaveChanges();

                var dbSCOutas = _context.OrdersAssignments
                    .Where(oa => oa.SCOId == srvm.Id)
                    .ToList();

                _context.OrdersAssignments.RemoveRange(dbSCOutas);
                _context.SaveChanges();

                _context.ComplexOrders.Remove(new SingleComplexOrder { Id = srvm.Id });
                _context.SaveChanges();

                return RedirectToAction("Index", "BudgetManager", null);
            }

            return PartialView("_finalizeShoppingPartial", srvm);
        }
        public IActionResult ListShoppingInformation()
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var plannedShoppingInformations = GenerateShoppingInformation(currentUserId);
            return PartialView("_plannedShoppingPartial", plannedShoppingInformations);
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

        public IActionResult NewShoppingList()
        {
            var shoppingCreateVM = new ShoppingCreateViewModel { };
            return PartialView("_createNewShoppingListPartial", shoppingCreateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewShoppingList([Bind("Subject, ExpenseCategory, GroupId, SingleElementTitles, SingleElementAmounts, SingleElementUnits, ParticipantIds")] ShoppingCreateViewModel scvm)
        {
            //TODO: change to current user Id
            int currentUserId = 1;
            if (ModelState.IsValid)
            {
                //Dividing data between new event and user assignments.
                var sco = new SingleComplexOrder
                {
                    Subject = scvm.Subject,
                    ExpenseCategory = scvm.ExpenseCategory,
                    CreationDate = DateTime.Now
                };

                _context.ComplexOrders.Add(sco);
                _context.SaveChanges();

                var soeToAppend = new List<SingleOrderElement>();

                //Creating per user invitations
                for (int i = 0; i < scvm.SingleElementTitles.Length; i++)
                {
                    soeToAppend.Add(new SingleOrderElement
                    {
                        Title = scvm.SingleElementTitles[i],
                        Amount = scvm.SingleElementAmounts[i],
                        Unit = scvm.SingleElementUnits[i],
                        SCOId = sco.Id
                    });
                }

                _context.OrderElements.AddRange(soeToAppend);
                _context.SaveChanges();

                var scoutas = new List<SCOUserTeamAssignment>
                {
                    new SCOUserTeamAssignment
                    {
                        SCOId = sco.Id,
                        UserId = currentUserId,
                        TeamId = scvm.GroupId
                    }
                };

                for (int i = 0; i < scvm.ParticipantIds.Length; i++)
                {
                    var scouta = new SCOUserTeamAssignment
                    {
                        SCOId = sco.Id,
                        UserId = scvm.ParticipantIds[i],
                        TeamId = scvm.GroupId
                    };
                    scoutas.Add(scouta);
                }

                _context.OrdersAssignments.AddRange(scoutas);
                _context.SaveChanges();

                //TODO: change for the currentUserId
                return RedirectToAction("Index", "BudgetManager", null);
            }
            return PartialView("_createNewShoppingListPartial", scvm);
        }

        public IActionResult ListUnitValues()
        {
            return Json(Enum.GetNames(typeof(Unit)));
        }

        public IActionResult ListRecurringBills()
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var recurringBillsInfo = GenerateRecurringBills(currentUserId);
            return PartialView("_futurePaymentsBMPartial", recurringBillsInfo);
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
            foreach (var rb in recurringBills)
            {
                var displayedNumberOfDays = 7;
                var nextOccurenceDate = CalculateNextOccurenceDate(rb);
                if (nextOccurenceDate.HasValue && (nextOccurenceDate.Value - DateTime.Now).TotalDays <= displayedNumberOfDays)
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
            //TODO; change to just DateTime.Now
            int sessionduration = 10;
            if (DateTime.Compare(DateTime.Now.AddMinutes(sessionduration), rb.StartDate) <= 0)
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
        public IActionResult NewRecurringBill()
        {
            var RBCreateVM = new BillingViewModel { };
            return PartialView("_newRecurringBillPartial", RBCreateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewRecurringBill([Bind("Subject, Value, Frequency, ExpenseCategory, StartDate, ExpirationDate, GroupId, ParticipantIds")] BillingViewModel rbcvm)
        {
            if (ModelState.IsValid)
            {
                var newRecurringBill = new RecurringBill
                {
                    Id = rbcvm.Id,
                    CreationDate = DateTime.Now,
                    ExpenseCategory = rbcvm.ExpenseCategory,
                    ExpirationDate = rbcvm.ExpirationDate,
                    Frequency = rbcvm.Frequency,
                    StartDate = rbcvm.StartDate,
                    Subject = rbcvm.Subject,
                    Value = rbcvm.Value
                };

                _context.RecurringBills.Add(newRecurringBill);
                _context.SaveChanges();

                var currentUserId = 1;

                var billAssignments = new List<RecurringBillPerTeamMember>()
                {
                    new RecurringBillPerTeamMember
                    {
                        TeamId = rbcvm.GroupId,
                        RecurringBillId = newRecurringBill.Id,
                        UserId = currentUserId
                    }
                };

                foreach (var participantId in rbcvm.ParticipantIds)
                {
                    billAssignments.Add(new RecurringBillPerTeamMember
                    {
                        TeamId = rbcvm.GroupId,
                        RecurringBillId = newRecurringBill.Id,
                        UserId = participantId
                    });
                }

                _context.RecurringBillAssignments.AddRange(billAssignments);
                _context.SaveChanges();
                //TODO: change for the currentUserId
                return RedirectToAction("Index", "BudgetManager", null);
            }
            return PartialView("_newRecurringBillPartial", rbcvm);
        }

        public IActionResult BillEdit(int RBId)
        {
            var recurringBill = _context.RecurringBills
                .Where(rb => rb.Id == RBId)
                .Include(rb => rb.RecipientsCollection)
                .AsNoTracking()
                .First();

            var recipientsIds = recurringBill.RecipientsCollection
                .Select(r => r.UserId);

            var recipientsNames = _context.Users
                .Where(u => recipientsIds.Any(ri => ri == u.Id))
                .AsNoTracking()
                .Select(u => u.FullName);

            var groupId = recurringBill.RecipientsCollection
                .First().TeamId;

            var groupName = _context.Teams.Find(groupId).Name;

            var RBEditVM = new BillingViewModel
            {
                CreationDate = recurringBill.CreationDate,
                ExpenseCategory = recurringBill.ExpenseCategory,
                ExpirationDate = recurringBill.ExpirationDate,
                Frequency = recurringBill.Frequency,
                GroupId = groupId,
                GroupName = groupName,
                Id = recurringBill.Id,
                LastOccurenceDate = recurringBill.LastOccurenceDate,
                ParticipantIds = recipientsIds.ToArray(),
                ParticipantNames = recipientsNames.ToArray(),
                StartDate = recurringBill.StartDate,
                Subject = recurringBill.Subject,
                Value = recurringBill.Value
            };

            return PartialView("_editRecurringBillPartial", RBEditVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRecurringBill([Bind("Id, Subject, Value, StartDate, ExpirationDate, " +
            "ExpenseCategory, Frequency, GroupId, ParticipantIds")] BillingViewModel bevm)
        {
            if (ModelState.IsValid)
            {
                var recurringBillToUpdate = _context.RecurringBills.Find(bevm.Id);

                if (await TryUpdateModelAsync<RecurringBill>(recurringBillToUpdate, "",
                    rb => rb.Subject, rb => rb.Value, rb => rb.StartDate,
                    rb => rb.ExpirationDate, rb => rb.ExpenseCategory, rb => rb.Frequency))
                {
                    var currentRecipients = _context.RecurringBillAssignments
                        .Where(rba => rba.RecurringBillId == bevm.Id)
                        .ToList();

                    //TODO: change to current user id
                    var currentUserId = 1;
                    var recipientsInfo = new List<RecurringBillPerTeamMember>()
                    {
                        new RecurringBillPerTeamMember
                        {
                            TeamId = bevm.GroupId,
                            UserId = currentUserId,
                            RecurringBillId = bevm.Id
                        }
                    };
                    for (int i = 0; i < bevm.ParticipantIds.Length; i++)
                    {
                        recipientsInfo.Add(new RecurringBillPerTeamMember
                        {
                            UserId = bevm.ParticipantIds[i],
                            TeamId = bevm.GroupId,
                            RecurringBillId = bevm.Id
                        });
                    }

                    _context.RecurringBillAssignments.RemoveRange(currentRecipients);
                    _context.RecurringBillAssignments.AddRange(recipientsInfo);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index", "BudgetManager", null);
            }

            return PartialView("_editRecurringBillPartial", bevm);
        }
        public IActionResult BillRemoval(int RBId)
        {
            var RBInfo = GenerateRBRemovalModel(RBId);
            return PartialView("_deleteRecurringBillPartial", RBInfo);
        }
        private BillingViewModel GenerateRBRemovalModel(int RBId)
        {
            var recurringBillToDelete = _context.RecurringBills
                .Where(rb => rb.Id == RBId)
                .Include(rb => rb.RecipientsCollection)
                .AsNoTracking()
                .First();

            var recipientIds = recurringBillToDelete.RecipientsCollection
                .Select(r => r.UserId);

            var recipientNames = _context.Users.Where(u => recipientIds.Any(ri => ri == u.Id)).Select(u => u.FullName);

            var groupId = recurringBillToDelete.RecipientsCollection.First().TeamId;
            var groupName = _context.Teams.Where(t => t.Id == groupId).First().Name;

            var billingViewModel = new BillingViewModel
            {
                CreationDate = recurringBillToDelete.CreationDate,
                ExpenseCategory = recurringBillToDelete.ExpenseCategory,
                ExpirationDate = recurringBillToDelete.ExpirationDate,
                Frequency = recurringBillToDelete.Frequency,
                GroupId = groupId,
                GroupName = groupName,
                Id = recurringBillToDelete.Id,
                LastOccurenceDate = recurringBillToDelete.LastOccurenceDate,
                ParticipantIds = recipientIds.ToArray(),
                ParticipantNames = recipientNames.ToArray(),
                StartDate = recurringBillToDelete.StartDate,
                Subject = recurringBillToDelete.Subject,
                Value = recurringBillToDelete.Value
            };

            return billingViewModel;
        }

        [HttpDelete]
        public IActionResult DeleteBill(int billId)
        {
            var dbRBAssignments = _context.RecurringBillAssignments
                .Where(rba => rba.RecurringBillId == billId)
                .ToList();

            _context.RecurringBills.Remove(new RecurringBill { Id = billId });
            _context.SaveChanges();

            return RedirectToAction("Index", "BudgetManager", null);

        }

        //TODO: mix with method from HomeControler
        public IActionResult ListCoveredSettlementInformation(int displayedDays)
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var userCredibilities = GenerateCoveredUserCredibilities(currentUserId, displayedDays);
            var userLiabilities = GenerateCoveredUserLiabilities(currentUserId, displayedDays);

            userCredibilities.Sort((a, b) => { return DateTime.Compare(b.FinalizationDate, a.FinalizationDate); });
            userLiabilities.Sort((a, b) => { return DateTime.Compare(b.FinalizationDate, a.FinalizationDate); });

            var settlementVM = new SettlementViewModel
            {
                UserCredibilities = userCredibilities,
                UserLiabilities = userLiabilities
            };

            return PartialView("_listCoveredExpensesPartial", settlementVM);
        }
        private List<SettlementViewModel.SingleExpense> GenerateCoveredUserCredibilities(int currentUserId, int displayedDays)
        {
            var userTotalExpenses = _context.TotalExpenses
                .Include(te => te.PartialExpenses)
                .Where(te => te.OwnerId == currentUserId)
                .AsNoTracking()
                .ToList();

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
                foreach (var pe in te.PartialExpenses)
                {
                    if (pe.UserId != currentUserId && pe.Covered && DateTime.Compare(pe.SettlementDate.Value, DateTime.Now.AddDays(-displayedDays)) >= 0)
                    {
                        var singleCredibility = new SettlementViewModel.SingleExpense
                        {
                            UserInfo = new Tuple<int, string>(pe.UserId, userDetails.Find(u => u.Id == pe.UserId).FullName),
                            TeamInfo = new Tuple<int, string>(pe.TeamId, teamDetails.Find(t => t.Id == pe.TeamId).Name),
                            TotalExpenseId = pe.TotalExpenseId,
                            Value = pe.Value,
                            FinalizationDate = pe.SettlementDate.Value
                        };
                        userCredibilities.Add(singleCredibility);
                    }
                }
            }

            return userCredibilities;
        }
        private List<SettlementViewModel.SingleExpense> GenerateCoveredUserLiabilities(int currentUserId, int displayedDays)
        {
            var chosenDate = DateTime.Now.AddDays(-displayedDays);
            var userPartialExpenses = _context.PartialExpenses
                .Where(pe => pe.UserId == currentUserId && pe.Covered && DateTime.Compare(pe.SettlementDate.Value, chosenDate) > 0)
                .AsNoTracking()
                .ToList();

            var userTotalExpenses = _context.TotalExpenses
                .Where(te => userPartialExpenses.Any(pe => pe.TotalExpenseId == te.Id) && te.OwnerId != currentUserId)
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

            foreach (var totalExpense in userTotalExpenses)
            {
                PartialExpense pe = userPartialExpenses.Find(partial => partial.TotalExpenseId == totalExpense.Id);
                var singleExpenseVM = new SettlementViewModel.SingleExpense
                {
                    UserInfo = new Tuple<int, string>(pe.UserId, userDetails.Find(u => u.Id == pe.UserId).FullName),
                    TeamInfo = new Tuple<int, string>(pe.TeamId, teamDetails.Find(t => t.Id == pe.TeamId).Name),
                    Value = pe.Value,
                    FinalizationDate = pe.SettlementDate.Value,
                    TotalExpenseId = totalExpense.Id
                };
                userLiabilities.Add(singleExpenseVM);
            }

            return userLiabilities;
        }
        public IActionResult ListCoveredExpenses(int displayedDays)
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var userCredibilities = GenerateCoveredUserCredibilities(currentUserId, displayedDays);
            var userLiabilities = GenerateCoveredUserLiabilities(currentUserId, displayedDays);

            userCredibilities.Sort((a, b) => { return DateTime.Compare(a.FinalizationDate, b.FinalizationDate); });
            userLiabilities.Sort((a, b) => { return DateTime.Compare(a.FinalizationDate, b.FinalizationDate); });

            var perDateCredibilities = userCredibilities.GroupBy(uc => uc.FinalizationDate.Date);
            var perDateLiabilities = userLiabilities.GroupBy(uc => uc.FinalizationDate.Date);

            var expenseHistoryInfo = new { perDateCredibilities, perDateLiabilities };
            return Json(expenseHistoryInfo);
        }
        public IActionResult ListCurrentSettlementInformation()
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var userCredibilities = GenerateCurrentUserCredibilities(currentUserId);
            var userLiabilities = GenerateCurrentUserLiabilities(currentUserId);

            userCredibilities.Sort((a, b) => { return DateTime.Compare(b.FinalizationDate, a.FinalizationDate); });
            userLiabilities.Sort((a, b) => { return DateTime.Compare(b.FinalizationDate, a.FinalizationDate); });

            var settlementVM = new SettlementViewModel
            {
                UserCredibilities = userCredibilities,
                UserLiabilities = userLiabilities
            };

            return PartialView("_listCurrentExpensesPartial", settlementVM);
        }
        private List<SettlementViewModel.SingleExpense> GenerateCurrentUserCredibilities(int currentUserId)
        {
            var userTotalExpenses = _context.TotalExpenses
                .Include(te => te.PartialExpenses)
                .Where(te => te.OwnerId == currentUserId)
                .AsNoTracking()
                .ToList();

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
                foreach (var pe in te.PartialExpenses)
                {
                    if (pe.UserId != currentUserId && !pe.Covered)
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
        private List<SettlementViewModel.SingleExpense> GenerateCurrentUserLiabilities(int currentUserId)
        {
            var userPartialExpenses = _context.PartialExpenses
                .Where(pe => pe.UserId == currentUserId && !pe.Covered)
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
        public IActionResult ListCurrentExpenses()
        {
            //TODO: change to current user Id
            var currentUserId = 1;

            var userCredibilities = GenerateCurrentUserCredibilities(currentUserId);
            var userLiabilities = GenerateCurrentUserLiabilities(currentUserId);

            userCredibilities.Sort((a, b) => { return DateTime.Compare(a.FinalizationDate, b.FinalizationDate); });
            userLiabilities.Sort((a, b) => { return DateTime.Compare(a.FinalizationDate, b.FinalizationDate); });

            var perDateCredibilities = userCredibilities.GroupBy(uc => uc.FinalizationDate.Date);
            var perDateLiabilities = userLiabilities.GroupBy(uc => uc.FinalizationDate.Date);

            var expenseHistoryInfo = new { perDateCredibilities, perDateLiabilities };
            return Json(expenseHistoryInfo);
        }
        [HttpPost]
        public IActionResult MarkExpensesAsCovered(int [] expenseIds, int [] userIds, int [] groupIds)
        {
            List<int> expenseIdList = expenseIds.ToList(), 
                userIdList = userIds.ToList(), groupIdList = groupIds.ToList();

            var partialExpensesToCover = _context.PartialExpenses
                .Where(pe => groupIds.Any(gi => gi == pe.TeamId) 
                        && userIdList.Any(ui => ui == pe.UserId) 
                        && expenseIdList.Any(ei => ei == pe.TotalExpenseId));

            foreach(var petu in partialExpensesToCover)
            {
                petu.Covered = true;
                petu.SettlementDate = DateTime.Now;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "BudgetManager", null);
        }
        public IActionResult ExpenseDetails(int totalExpenseId, int userId, int groupId)
        {
            var partialExpenseInfo = _context.PartialExpenses
                .Where(pe => pe.TotalExpenseId == totalExpenseId && pe.UserId == userId && pe.TeamId == groupId)
                .First();

            var totalExpenseInfo = _context.TotalExpenses
                .Where(te => te.Id == partialExpenseInfo.TotalExpenseId)
                .First();

            var groupName = _context.Teams
                .Where(t => t.Id == groupId)
                .First().Name;

            var userName = _context.Users
                .Where(u => u.Id == userId)
                .First().FullName;

            var ExpenseDetailsVM = new ExpenseDetailsPartialView
            {
                Subject = totalExpenseInfo.Subject,
                Value = partialExpenseInfo.Value,
                ExpenseCategory = totalExpenseInfo.ExpenseCategory,
                GroupName = groupName,
                UserName = userName
            };
            return PartialView("_detailsSettlementPartial", ExpenseDetailsVM);
        }
        public IActionResult NewExpense()
        {
            var ExpenseCreateVM = new ExpenseCreateViewModel { };
            return PartialView("_newExpensePartial", ExpenseCreateVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewExpense([Bind("Subject, Value, IsCovered, ExpenseCategory, GroupId, ParticipantIds, ParticipantsCharge, DidParticipantsPay")] ExpenseCreateViewModel ecvm)
        {
            //TODO:check validation after adding it
            if (ModelState.IsValid)
            {
                //TODO: change to currentuserId
                var currentUserId = 1;

                bool isCovered = ecvm.IsCovered ? true : ecvm.DidParticipantsPay.All(pc => pc == true);

                var totalExpense = new TotalExpense
                {
                    Covered = isCovered,
                    FinalizationDate = DateTime.Now,
                    OwnerId = currentUserId,
                    ExpenseCategory = ecvm.ExpenseCategory,
                    Subject = ecvm.Subject,
                    Value = ecvm.Value
                };

                _context.TotalExpenses.Add(totalExpense);
                _context.SaveChanges();

                var partialExpenses = new List<PartialExpense>();

                for (int i = 0; i < ecvm.ParticipantIds.Length; i++)
                {
                    partialExpenses.Add(new PartialExpense
                    {
                        Covered = ecvm.DidParticipantsPay[i],
                        SettlementDate = ecvm.DidParticipantsPay[i] ? DateTime.Now : (DateTime?)null,
                        TeamId = ecvm.GroupId,
                        TotalExpenseId = totalExpense.Id,
                        UserId = ecvm.ParticipantIds[i],
                        Value = ecvm.ParticipantsCharge[i]
                    });
                }

                _context.PartialExpenses.AddRange(partialExpenses);
                _context.SaveChanges();
                
                return RedirectToAction("Index", "BudgetManager", new { id = currentUserId });
            }

            return PartialView("_newExpensePartial", ecvm);
        }
    }
}
