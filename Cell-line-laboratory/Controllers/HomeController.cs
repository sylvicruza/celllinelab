using ArtisanELearningSystem.Services;
using Cell_line_laboratory.Models;
using Cell_line_laboratory.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Cell_line_laboratory.Models.ForgotPasswordModel;
using System.Security.Claims;
using Cell_line_laboratory.Entities;
using Cell_line_laboratory.Exceptions;
using System.Security.Principal;
using Cell_line_laboratory.Data;
using Microsoft.EntityFrameworkCore;

namespace Cell_line_laboratory.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginService _loginService;
        private readonly Cell_line_laboratoryContext _context;


        public HomeController(ILogger<HomeController> logger, ILoginService loginService, Cell_line_laboratoryContext context)
        {
            _logger = logger;
            _loginService = loginService;
            _context = context;
        }

        public IActionResult Index(string? userInfo)
        {
            if (!string.IsNullOrEmpty(userInfo))
            {
                ModelState.AddModelError(string.Empty, userInfo);
            }
            ViewBag.EnzymesCount = _context?.Enzyme?.Count(e => !e.IsMarkedForDeletion);
            ViewBag.ChemicalCount = _context?.Chemical?.Count(c => !c.IsMarkedForDeletion);
            ViewBag.CelllineCount = _context?.CellLine?.Count(c => !c.IsMarkedForDeletion);
            ViewBag.PlasmidCount = _context?.PlasmidCollection?.Count(p => !p.IsMarkedForDeletion);
            ViewBag.AssignedTask = _context?.AssignTask?.Include(a => a.User).ToListAsync().Result;
            return View();
        }

        public IActionResult SignIn(string? userInfo)
        {
            if (!string.IsNullOrEmpty(userInfo))
            {
                ModelState.AddModelError(string.Empty, userInfo);
            }



            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = "*Some fields missing";
                return RedirectToAction("SignIn", "Home", new { userInfo = message });
            } 
            
            try
            {
                var principal = await _loginService.Authenticate(model);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                    IsPersistent = model.RememberMe
                };

                // Authenticate the user and create an authentication cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal, authProperties);

                // Redirect to the home page or to the URL the user originally requested
                return redirectToDashboard(principal);
            }
            catch (AuthenticationException ex)
            {
                // If the user's credentials are not valid, display an error message               
                return RedirectToAction("SignIn", "Home", new { userInfo = ex.Message });

            }
            catch (Exception ex)
            {
                return RedirectToAction("SignIn", "Home", new { userInfo = ex.Message });
            }

        }

        public RedirectToActionResult redirectToDashboard(ClaimsPrincipal principal)
        {
            // Retrieve the role claim from the authenticated user's ClaimsPrincipal
            var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roleClaim == null)
            {
                var message = "Error retrieving the role claim";
                return RedirectToAction("SignIn", "Home", new { userInfo = message });
            }

            else if (!string.IsNullOrEmpty(roleClaim.Value))
            {
                return RedirectToAction("Index", "Home");
            }

            else
            {
                var message = "Role not found";
                return RedirectToAction("SignIn", "Home", new { userInfo = message });
            }


        }


        public async Task<IActionResult> Logout()
        {
            // Sign the user out and delete the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the home page or to the URL the user originally requested
            return RedirectToAction("SignIn", "Home");
        }


        public IActionResult ForgotPassword()
        {
            ForgotPasswordModel model = new ForgotPasswordModel();
            model.Status = ForgotPasswordStatus.None;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {

            if (string.IsNullOrWhiteSpace(model.Input.Email))
            {
                var message = "*Email fields missing";
                return RedirectToAction("ForgotPassword", "Home", new { userInfo = message });
            }

            var user = await _loginService.GetUserByEmailAsync(model.Input.Email.Trim());
            var userObject = (dynamic)user;
            if (userObject.Status == ForgotPasswordStatus.Error)
            {
                model.Status = userObject.Status;
                model.ErrorMessage = userObject.ErrorMessage;
                return View(model);
            }
            else if (userObject.Status == ForgotPasswordStatus.Success)
            {
                model.Status = userObject.Status;
            }

            return View(model);
        }

        private async Task<User> GetCurrentUser()
        {
            var email = User?.Identity?.Name;

            var user = await _loginService.GetUserByEmailAsync(email);
            var userObject = (dynamic)user;
            if (userObject.User == null)
            {
                throw new UserNotFoundException("User not found");

            }
            return userObject.User;
        }

        public IActionResult LandingPage()
        {
            return View();
        }

        public IActionResult DisplayReminders()
        {
            // Get the current time
            var currentTime = DateTime.Now.TimeOfDay;

            // Retrieve reminders that match the current time

            ViewBag.Time = currentTime;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}