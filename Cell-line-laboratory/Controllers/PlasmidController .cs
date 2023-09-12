using Microsoft.AspNetCore.Mvc;
using Cell_line_laboratory.Models;
using Cell_line_laboratory.Data;
using Microsoft.AspNetCore.Authorization; // Import the authorization namespace
using System;
using System.Linq;
using System.Security.Claims; // Import the claims namespace
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using Cell_line_laboratory.Entities;
using OfficeOpenXml;

namespace Cell_line_laboratory.Controllers
{
    //[Authorize] // Apply the authorization attribute to the controller
    public class PlasmidController : Controller
    {
        private readonly Cell_line_laboratoryContext _context;

        public PlasmidController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10; // Number of items per page

            var plasmids = _context.PlasmidCollection
                .Include(u=>u.User)
                .Where(plasmids => !(plasmids.IsMarkedForDeletion && plasmids.DeletionTimestamp > DateTime.UtcNow))
                .OrderByDescending(cellLine => cellLine.Id) // Optional: Order the data as needed
                .ToPagedList(pageNumber, pageSize);

           

            return View(plasmids);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cellLineToDelete = await _context.PlasmidCollection.FindAsync(id);
            if (cellLineToDelete != null)
            {
                cellLineToDelete.IsMarkedForDeletion = true;
                cellLineToDelete.DeletionTimestamp = DateTime.UtcNow.AddMonths(3);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        [Authorize]
        public IActionResult Create()
        {
            var viewModel = new PlasmidModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(PlasmidModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Get the UserId from the logged-in user's claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    TempData["ErrorMessage"] = "User ID not found.";
                    return View(viewModel);
                }

                var plasmidEntity = new Entities.Plasmid
                {
                    PlasmidCode = viewModel.PlasmidCode,
                    PlasmidName = viewModel.PlasmidName,
                    Origin = viewModel.Origin,
                    Date = DateTime.Now,
                    UserId = userId, // Use the UserId from claims
                    Status = "Unused",
                    PlasmidMapLink = viewModel.PlasmidMapLink,
                    Note = viewModel.Note
                };

                try
                {
                    _context.PlasmidCollection.Add(plasmidEntity);
                    int rowsAffected = _context.SaveChanges();

                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Plasmid added successfully!";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No changes were applied to the database.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while adding the plasmid: " + ex.Message;
                }
            }

            return View(viewModel);
        }


        public IActionResult DeletedItems(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10; // Number of items per page

            var deletedPlasmids = _context.PlasmidCollection.Include(a => a.User)
                .Where(plasmids => plasmids.IsMarkedForDeletion && plasmids.DeletionTimestamp > DateTime.UtcNow)
                .ToPagedList(pageNumber, pageSize);

            return View(deletedPlasmids);
        }


        [HttpPost]
        public IActionResult Restore(int id)
        {
            var plasmid = _context.PlasmidCollection.Find(id);
            if (plasmid != null)
            {
                plasmid.IsMarkedForDeletion = false;
                plasmid.DeletionTimestamp = null;
                _context.SaveChanges();
            }
            return RedirectToAction("DeletedItems");
        }

        [HttpPost]
        public IActionResult PermanentlyDelete(int id)
        {
            var plasmid = _context.PlasmidCollection.Find(id);
            if (plasmid != null)
            {
                _context.PlasmidCollection.Remove(plasmid);
                _context.SaveChanges();
            }
            return RedirectToAction("DeletedItems");
        }


        [HttpGet]
        public IActionResult UploadExcel()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult UploadExcel(UploadExcelViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = GetCurrentUser().Id;

                    using (var package = new ExcelPackage(model.File.OpenReadStream()))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet != null)
                        {
                            int rowsInserted = 0;
                            int rowsSkipped = 0;

                            // Iterate through the rows and process data
                            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                            {
                                var plasmid = new Plasmid
                                {
                                    // ... other properties ...
                                    PlasmidCode = worksheet.Cells[row, 1].Value.ToString(),
                                    PlasmidName = worksheet.Cells[row, 2].Value.ToString(),
                                    Origin = worksheet.Cells[row, 3].GetValue<string>(),
                                    PlasmidMapLink = worksheet.Cells[row, 4].GetValue<string>(),
                                    Note = worksheet.Cells[row, 5].GetValue<string>(),
                                    Date = DateTime.UtcNow,
                                    UserId = userName,
                                    Status = "unused"
                                    //UserId = userId // Insert the obtained User ID
                                };

                                _context.PlasmidCollection.Add(plasmid);
                                rowsInserted++;
                            }

                            _context.SaveChanges();

                            if (rowsSkipped > 0)
                            {
                                TempData["ErrorMessage"] = $"{rowsSkipped} rows skipped due to existing positions.";
                            }
                            else
                            {
                                TempData["SuccessMessage"] = $"{rowsInserted} rows inserted successfully.";
                            }

                            return View(model); // Return the view to show the message
                        }
                    }

                    TempData["ErrorMessage"] = "Excel file does not contain valid data.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while uploading the Excel file: " + ex.Message;
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var plasmid = _context.PlasmidCollection.FirstOrDefault(cl => cl.Id == id);
            if (plasmid == null)
            {
                return NotFound();
            }

