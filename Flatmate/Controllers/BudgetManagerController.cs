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
using Microsoft.AspNetCore.Identity;

namespace Flatmate.Controllers
{
    public class BudgetManagerController : Controller
    {
        private readonly FlatmateContext _context;
        private readonly UserManager<User> userManager;

        public BudgetManagerController(UserManager<User> userManager,
                                        FlatmateContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        // GET: BudgetManager
        public async Task<IActionResult> Index()
        {
            var flatmateContext = _context.TotalExpenses.Include(t => t.Owner);
            return View(await flatmateContext.ToListAsync());
        }

        public async Task<IActionResult> ShoppingFinalization(int orderId)
        {
            var currentUserId = (await userManager.GetUserAsync(HttpContext.User)).Id;

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
        public IActionResult FinalizeShopping([Bind("")] ShoppingFinalizationViewModel sfvm)
        {
            return null;
        }
        public async Task<IActionResult> ShoppingRemoval(int orderId)
        {
            //TODO: change to current user Id
            var currentUserId = (await userManager.GetUserAsync(HttpContext.User)).Id;

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
        public async Task<IActionResult> ListShoppingInformation()
        {
            //TODO: change to current user Id
            var currentUserId = (await userManager.GetUserAsync(HttpContext.User)).Id;

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
        public async Task<IActionResult> NewShoppingList([Bind("Subject, ExpenseCategory, GroupId, SingleElementTitles, SingleElementAmounts, SingleElementUnits, ParticipantIds")] ShoppingCreateViewModel scvm)
        {
            //TODO: change to current user Id
            var currentUserId = (await userManager.GetUserAsync(HttpContext.User)).Id;
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
                return RedirectToAction("Index", "BudgetManager"/*, new { id = currentUserId }*/);
            }
            return PartialView("_createNewShoppingListPartial", scvm);
        }

        public IActionResult ListUnitValues()
        {
            return Json(Enum.GetNames(typeof(Unit)));
        }

        // GET: BudgetManager/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var totalExpense = await _context.TotalExpenses
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (totalExpense == null)
            {
                return NotFound();
            }

            return View(totalExpense);
        }

        // GET: BudgetManager/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: BudgetManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Subject,FinalizationDate,Value,Covered,ExpenseCategory,OwnerId")] TotalExpense totalExpense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(totalExpense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", totalExpense.OwnerId);
            return View(totalExpense);
        }

        // GET: BudgetManager/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var totalExpense = await _context.TotalExpenses.FindAsync(id);
            if (totalExpense == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", totalExpense.OwnerId);
            return View(totalExpense);
        }

        // POST: BudgetManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Subject,FinalizationDate,Value,Covered,ExpenseCategory,OwnerId")] TotalExpense totalExpense)
        {
            if (id != totalExpense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(totalExpense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TotalExpenseExists(totalExpense.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", totalExpense.OwnerId);
            return View(totalExpense);
        }

        // GET: BudgetManager/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var totalExpense = await _context.TotalExpenses
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (totalExpense == null)
            {
                return NotFound();
            }

            return View(totalExpense);
        }

        // POST: BudgetManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var totalExpense = await _context.TotalExpenses.FindAsync(id);
            _context.TotalExpenses.Remove(totalExpense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TotalExpenseExists(int id)
        {
            return _context.TotalExpenses.Any(e => e.Id == id);
        }
    }
}
