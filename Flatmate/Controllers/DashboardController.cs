using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Flatmate.ViewModels;
using Flatmate.ViewModels.Dashboard;
using Flatmate.Models.Services;

namespace Flatmate.Controllers
{
    //[Produces("application/json")]
    //[Route("api/Dashboard")]
    public class DashboardController : Controller
    {
        public IUnitOfWork _repository;
        public IMapper _mapper;
        public DashboardService _service;

        public DashboardController(IUnitOfWork uow, IMapper mapper)
        {
            _repository = uow;
            _mapper = mapper;
            _service = new DashboardService();
        }

        public IActionResult Index()
        {
            //var userId = 1; // id will be extracted from user context
            //var teamId = _repository.Users.GetUserTeamId(userId);
            //var team = _repository.Teams.GetTeamWithMembersById(teamId);
            //var credi = _repository.Expenses.GetUserCredibilities(userId);
            //var liab = _repository.Expenses.GetUserLiabilities(userId);
            //var balances = _service.GetAllFlatmatesBalances(userId,
            //                                               team.UsersCollection,
            //                                               liab,
            //                                               credi);
            //var userCrediPerDebitor = _service.GetUserSpecificCredibilityThumbnailModels(userId, credi);
            //var userLiabSplited = _service.GetUserSpecificLiabilityThumbnailModels(userId, liab);
            
            //var DataModel = new DashboardViewModel
            //{
            //    UserLiabilities = userLiabSplited.Select(obj => _mapper.Map<LiabilityExpenseViewModel>(obj)).ToList(),
            //    UserCredibilities = userCrediPerDebitor.Select(obj => _mapper.Map<CredibilityExpenseViewModel>(obj)).ToList(),
            //    FlatmateBalances = balances
            //};

            return View();
        }

        
    }
}