            return View(plasmid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(int id, Plasmid plasmid)
        {
            if (id != plasmid.Id)
            {
                return NotFound();
            }

            try
            {
                // Retrieve the existing Plasmid from the database
                var existingPlasmid = _context.PlasmidCollection.FirstOrDefault(cl => cl.Id == id);
                if (existingPlasmid == null)
                {
                    return NotFound();
                }

                // Check if the PlasmidCode already exists in the database
                if (!string.IsNullOrEmpty(plasmid.PlasmidCode) &&
                    _context.PlasmidCollection.Any(cl => cl.Id != id && cl.PlasmidCode == plasmid.PlasmidCode))
                {
                    ModelState.AddModelError("PlasmidCode", "PlasmidCode already exists in the database.");
                    return View(plasmid);
                }

                // Update the editable properties
                existingPlasmid.PlasmidCode = plasmid.PlasmidCode;
                existingPlasmid.PlasmidName = plasmid.PlasmidName;
                existingPlasmid.Origin = plasmid.Origin;
                existingPlasmid.Note = plasmid.Note;
                existingPlasmid.PlasmidMapLink = plasmid.PlasmidMapLink;

                

                // Update the record in the database
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Plasmid updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the Plasmid: " + ex.Message;
            }       

            // Always return the view
            return View(plasmid);
        }




        public IActionResult Details(int id)
        {
            var plasmid = _context.PlasmidCollection
                .Include(u=>u.User)
                .FirstOrDefault(c => c.Id == id);

            if (plasmid == null)
            {
                return NotFound();
            }

          

            ViewData["UserName"] = plasmid.User.Name;
            return View(plasmid);
        }


        public IActionResult SearchCellLines(string status, DateTime? startDate, DateTime? endDate, string genotype, int page = 1, int pageSize = 10)
        {
            var distinctPlasmidNames = _context.PlasmidCollection.Select(cl => cl.PlasmidName).Distinct().ToList();
            ViewBag.DistinctGenotypes = distinctPlasmidNames;

            var query = _context.PlasmidCollection.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(cl => cl.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(cl => cl.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(cl => cl.Date <= endDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            if (!string.IsNullOrEmpty(genotype))
            {
                query = query.Where(cl => cl.PlasmidName == genotype);
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            var cellLines = query.ToList();

            ViewBag.Status = status;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Genotype = genotype;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(cellLines);
        }




        public IActionResult DownloadExcel(string status, DateTime? startDate, DateTime? endDate, string genotype)
        {
            var query = _context.PlasmidCollection.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(cl => cl.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(cl => cl.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(cl => cl.Date <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(genotype))
            {
                query = query.Where(cl => cl.PlasmidName.Contains(genotype));
            }

            var cellLines = query.ToList();

            // Create an Excel package and populate data
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Search Results");

                // Add headers
                worksheet.Cells["A1"].Value = "PlasmidCode";
                worksheet.Cells["B1"].Value = "PlasmidName";
                worksheet.Cells["C1"].Value = "Origin";
                worksheet.Cells["D1"].Value = "Date";
                worksheet.Cells["E1"].Value = "PlasmidMapLink";
                worksheet.Cells["F1"].Value = "Notes";
                worksheet.Cells["G1"].Value = "UserId";
                worksheet.Cells["H1"].Value = "Status";

                // Set column headers style
                using (var cells = worksheet.Cells["A1:H1"])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Populate data rows
                for (int i = 0; i < cellLines.Count; i++)
                {
                    var cellLine = cellLines[i];

                    worksheet.Cells[i + 2, 1].Value = cellLine.PlasmidCode;
                    worksheet.Cells[i + 2, 2].Value = cellLine.PlasmidName;
                    worksheet.Cells[i + 2, 3].Value = cellLine.Origin;
                    worksheet.Cells[i + 2, 4].Value = cellLine.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 5].Value = cellLine.PlasmidMapLink;
                    worksheet.Cells[i + 2, 6].Value = cellLine.Note;
                    worksheet.Cells[i + 2, 7].Value = cellLine.UserId;
                    worksheet.Cells[i + 2, 8].Value = cellLine.Status;
                }

                // Auto fit columns for better readability
                worksheet.Cells.AutoFitColumns();

                // Return the Excel file
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var currentDate = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"SearchResults_{currentDate}.xlsx";
                var stream = new MemoryStream(package.GetAsByteArray());

                return File(stream, contentType, fileName);
            }
        }

        public async Task<User> GetCurrentUser()
        {
            var email = User.Identity.Name;
            var user = await _context.User

                .FirstOrDefaultAsync(m => m.Email == email);
            return user;
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
                var plasmid = _context.PlasmidCollection.Include(u=>u.User).FirstOrDefault(cl => cl.PlasmidCode == viewModel.Code);

                if (plasmid == null)
                {
                    TempData["ErrorMessage"] = "No such Code or Code not found.";
                }
                else
                {
                    viewModel.PlasmidDetails = plasmid; // Set the found Plasmid to the view model

                    // Fetch associated DailyUsage records
                    viewModel.DailyUsages = _context.DailyUsage
                        .Where(du => du.CellLineCode == viewModel.Code)
                        .ToList();
                }
            }
            else
            {
                viewModel.PlasmidDetails = null; // Reset plasmidDetails if Code is empty
            }

            return View("Search", viewModel);
        }

        [HttpPost]
        public IActionResult SaveDailyUsage(DailyUsageViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Code))
            {
                var cellLine = _context.PlasmidCollection.FirstOrDefault(cl => cl.PlasmidCode == viewModel.Code);

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
                        string currentUsername = GetCurrentUser().Result.Name;

                        // Update the plasmid status based on remaining balance
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
                        TempData["ErrorMessage"] = "Usage entered is more than the remaining Plasmid balance.";
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
