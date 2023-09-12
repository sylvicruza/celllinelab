using Microsoft.AspNetCore.Mvc;
using Cell_line_laboratory.Models;
using Cell_line_laboratory.Data;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Diagnostics;
using Cell_line_laboratory.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;
using OfficeOpenXml;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;


namespace Cell_line_laboratory.Controllers
{
    public class CellLineController : Controller
    {
        private readonly Cell_line_laboratoryContext _context;

        public CellLineController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }


        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10; // Number of items per page

            var cellLines = _context.CellLine
                .Where(cellLine => !(cellLine.IsMarkedForDeletion && cellLine.DeletionTimestamp > DateTime.UtcNow))
                .OrderByDescending(cellLine => cellLine.Id) // Optional: Order the data as needed
                .ToPagedList(pageNumber, pageSize);

            foreach (var cellLine in cellLines)
            {
                var user = _context.User.FirstOrDefault(u => u.Name == cellLine.UserName); // Assuming User entity has a property 'Name'
                if (user != null)
                {
                    cellLine.UserName = user.Name;
                }
            }

            return View(cellLines);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cellLineToDelete = await _context.CellLine.FindAsync(id);
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
            // Retrieve all positions that are already inserted in the database as strings
            var allPositionsInDatabaseStrings = _context.CellLine
                .Select(c => c.Position)
                .ToList();

            // Parse the retrieved position strings into integers
            var allSelectedPositionsInDatabase = new List<int>();
            foreach (var positions in allPositionsInDatabaseStrings)
            {
                var positionList = positions.Split(','); // Split the positions string by commas

                foreach (var positionString in positionList)
                {
                    if (int.TryParse(positionString, out int positionValue)) // Try parsing each value
                    {
                        allSelectedPositionsInDatabase.Add(positionValue); // Add valid integer to the list
                    }
                    // You can optionally handle non-integer values here if needed
                }
            }

            // Pass the list of selected positions in the ViewBag
            ViewBag.SelectedPositionsInDatabase = allSelectedPositionsInDatabase;

