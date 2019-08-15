using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AutoMapper;
using Flatmate.Models;
using Microsoft.AspNetCore.Mvc;
using Flatmate.Data;
using Microsoft.EntityFrameworkCore;

namespace Flatmate.Controllers
{
    public class SchedulerController : Controller
    {
        private readonly FlatmateContext _context;
        public SchedulerController(FlatmateContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            return View();
        }

        public async Task<IActionResult> ListEventInfo(int id)
        {
            return Json(await _context.ScheduledEvents
                .Where(se => se.AttachedUsersCollection.Any(u => u.UserId == id))
                .AsNoTracking()
                .ToListAsync());
        }

        public async Task<IActionResult> ListGroupInfo(int id)
        {
            return Json(await _context.Teams
                .Where(t => t.UserAssignments.Any(u => u.UserId == id))
                .AsNoTracking()
                .ToListAsync());
        }

        public IActionResult NewEventPartial()
        {
            return PartialView("_createNewEventPartial");
        }

            //public IActionResult Events()
            //{
            //    //int userId = 1;
            //    //var events = _repository.ScheduledEvents.GetAllEvents(userId);
            //    //var jsonResult = Json(events.ToList());
            //    ////var str = JsonConvert.SerializeObject(events, Formatting.Indented);
            //    ////String json = new JsonSerializer().Serialize(jsonResult.Value)
            //    //return jsonResult;
            //}
        }
}