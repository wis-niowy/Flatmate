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
using Flatmate.Models.EntityModels;
using Flatmate.ViewModels.Scheduler;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

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

        public async Task<IActionResult> ListPeriodEventInfo(int id, DateTime weekStart, DateTime weekEnd)
        {
            return Json(await _context.ScheduledEvents
                .Where(se => se.AttachedUsersCollection.Any(u => u.UserId == id) && DateTime.Compare(weekStart,se.StartDate) <= 0 && DateTime.Compare(weekEnd, se.StartDate) >= 0)
                .AsNoTracking()
                .ToListAsync());
        }

        public IActionResult NewEvent()
        {
            var eventInvitation = new EventDetails { };
            return PartialView("_createNewEventPartial", eventInvitation);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        //TODO: delete unnecessary fields in model or change model
        public async Task<IActionResult> EditEventDetails([Bind("Id, Title, Description, IsBlocking, StartDate, EndDate, GroupId, ParticipantIds")] EventDetails eventDetails)
        {
            int eventId = eventDetails.Id, teamId = eventDetails.GroupId;
            var eventToUpdate = _context.ScheduledEvents.Find(eventId);

            //TODO: Add some serious data when validation added
            eventDetails.GroupName = "GroupName";
            var pNames = new string[(eventDetails.ParticipantIds ?? new int[] { }).Length];
            for (int i = 0; i < (eventDetails.ParticipantIds ?? new int[] { }).Length; i++)
            {
                pNames[i] = "Leo";
            }
            eventDetails.ParticipantNames = pNames;

            if (await TryUpdateModelAsync<ScheduledEvent>(eventToUpdate, "",
                e => e.Title, e => e.Description, e => e.IsBlocking, e => e.StartDate, e => e.EndDate))
            {
                try
                {
                    var usersParticipations = _context.ScheduledEventUsers.Where(seu => seu.ScheduledEventId == eventDetails.Id);
                    var ownerId = usersParticipations.First(p => p.IsOwner == true).UserId;
                    _context.ScheduledEventUsers.RemoveRange(usersParticipations);

                    var SEUsToInsert = new List<ScheduledEventUser>();
                    foreach (var userId in eventDetails.ParticipantIds ?? new int[] { })
                    {
                        SEUsToInsert.Add(new ScheduledEventUser
                        {
                            ScheduledEventId = eventId,
                            TeamId = teamId,
                            UserId = userId,
                            IsOwner = false
                        });
                    }

                    //TODO: change to currentUserId
                    var defaultUserId = 1;
                    SEUsToInsert.Add(new ScheduledEventUser
                    {
                        ScheduledEventId = eventId,
                        TeamId = teamId,
                        UserId = defaultUserId,
                        IsOwner = true
                    });

                    await _context.ScheduledEventUsers.AddRangeAsync(SEUsToInsert);
                    await _context.SaveChangesAsync();
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return PartialView("_showEventDetailsPartial", eventDetails);
        }

        public IActionResult EventDetails(int eventId)
        {
            var scheduledEvent = _context.ScheduledEvents
                .Include(se => se.AttachedUsersCollection)
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == eventId);

            var eventUser = _context.ScheduledEventUsers
                .AsNoTracking()
                .FirstOrDefault(seu => seu.ScheduledEventId == eventId);

            var eventTeam = _context.Teams
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == eventUser.TeamId);

            var participantsInfo = _context.Users
                .Where(u => scheduledEvent.AttachedUsersCollection.Any(p => p.UserId == u.Id))
                .Select(u => new Tuple<int, string>(u.Id, u.FullName))
                .AsNoTracking()
                .ToArray();

            var groupInfo = new Tuple<int, string>(eventTeam.Id, eventTeam.Name);

            var eventDetails = new EventDetails
            {
                Id = scheduledEvent.Id,
                Title = scheduledEvent.Title,
                Description = scheduledEvent.Description,
                StartDate = scheduledEvent.StartDate,
                EndDate = scheduledEvent.EndDate,
                IsBlocking = scheduledEvent.IsBlocking,
                GroupId = groupInfo.Item1,
                GroupName = groupInfo.Item2,
                ParticipantIds = participantsInfo.Select(pi => pi.Item1).ToArray(),
                ParticipantNames = participantsInfo.Select(pi => pi.Item2).ToArray()
            };

            return PartialView("_showEventDetailsPartial", eventDetails);
        }

        [HttpDelete]
        public IActionResult DeleteEvent(int eventId)
        {
            try
            {
                var userParticipations = _context.ScheduledEventUsers
                    .Where(seu => seu.ScheduledEventId == eventId)
                    .ToList();

                _context.ScheduledEventUsers.RemoveRange(userParticipations);
                _context.ScheduledEvents.Remove(new ScheduledEvent { Id = eventId });

                _context.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //TODO: change the id to the currentUser and signal error
            }
            return RedirectToAction(nameof(Index), new { id = 1 });
        }

        [HttpPut]
        public async Task<IActionResult> UnsubFromEvent(int eventId)
        {
            try
            {
                var currentUserId = 1;
                var userParticipations = _context.ScheduledEventUsers
                    .Where(seu => seu.ScheduledEventId == eventId)
                    .ToList();

                var currentUserParticipation = userParticipations.Find(up => up.UserId == currentUserId);

                bool isEventToDelete = userParticipations.Count == 1,
                    isCurrentUserEventOwner = currentUserParticipation.IsOwner;
                _context.ScheduledEventUsers.Remove(currentUserParticipation);

                if (isEventToDelete)
                {
                    _context.ScheduledEvents.Remove(new ScheduledEvent { Id = eventId });
                }
                else
                {
                    if (isCurrentUserEventOwner)
                    {
                        var ownerToBe = userParticipations.Find(up => up.UserId != currentUserId);
                        ownerToBe.IsOwner = true;

                        await TryUpdateModelAsync<ScheduledEventUser>(ownerToBe, "",
                            e => e.IsOwner);
                    }
                }

                _context.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //TODO: change the id to the currentUser and signal error
            }
            return RedirectToAction(nameof(Index), new { id = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewEvent([Bind("Title, Description, IsBlocking, StartDate, EndDate, GroupId, ParticipantIds")] EventDetails eventInvitation)
        {
            if (ModelState.IsValid)
            {
                //Dividing data between new event and user assignments.
                var scheduledEvent = new ScheduledEvent
                {
                    Title = eventInvitation.Title,
                    Description = eventInvitation.Description,
                    IsBlocking = eventInvitation.IsBlocking,
                    StartDate = eventInvitation.StartDate,
                    EndDate = eventInvitation.EndDate
                };
                _context.ScheduledEvents.Add(scheduledEvent);
                _context.SaveChanges();
                
                var seuToAppend = new List<ScheduledEventUser>
                {

                    //Creating event owner participation
                    new ScheduledEventUser
                    {
                        IsOwner = true,
                        ScheduledEventId = scheduledEvent.Id,
                        TeamId = eventInvitation.GroupId,

                        //TODO: change for cookie identification
                        UserId = 1
                    }
                };

                //Creating per user invitations
                foreach (var userId in eventInvitation.ParticipantIds ?? new int[] { })
                {
                    seuToAppend.Add(new ScheduledEventUser
                    {
                        IsOwner = false,
                        TeamId = eventInvitation.GroupId,
                        ScheduledEventId = scheduledEvent.Id,
                        UserId = userId
                    });
                }

                _context.ScheduledEventUsers.AddRange(seuToAppend);
                _context.SaveChanges();

                //TODO: change for the currentUserId
                return RedirectToAction("Index", "Scheduler", new { id = 1 });
            }
            return PartialView("_createNewEventPartial", eventInvitation);
        }

        public async Task<IActionResult> ListGroupInfo(int userId)
        {
            return Json(await _context.Teams
                .Where(t => t.UserAssignments.Any(u => u.UserId == userId))
                .AsNoTracking()
                .ToListAsync());
        }
        public async Task<IActionResult> FindGroupName(int groupId)
        {
            return Json(await _context.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == groupId));
        }

        public IActionResult NewEventPartial()
        {
            return PartialView("_createNewEventPartial");
        }        
    }
}