            var viewModel = new CellLineModel();
            viewModel.SelectedPositions = new List<int>(); // Initialize the list
            return View(viewModel);
        }

        //Create Celline

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public IActionResult Create(CellLineModel viewModel)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userName = User.FindFirstValue(ClaimTypes.Name);

                List<int> selectedPositions = Request.Form["selectedPositions"]
                    .Where(checkbox => !string.IsNullOrEmpty(checkbox))
                    .Select(int.Parse)
                    .ToList();

                // Check if the Code already exists in the database
                if (_context.CellLine.Any(cl => cl.CellLineCode == viewModel.CellLineCode))
                {
                    
                    ModelState.AddModelError(nameof(viewModel.CellLineCode), "Code already exists!");
                }
                else if (selectedPositions.Count == 0)
                {
                    TempData["ErrorMessage"] = "Please select at least one position!";
                }
                else
                {
                    // Create the cellLineEntity and save it to the database
                    var cellLineEntity = new Entities.CellLine
                    {
                        CellLineCode = viewModel.CellLineCode,
                        Genotype = viewModel.Genotype,
                        ParentalLine = viewModel.ParentalLine,
                        Notes = viewModel.Notes,
                        Date = DateTime.Now,
                        UserName = userName,
                        Status = "Unused",
                        Position = string.Join(",", selectedPositions) // Convert to comma-separated string
                    };

                    _context.CellLine.Add(cellLineEntity);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Cell line created successfully!";
                    ModelState.Clear(); // Clear the model state to remove any existing validation errors
                    viewModel = new CellLineModel(); // Create a new instance to clear form values
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the cell line: " + ex.Message;

                // Handle the specific error case
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint 'UQ_CellLineCode'"))
                {
                    TempData["CellLineCodeError"] = "Code already exists!";
                }
            }

            // Retrieve all positions that are already inserted in the database as strings
            var allPositionsInDatabaseStrings = _context.CellLine
                .Select(c => c.Position)
                .ToList();

            // Parse the retrieved position strings into integers
            var allSelectedPositionsInDatabase = allPositionsInDatabaseStrings
                .SelectMany(positions => positions.Split(','))
                .Where(position => int.TryParse(position, out _)) // Only include valid integers
                .Select(int.Parse)
                .ToList();

            // Pass the list of selected positions in the ViewBag
            ViewBag.SelectedPositionsInDatabase = allSelectedPositionsInDatabase;

            return View("Create", viewModel); // Return to the Create view
        }


        //public IActionResult DeletedItems()
        //{
        //    var deletedItems = _context.CellLine.Where(c => c.IsMarkedForDeletion && c.DeletionTimestamp > DateTime.UtcNow).ToList();
        //    return View(deletedItems);
        //}

        public IActionResult DeletedItems(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10; // Number of items per page

            var deletedCellLines = _context.CellLine
                .Where(cellLine => cellLine.IsMarkedForDeletion && cellLine.DeletionTimestamp > DateTime.UtcNow)
                .ToPagedList(pageNumber, pageSize);

            return View(deletedCellLines);
        }


        [HttpPost]
        public IActionResult Restore(int id)
        {
            var cellLine = _context.CellLine.Find(id);
            if (cellLine != null)
            {
                cellLine.IsMarkedForDeletion = false;
                cellLine.DeletionTimestamp = null;
                _context.SaveChanges();
            }
            return RedirectToAction("DeletedItems");
        }

        [HttpPost]
        public IActionResult PermanentlyDelete(int id)
        {
            var cellLine = _context.CellLine.Find(id);
            if (cellLine != null)
            {
                _context.CellLine.Remove(cellLine);
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
                    var userName = User.FindFirstValue(ClaimTypes.Name);

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
                                var position = worksheet.Cells[row, 4].GetValue<string>();
                                if (!_context.CellLine.Any(cl => cl.Position == position))
                                {

                                    var cellLine = new CellLine
                                    {
                                        // ... other properties ...
                                        CellLineCode = worksheet.Cells[row, 1].Value.ToString(),
                                        Genotype = worksheet.Cells[row, 2].Value.ToString(),
                                        ParentalLine = worksheet.Cells[row, 3].GetValue<string>(),
                                        Position = position,
                                        Notes = worksheet.Cells[row, 5].GetValue<string>(),
                                        Date = DateTime.UtcNow,
                                        UserName = userName,
                                        Status = "unused"
                                        //UserId = userId // Insert the obtained User ID
                                    };

                                    _context.CellLine.Add(cellLine);
                                    rowsInserted++;
                                }
                                else
                                {
                                    rowsSkipped++;
                                }
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
            var cellLine = _context.CellLine.FirstOrDefault(cl => cl.Id == id);
            if (cellLine == null)
            {
                return NotFound();
            }

            return View(cellLine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(int id, CellLine cellLine)
        {
            if (id != cellLine.Id)
            {
                return NotFound();
            }

            try
            {
                // Retrieve the existing cell line from the database
                var existingCellLine = _context.CellLine.FirstOrDefault(cl => cl.Id == id);
                if (existingCellLine == null)
                {
                    return NotFound();
                }

                // Check if the Code already exists in the database
                if (!string.IsNullOrEmpty(cellLine.CellLineCode) &&
                    _context.CellLine.Any(cl => cl.Id != id && cl.CellLineCode == cellLine.CellLineCode))
                {
                    ModelState.AddModelError("Code", "Code already exists in the database.");
                    return View(cellLine);
                }

                // Update the editable properties
                existingCellLine.CellLineCode = cellLine.CellLineCode;
                existingCellLine.Genotype = cellLine.Genotype;
                existingCellLine.ParentalLine = cellLine.ParentalLine;
                existingCellLine.Notes = cellLine.Notes;
                existingCellLine.Position = cellLine.Position;

                // Validate if the updated position is unique
                if (!string.IsNullOrEmpty(cellLine.Position) &&
                    _context.CellLine.Any(cl => cl.Id != id && cl.Position == cellLine.Position))
                {
                    ModelState.AddModelError("Position", "Position already exists in the database.");
                    return View(cellLine);
                }

                // Update the record in the database
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cell line updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the cell line: " + ex.Message;
            }

            // Retrieve all positions that are already inserted in the database as strings
            var allPositionsInDatabaseStrings = _context.CellLine
                .Select(c => c.Position)
                .ToList();

            // Parse the retrieved position strings into integers
            var allSelectedPositionsInDatabase = allPositionsInDatabaseStrings
                .SelectMany(positions => positions.Split(','))
                .Where(position => int.TryParse(position, out _)) // Only include valid integers
                .Select(int.Parse)
                .ToList();

            // Pass the list of selected positions in the ViewBag
            ViewBag.SelectedPositionsInDatabase = allSelectedPositionsInDatabase;

            // Always return the view
            return View(cellLine);
        }




        public IActionResult Details(int id)
        {
            var cellLine = _context.CellLine.FirstOrDefault(c => c.Id == id);

            if (cellLine == null)
            {
                return NotFound();
            }

            var userNmae = cellLine.UserName;
            var userName = _context.User.FirstOrDefault(u => u.Name == cellLine.UserName);

            ViewData["UserName"] = userName;
            return View(cellLine);
        }


        public IActionResult SearchCellLines(string status, DateTime? startDate, DateTime? endDate, string genotype, int page = 1, int pageSize = 10)
        {
            var distinctGenotypes = _context.CellLine.Select(cl => cl.Genotype).Distinct().ToList();
            ViewBag.DistinctGenotypes = distinctGenotypes;

            var query = _context.CellLine.AsQueryable();

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
                query = query.Where(cl => cl.Genotype == genotype);
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
            var query = _context.CellLine.AsQueryable();

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
                query = query.Where(cl => cl.Genotype.Contains(genotype));
            }

            var cellLines = query.ToList();

            // Create an Excel package and populate data
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Search Results");

                // Add headers
                worksheet.Cells["A1"].Value = "Code";
                worksheet.Cells["B1"].Value = "Genotype";
                worksheet.Cells["C1"].Value = "ParentalLine";
                worksheet.Cells["D1"].Value = "Date";
                worksheet.Cells["E1"].Value = "Position";
                worksheet.Cells["F1"].Value = "Notes";
                worksheet.Cells["G1"].Value = "UserName";
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

                    worksheet.Cells[i + 2, 1].Value = cellLine.CellLineCode;
                    worksheet.Cells[i + 2, 2].Value = cellLine.Genotype;
                    worksheet.Cells[i + 2, 3].Value = cellLine.ParentalLine;
                    worksheet.Cells[i + 2, 4].Value = cellLine.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 5].Value = cellLine.Position;
                    worksheet.Cells[i + 2, 6].Value = cellLine.Notes;
                    worksheet.Cells[i + 2, 7].Value = cellLine.UserName;
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
