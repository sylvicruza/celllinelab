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
    public class EnzymeController : Controller
    {
        private readonly Cell_line_laboratoryContext _context;

        public EnzymeController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10; // Number of items per page

            var enzymes = _context.Enzyme
                .Include(u => u.User)
                .Where(enzymes => !(enzymes.IsMarkedForDeletion && enzymes.DeletionTimestamp > DateTime.UtcNow))
                .OrderByDescending(cellLine => cellLine.Id) // Optional: Order the data as needed
                .ToPagedList(pageNumber, pageSize);



            return View(enzymes);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cellLineToDelete = await _context.Enzyme.FindAsync(id);
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
            var viewModel = new EnzymeModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(EnzymeModel viewModel)
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

                var enzymeEntity = new Entities.Enzyme
                {
                    PlasmidCode = viewModel.PlasmidCode,
                    AntibodyName = viewModel.AntibodyName,
                    CatalogueNo = viewModel.CatalogueNo,
                    Date = DateTime.Now,
                    UserId = userId, // Use the UserId from claims
                    Status = "Unused",
                    Maker = viewModel.Maker,
                    Location = viewModel.Location,
                    Data = viewModel.Data,
                    Note = viewModel.Note
                };

                try
                {
                    _context.Enzyme.Add(enzymeEntity);
                    int rowsAffected = _context.SaveChanges();

                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "enzyme added successfully!";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No changes were applied to the database.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while adding the enzyme: " + ex.Message;
                }
            }

            return View(viewModel);
        }


        public IActionResult DeletedItems(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10; // Number of items per page

            var deletedenzymes = _context.Enzyme.Include(u=>u.User)
                .Where(enzymes => enzymes.IsMarkedForDeletion && enzymes.DeletionTimestamp > DateTime.UtcNow)
                .ToPagedList(pageNumber, pageSize);

            return View(deletedenzymes);
        }


        [HttpPost]
        public IActionResult Restore(int id)
        {
            var enzyme = _context.Enzyme.Find(id);
            if (enzyme != null)
            {
                enzyme.IsMarkedForDeletion = false;
                enzyme.DeletionTimestamp = null;
                _context.SaveChanges();
            }
            return RedirectToAction("DeletedItems");
        }

        [HttpPost]
        public IActionResult PermanentlyDelete(int id)
        {
            var enzyme = _context.Enzyme.Find(id);
            if (enzyme != null)
            {
                _context.Enzyme.Remove(enzyme);
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
                    var userId = GetCurrentUser().Id;

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
                                var enzyme = new Enzyme
                                {
                                    // ... other properties ...
                                    PlasmidCode = worksheet.Cells[row, 1].Value.ToString(),
                                    AntibodyName = worksheet.Cells[row, 2].Value.ToString(),
                                    CatalogueNo = worksheet.Cells[row, 3].GetValue<string>(),
                                    Maker = worksheet.Cells[row, 4].GetValue<string>(),
                                    Note = worksheet.Cells[row, 5].GetValue<string>(),
                                    Location = worksheet.Cells[row, 6].GetValue<string>(),
                                    Data = worksheet.Cells[row, 7].GetValue<string>(),
                                    Date = DateTime.UtcNow,
                                    UserId = userId,
                                    Status = "Unused",
                                  
                                    //UserId = userId // Insert the obtained User ID
                                };

                                _context.Enzyme.Add(enzyme);
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
            var enzyme = _context.Enzyme.FirstOrDefault(cl => cl.Id == id);
            if (enzyme == null)
            {
                return NotFound();
            }

            return View(enzyme);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(int id, Enzyme enzyme)
        {
            if (id != enzyme.Id)
            {
                return NotFound();
            }

            try
            {
                // Retrieve the existing enzyme from the database
                var existingenzyme = _context.Enzyme.FirstOrDefault(cl => cl.Id == id);
                if (existingenzyme == null)
                {
                    return NotFound();
                }

                

                // Update the editable properties
                existingenzyme.PlasmidCode = enzyme.PlasmidCode;
                existingenzyme.AntibodyName = enzyme.AntibodyName;
                existingenzyme.CatalogueNo = enzyme.CatalogueNo;
                existingenzyme.Maker = enzyme.Maker;
                existingenzyme.Location = enzyme.Note;
                existingenzyme.Note = enzyme.Note;
                existingenzyme.Data = enzyme.Data;
                _context.Update(existingenzyme);


                // Update the record in the database
                _context.SaveChanges();

                TempData["SuccessMessage"] = "enzyme updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the enzyme: " + ex.Message;
            }

            // Always return the view
            return View(enzyme);
        }




        public IActionResult Details(int id)
        {
            var enzyme = _context.Enzyme
                .Include(u => u.User)
                .FirstOrDefault(c => c.Id == id);

            if (enzyme == null)
            {
                return NotFound();
            }



            ViewData["UserName"] = enzyme.User.Name;
            return View(enzyme);
        }


        public IActionResult SearchCellLines(string status, DateTime? startDate, DateTime? endDate, string genotype, int page = 1, int pageSize = 10)
        {
            var distinctenzymeNames = _context.Enzyme.Select(cl => cl.AntibodyName).Distinct().ToList();
            ViewBag.DistinctGenotypes = distinctenzymeNames;

            var query = _context.Enzyme.AsQueryable();

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
                query = query.Where(cl => cl.AntibodyName == genotype);
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
            var query = _context.Enzyme.Include(u=>u.User).AsQueryable();

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
                query = query.Where(cl => cl.AntibodyName.Contains(genotype));
            }

            var cellLines = query.ToList();

            // Create an Excel package and populate data
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Search Results");

                // Add headers
                worksheet.Cells["A1"].Value = "PlasmidCode";
                worksheet.Cells["B1"].Value = "AntibodyName";
                worksheet.Cells["C1"].Value = "CatalogueNo";
                worksheet.Cells["D1"].Value = "Date";
                worksheet.Cells["E1"].Value = "Maker";
                worksheet.Cells["F1"].Value = "Note";
                worksheet.Cells["G1"].Value = "UserId";
                worksheet.Cells["H1"].Value = "Status";
                worksheet.Cells["I1"].Value = "Location";
                worksheet.Cells["J1"].Value = "Data";

                // Set column headers style
                using (var cells = worksheet.Cells["A1:J1"])
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
                    worksheet.Cells[i + 2, 2].Value = cellLine.AntibodyName;
                    worksheet.Cells[i + 2, 3].Value = cellLine.CatalogueNo;
                    worksheet.Cells[i + 2, 4].Value = cellLine.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 5].Value = cellLine.Maker;
                    worksheet.Cells[i + 2, 6].Value = cellLine.Note;
                    worksheet.Cells[i + 2, 7].Value = cellLine.User.Name;
                    worksheet.Cells[i + 2, 8].Value = cellLine.Status;
                    worksheet.Cells[i + 2, 9].Value = cellLine.Location;
                    worksheet.Cells[i + 2, 10].Value = cellLine.Data;
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
                var enzyme = _context.Enzyme.Include(u => u.User).FirstOrDefault(cl => cl.PlasmidCode == viewModel.Code);

                if (enzyme == null)
                {
                    TempData["ErrorMessage"] = "No such Code or Code not found.";
                }
                else
                {
                    viewModel.EnzymeDetails = enzyme; // Set the found chemical to the view model

                    // Fetch associated DailyUsage records
                    viewModel.DailyUsages = _context.DailyUsage
                        .Where(du => du.CellLineCode == viewModel.Code)
                        .ToList();
                }
            }
            else
            {
                viewModel.EnzymeDetails = null; // Reset chemicalDetails if Code is empty
            }

            return View("Search", viewModel);
        }

        [HttpPost]
        public IActionResult SaveDailyUsage(DailyUsageViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Code))
            {
                var cellLine = _context.Chemical.FirstOrDefault(cl => cl.PlasmidCode == viewModel.Code);

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
                        TempData["ErrorMessage"] = "Usage entered is more than the remaining Enzyme balance.";
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
