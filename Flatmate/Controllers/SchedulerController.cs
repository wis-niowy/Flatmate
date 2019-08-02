using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AutoMapper;
using Flatmate.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flatmate.Controllers
{
    public class SchedulerController : Controller
    {
        public IUnitOfWork _repository;
        public IMapper _mapper;

        public SchedulerController(IUnitOfWork uow, IMapper mapper)
        {
            _repository = uow;
            _mapper = mapper;
        }
        
        public IActionResult Index()
        {
            return View();
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