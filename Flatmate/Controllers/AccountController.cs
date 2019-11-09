using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmate.Data;
using Flatmate.Models.EntityModels;
using Flatmate.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flatmate.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly FlatmateContext context;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 FlatmateContext context)
        {
            this.signInManager  = signInManager;
            this.userManager    = userManager;
            this.context        = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("")]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName      = model.UserName,
                                      EmailAddress  = model.Email,
                                      FirstName     = model.UserFirstName,
                                      LastName      = model.UserLastName,
                                      PhoneNumber   = model.PhoneNumber};
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //_logger.LogInformation("User created a new account with password.");

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");

                    //var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    // dodaj komunikaty o błędach, które wyświetli później element generowany przez asp-validation-summary TagHelper (w Register View)
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = this.context.Users.Select(u => u).Where(u => u.Email == model.Email).FirstOrDefault();
                var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (ModelState.IsValid)
            {
                await signInManager.SignOutAsync();
                //Response.Cookies.Delete("FlatmateCookie");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var userId = (await userManager.GetUserAsync(HttpContext.User)).Id;

            var dto = context.Users.Select(u => u)
                                   .Where(u => u.Id == userId)
                                   .Include(u => u.TeamAssignments)
                                   .ThenInclude(ut => ut.Team)
                                   .FirstOrDefault();
            var model = new UserDetailsViewModel
            {
                FullName        = dto.FullName,
                EmailAddress    = dto.EmailAddress,
                TeamAssignments = new List<UserTeamViewModel>()
            };
            foreach (var team in dto.TeamAssignments)
            {
                model.TeamAssignments.Add(new UserTeamViewModel
                {
                    TeamName = team.Team.Name
                });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = (await userManager.GetUserAsync(HttpContext.User)).Id;

            var dto = context.Users.Select(u => u)
                                   .Where(u => u.Id == userId)
                                   .Include(u => u.TeamAssignments)
                                   .ThenInclude(ut => ut.Team)
                                   .FirstOrDefault();
            var model = new EditUserViewModel
            {
                Id              = dto.Id,
                FirstName       = dto.FirstName,
                LastName        = dto.LastName,
                EmailAddress    = dto.EmailAddress
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("")] EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: zerializacja obiektu do cache zamiast czytania drugi raz
                var dto = await context.Users.Select(u => u)
                                       .Where(u => u.Id == model.Id)
                                       .Include(u => u.TeamAssignments)
                                       .ThenInclude(ut => ut.Team)
                                       .FirstOrDefaultAsync();
                dto.FirstName       = model.FirstName;
                dto.LastName        = model.LastName;
                dto.EmailAddress    = model.EmailAddress;
                context.Users.Update(dto);
                var detailsModel = new UserDetailsViewModel
                {
                    FullName = dto.FullName,
                    EmailAddress = dto.EmailAddress,
                    TeamAssignments = new List<UserTeamViewModel>()
                };
                foreach (var team in dto.TeamAssignments)
                {
                    detailsModel.TeamAssignments.Add(new UserTeamViewModel
                    {
                        TeamName = team.Team.Name
                    });
                }
                await context.SaveChangesAsync();
                return RedirectToAction("Details","Account"/*, new { userId = model.Id }*/);

            }

            return View(model);
        }
    }
}