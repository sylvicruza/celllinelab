using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cell_line_laboratory.Data;
using Cell_line_laboratory.Entities;
using Cell_line_laboratory.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Cell_line_laboratory.Models;
using Cell_line_laboratory.Services;
using Cell_line_laboratory.Utils;

namespace Cell_line_laboratory.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        // GET: User
        public async Task<IActionResult> Index(string response)
        {
            if (!string.IsNullOrEmpty(response) && response.Contains("error"))
            {
                ViewBag.Failure = response;
            }

            else
            {
                ViewBag.Message = response;
            }

            ViewData["AllUsers"] = await userService.GetAllUsers();
            var user = new User
            {
                CreatedAt = DateTime.Now,
                Status = "InActive",
                UserType = "Unknown",
                Password = "Unassigned",

            };
            return View(user);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || userService.GetUser == null)
            {
                return NotFound();
            }

            var user = await userService.GetUserId(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Profile
        public IActionResult Profile()
        {
            var user = GetCurrentUser().Result;
            Console.WriteLine(user);
            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Role,UserType,Status,CreatedAt,LastUpdatedAt,DeletedAt,DeletedBy")] User user)
        {

           /* if (ModelState.IsValid)
            {*/
                try
                {
                    await userService.CreateUser(user);
                    var message = "User created successfully ";
                    return RedirectToAction("Index", "User", new { response = message });
                }
                catch (Exception ex)
                {
                    var error1 = "Error occur creating user \n" + ex.Message;
                    return RedirectToAction("Index", "User", new { response = error1 });

                }
            /*}
            var error2 = "Failed to create user error " + ModelState.ErrorCount;
            return RedirectToAction("Index", "User", new { response = error2 });*/
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {


            var user = await userService.GetUserByUserId(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> BlockUser(int id)
        {
            var user = await userService.GetUserByUserId(id);
            if (user == null)
            {
                return NotFound();
            }
            string message = "";
            if (user.Status != "Blocked")
            {
                user.Status = "Blocked";
                message = "User blocked successfully ";
            }
            else
            {
                user.Status = "InActive";
                message = "User deactivated successfully ";
            }

            await userService.UpdateUser(user);

            return RedirectToAction("Index", "User", new { response = message });


        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,Role,UserType,Status,CreatedAt,LastUpdatedAt,DeletedAt,DeletedBy,CellLines")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    await userService.UpdateUser(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!userService.UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var message = "User updated successfully ";
                return RedirectToAction("Index", "User", new { response = message });

            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || userService.GetUser == null)
            {
                return NotFound();
            }

            var user = await userService.GetUserId(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (userService.GetUser == null)
            {
                return Problem("Entity set 'Cell_line_laboratoryContext.User'  is null.");
            }

            await userService.DeleteUser(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<User> GetCurrentUser()
        {
            var email = User?.Identity?.Name;
            var user = await userService.GetUserByEmail(email);
            return user;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current user from your database
                    var user = await GetCurrentUser();

                    if (user != null)
                    {
                        // Verify the current password
                        var passwordHasher = new PasswordHasher<User>();
                        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, model.CurrentPassword);

                        if (passwordVerificationResult == PasswordVerificationResult.Success)
                        {
                            // Hash the new password
                            var newPasswordHash = passwordHasher.HashPassword(user, model.NewPassword);

                            // Update the user's password hash
                            user.Password = newPasswordHash;

                            // Save changes to the database
                            await userService.UpdateUser(user);

                            // Password changed successfully
                            TempData["SuccessMessage"] = "Password changed successfully.";
                            return RedirectToAction("Profile"); // Redirect back to the change password page
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Incorrect current password.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User not found.");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }

            // If we reach this point, something went wrong, show the form again with error messages
            return View("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFeedback(string email, string comment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(comment))
                {
                    ModelState.AddModelError(string.Empty, "Email or comment not provided.");
                    return View(); // Return the view to show the error message
                }

                // Assuming EmailSender.SendEmailAsync returns a Task
                await EmailSender.SendEmailAsync("morenikejiolatunbosun66@gmail.com", "App User Feedback", comment);

                // Feedback received
                TempData["SuccessMessage"] = "Feedback sent, thank you!";
                return RedirectToAction("SendFeedback");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return View(); // Return the view to show the error message
            }
        }

        public IActionResult SendFeedback()
        {
            var user = GetCurrentUser().Result;
            Console.WriteLine(user);
            return View(user);
        }

    }
}