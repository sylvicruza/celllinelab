using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cell_line_laboratory.Data;
using Cell_line_laboratory.Entities;
using Cell_line_laboratory.Services;
using Cell_line_laboratory.Models;
using System.Web.Helpers;
using Cell_line_laboratory.Utils;

namespace Cell_line_laboratory.Controllers
{
    public class AssignTaskController : Controller
    {
        private readonly Cell_line_laboratoryContext _context;

        public AssignTaskController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }

        // GET: AssignTask
        public async Task<IActionResult> Index()
        {
            var cell_line_laboratoryContext = _context.AssignTask.Include(a => a.User);
            return View(await cell_line_laboratoryContext.ToListAsync());
        }

        // GET: AssignTask/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AssignTask == null)
            {
                return NotFound();
            }

            var assignTask = await _context.AssignTask
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignTask == null)
            {
                return NotFound();
            }

            return View(assignTask);
        }

        // GET: AssignTask/Create
        public IActionResult Create()
        {
            ViewData["AllUsers"] =  _context.User.ToListAsync().Result;
            
          

            return View();
        }

        // POST: AssignTask/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignTaskViewModel model)
        {
           

            try
            {
                var currentUserEmail = await GetCurrentUserEmail();

                foreach (var userId in model.UserId)
                {
                    var assign = new AssignTask
                    {
                        AssignedBy = currentUserEmail,
                        CreatedDate = DateTime.Now,
                        UserId = userId,
                        Designation = model.Designation
                    };

                    await SendAssignedTaskByEmail(userId, assign);
                    _context.Add(assign);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["AllUsers"] = await _context.User.ToListAsync();
                // Handle exception here
                ViewBag.Failure = ex.Message;
                return View(model);
            }
        }

        private async Task<string> GetCurrentUserEmail()
        {
            var currentUser = await GetCurrentUser();
            return currentUser.Email;
        }

        

        private async Task SendAssignedTaskByEmail(int userId, AssignTask assignTask)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);

            var emailBody = $"You have been assigned a task on Cell Division portal\n" +
                            $"Task Assigned: {assignTask.Designation}\n" +
                            $"Date Assigned: {assignTask.CreatedDate}\n" +
                            $"Assigned By: {assignTask.AssignedBy}\n";

            await EmailSender.SendEmailAsync(user.Email, "New Task Assigned", emailBody);
        }


        // GET: AssignTask/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AssignTask == null)
            {
                return NotFound();
            }

            var assignTask = await _context.AssignTask.FindAsync(id);
            if (assignTask == null)
            {
                return NotFound();
            }
            ViewData["AllUsers"] = await _context.User.ToListAsync();
            return View(assignTask);
        }

        // POST: AssignTask/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Designation,CreatedDate,AssignedBy")] AssignTask assignTask)
        {
            if (id != assignTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignTaskExists(assignTask.Id))
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
            ViewData["AllUsers"] = await _context.User.ToListAsync();
            return View(assignTask);
        }

        // GET: AssignTask/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AssignTask == null)
            {
                return NotFound();
            }

            var assignTask = await _context.AssignTask
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignTask == null)
            {
                return NotFound();
            }

            return View(assignTask);
        }

        // POST: AssignTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AssignTask == null)
            {
                return Problem("Entity set 'Cell_line_laboratoryContext.AssignTask'  is null.");
            }
            var assignTask = await _context.AssignTask.FindAsync(id);
            if (assignTask != null)
            {
                _context.AssignTask.Remove(assignTask);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignTaskExists(int id)
        {
          return (_context.AssignTask?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<User> GetCurrentUser()
        {
            var email = User.Identity.Name;
            var user = await _context.User

                .FirstOrDefaultAsync(m => m.Email == email);
            return user;
        }
    }
}
