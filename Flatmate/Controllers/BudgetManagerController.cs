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
            int sessionduration = 10;
            if (DateTime.Compare(DateTime.Now.AddMinutes(sessionduration), rb.StartDate) <= 0)
            {
                return rb.StartDate;
            }
            else
            {
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
            if(ModelState.IsValid)
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

                foreach(var participantId in rbcvm.ParticipantIds)
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

            ////TODO: check if delete works with only rb remove
            //_context.RecurringBillAssignments.RemoveRange(dbRBAssignments);
            //_context.SaveChanges();

            _context.RecurringBills.Remove(new RecurringBill { Id = billId });
            _context.SaveChanges();

            return RedirectToAction("Index", "BudgetManager", null);
            
        }
    }
}
