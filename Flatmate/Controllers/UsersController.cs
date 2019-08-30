using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Flatmate.Models;
using Flatmate.Models.EntityModels;
using Flatmate.Data;

namespace Flatmate.Controllers
{
    public class UsersController : Controller
    {
        private readonly FlatmateContext _context;

        public UsersController(FlatmateContext context) => _context = context;

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .AsNoTracking()
                .ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.TeamAssignments)
                    .ThenInclude(ta => ta.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/GroupDetails/5
        public async Task<IActionResult> GroupDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.TeamAssignments)
                    .ThenInclude(ta => ta.Team)
                        .ThenInclude(t => t.UserAssignments)
                            .ThenInclude(ua => ua.User)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }        

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.SingleAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,FirstName,LastName,EmailAddress")] User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dbUser = _context.Users.Find(user.Id);
                    if(await TryUpdateModelAsync<User>(dbUser, "", u => u.FirstName, u => u.LastName, u => u.EmailAddress))
                    {
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    //TODO: insert logic here & check other errors
                }
                //TODO: change id to current user
                return RedirectToAction(nameof(Details), new { id = 1 });
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
