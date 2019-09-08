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
            foreach(var singleElement in complexOrderInfo.OrderElements)
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

                //var singleElements = new List<SingleOrderElement>();
                //foreach(var element in dbSingleElements)
                //{
                //    singleElements.Add(new SingleOrderElement { Id = element.Id });
                //}

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
                for(int i = 0; i < scvm.SingleElementTitles.Length; i++)
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
    }
}
