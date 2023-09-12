using Cell_line_laboratory.Data;
using Cell_line_laboratory.Entities;
using Microsoft.AspNetCore.Mvc;
using Cell_line_laboratory.Models;
using Microsoft.EntityFrameworkCore;

namespace Cell_line_laboratory.Controllers
{
    public class DailyUsageController : Controller
    {
        private readonly Cell_line_laboratoryContext _context;

        public DailyUsageController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }

        // Action to display the search form
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PerformSearch(DailyUsageViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Code))
            {
                var cellLine = _context.CellLine.FirstOrDefault(cl => cl.CellLineCode == viewModel.Code);

                if (cellLine == null)
                {
                    TempData["ErrorMessage"] = "No such Code or Code not found.";
                }
                else
                {
                    viewModel.CellLineDetails = cellLine; // Set the found CellLine to the view model

                    // Fetch associated DailyUsage records
                    viewModel.DailyUsages = _context.DailyUsage
                        .Where(du => du.CellLineCode == viewModel.Code)
                        .ToList();
                }
            }
            else
            {
                viewModel.CellLineDetails = null; // Reset CellLineDetails if Code is empty
            }

            return View("Search", viewModel);
        }

        [HttpPost]
        public IActionResult SaveDailyUsage(DailyUsageViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Code))
            {
                var cellLine = _context.CellLine.FirstOrDefault(cl => cl.CellLineCode == viewModel.Code);

                if (cellLine != null)
                {
                    decimal enteredUsage = viewModel.Usage;

                    // Get the last DailyUsage record with the same Code (if exists)
                    var lastUsage = _context.DailyUsage
                        .Where(du => du.CellLineCode == viewModel.Code)
                        .OrderByDescending(du => du.Date)
                        .FirstOrDefault();

                    decimal remainingBalance = 100 - enteredUsage; // Default to 100 - entered usage if no previous record

                    if (lastUsage != null)
                    {
                        // Subtract the last balance from the entered usage
                        remainingBalance = lastUsage.Balance - enteredUsage;
                    }

                    if (remainingBalance >= 0)
                    {
                        // Get the current logged-in user's username
                        string currentUsername = User.Identity.Name;

                        // Update the CellLine status based on remaining balance
                        if (remainingBalance <= 5)
                        {
                            cellLine.Status = "Used";
                        }
                        else if (remainingBalance <= 100)
                        {
                            cellLine.Status = "Semi-Used";
                        }

                        var newUsage = new DailyUsage
                        {
                            CellLineCode = viewModel.Code,
                            Usage = enteredUsage,
                            Balance = remainingBalance,
                            Date = DateTime.Now,
                            Username = currentUsername,
                            Comment = viewModel.Comment
                        };

                        _context.DailyUsage.Add(newUsage);
                        _context.SaveChanges();

                        TempData["SuccessMessage"] = "Daily usage saved successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Usage entered is more than the remaining CellLine balance.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid Code.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid data entered.";
            }

            return RedirectToAction("Search", new { CellLineCode = viewModel.Code });
        }

        public IActionResult EditUsage(int id)
        {
            var usage = _context.DailyUsage.FirstOrDefault(du => du.Id == id);

            if (usage == null)
            {
                return NotFound();
            }

            var viewModel = new DailyUsageViewModel
            {
                UsageId = usage.Id,
                Code = usage.CellLineCode,
                Usage = usage.Usage,
                Comment = usage.Comment
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUsage(DailyUsageViewModel viewModel)
        {
            var usage = _context.DailyUsage.FirstOrDefault(du => du.Id == viewModel.UsageId);

            if (usage == null)
            {
                return NotFound();
            }

            usage.Usage = viewModel.Usage;
            usage.Comment = viewModel.Comment;
            usage.Date = DateTime.Now;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Usage updated successfully.";

            return RedirectToAction("Search", new { CellLineCode = usage.CellLineCode });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUsage(int id)
        {
            var usage = _context.DailyUsage.FirstOrDefault(du => du.Id == id);

            if (usage == null)
            {
                return NotFound();
            }

            _context.DailyUsage.Remove(usage);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Usage deleted successfully.";

            return RedirectToAction("Search", new { CellLineCode = usage.CellLineCode });
        }




    }

}
