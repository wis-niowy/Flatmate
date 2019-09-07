using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Flatmate.Data;
using Flatmate.Models.EntityModels;
using Flatmate.ViewModels.Dashboard;

namespace Flatmate.Controllers
{
    public class TeamsController : Controller
    {
        private readonly FlatmateContext _context;

        public TeamsController(FlatmateContext context)
        {
            _context = context;
        }

        // GET: Teams
        public IActionResult Index()
        {
            //var teams = await _context.Teams.ToListAsync();
            return View();
        }
        public async Task<IActionResult> ListGroupInfo(int userId)
        {
            return Json(await _context.Teams
                .Where(t => t.UserAssignments.Any(u => u.UserId == userId))
                .AsNoTracking()
                .ToListAsync());
        }

        public async Task<IActionResult> ListGroupMembersInfo(int groupId)
        {
            var groupMembers = (from ut in _context.UserPerTeams
                                where ut.TeamId == groupId
                                select ut.UserId).ToList();

            return Json(await _context.Users
                .Where(u => groupMembers.Any(gm => gm == u.Id))
                .AsNoTracking()
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync());
        }
        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Create));
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamName", "InvitationEmails")] TeamCreationData teamCreationData)
        {
            if (ModelState.IsValid)
            {
                var team = new Team
                {
                    Name = teamCreationData.TeamName
                };
                _context.Teams.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teamCreationData);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
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
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
