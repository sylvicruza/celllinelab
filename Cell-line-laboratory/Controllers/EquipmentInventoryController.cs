using Cell_line_laboratory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cell_line_laboratory.Models;
using System.Security.Claims;
using Cell_line_laboratory.Entities;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Cell_line_laboratory.Controllers
{

    public class EquipmentInventoryController : Controller
    {
        private readonly Cell_line_laboratoryContext _context;

        public EquipmentInventoryController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }

        // GET: EquipmentInventory
        public async Task<IActionResult> Index()
        {
            return View(await _context.EquipmentInventory.ToListAsync());
        }

        //public IActionResult ViewAll()
        //{
        //    var equipmentList = _context.EquipmentInventory.ToList();

        //    var userNames = new Dictionary<int, string>();

        //    foreach (var equipment in equipmentList)
        //    {
        //        if (equipment.CreatedBy != null && !userNames.ContainsKey(equipment.CreatedBy))
        //        {
        //            var user = _context.User.FirstOrDefault(u => u.Id == equipment.CreatedBy);
        //            if (user != null)
        //            {
        //                userNames[equipment.CreatedBy] = user.Name;
        //            }
        //        }
        //    }

        //    ViewBag.UserNames = userNames;

        //    return View(equipmentList);
        //}


        // GET: EquipmentInventory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentInventory = await _context.EquipmentInventory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipmentInventory == null)
            {
                return NotFound();
            }

            return View(equipmentInventory);
        }

        // GET: EquipmentInventory/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(EquipmentInventoryModel viewModel, int SelectedProductId)
        {
            if (_context.EquipmentInventory.Any(e => e.ProductName == viewModel.ProductName))
            {
                TempData["ErrorMessage"] = "Product Name already exists.";
                return View(viewModel);
            }

            if (_context.EquipmentInventory.Any(e => e.ProductCode == viewModel.ProductCode))
            {
                TempData["ErrorMessage"] = "Product Code already exists.";
                return View(viewModel);
            }
            // Get the UserId from the logged-in user's claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = User.FindFirst(ClaimTypes.Name);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId) || userNameClaim == null)
            {
                TempData["ErrorMessage"] = "User information not found.";
                return View(viewModel);
            }

            var userName = userNameClaim.Value;

            // Now you have both the user's ID (userId) and name (userName)

            var eInventoryEntity = new Entities.EquipmentInventory
            {
                ProductId = SelectedProductId,
                ProductName = viewModel.ProductName,
                ProductCode = viewModel.ProductCode,
                ProductDescription = viewModel.ProductDescription,
                Vendor = viewModel.Vendor,
                SerialNumber = viewModel.SerialNumber,
                Quantity = viewModel.Quantity,
                Amount = viewModel.Amount,
                LastMaintenanceDate = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                NextMaintenanceDate = DateTime.Now.AddDays(7),
                CreatedBy = userName
            };

            try
            {
                _context.EquipmentInventory.Add(eInventoryEntity);
                int rowsAffected = _context.SaveChanges();

                if (rowsAffected > 0)
                {
                    TempData["SuccessMessage"] = "EquipmentInventory added successfully!";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["ErrorMessage"] = "No changes were applied to the database.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the EquipmentInventory: " + ex.Message;
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += " Inner Exception: " + ex.InnerException.Message;
                }
            }

            return View(viewModel);
        }

  
        // GET: EquipmentInventory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentInventory = await _context.EquipmentInventory.FindAsync(id);
            if (equipmentInventory == null)
            {
                return NotFound();
            }
            return View(equipmentInventory);
        }

        // POST: EquipmentInventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Product,ProductName,ProductCode,ProductDescription,Vendor,SerialNumber,Quantity,Amount,LastMaintenanceDate,NextMaintenanceDate,CreatedAt,UpdatedAt,CreatedBy")] EquipmentInventory equipmentInventory)
        {
            if (id != equipmentInventory.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(equipmentInventory);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Equipment Inventory updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentInventoryExists(equipmentInventory.Id))
                {
                    return NotFound();
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the Equipment Inventory.";
                    // Optionally, log the exception for debugging purposes.
                    // logger.LogError(ex, "An error occurred while updating the Equipment Inventory.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the Equipment Inventory: " + ex.Message;
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += " Inner Exception: " + ex.InnerException.Message;
                }
            }

            return View(equipmentInventory);
        }



        // GET: EquipmentInventory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentInventory = await _context.EquipmentInventory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipmentInventory == null)
            {
                return NotFound();
            }

            return View(equipmentInventory);
        }

        // POST: EquipmentInventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var equipmentInventory = await _context.EquipmentInventory.FindAsync(id);
                if (equipmentInventory == null)
                {
                    return NotFound();
                }

                Console.WriteLine("Deleting equipment inventory with ID: " + id); // Add this line

                _context.EquipmentInventory.Remove(equipmentInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }




        private bool EquipmentInventoryExists(int id)
        {
            return _context.EquipmentInventory.Any(e => e.Id == id);
        }


        [HttpGet]
        public IActionResult UploadExcel()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file, int SelectedProductId)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select an Excel file to upload.";
                return RedirectToAction("UploadExcel");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = User.FindFirst(ClaimTypes.Name);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId) || userNameClaim == null)
            {
                TempData["ErrorMessage"] = "User information not found.";
                return RedirectToAction("UploadExcel");
            }

            var userName = userNameClaim.Value;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    List<EquipmentInventory> equipmentList = new List<EquipmentInventory>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        EquipmentInventory equipment = new EquipmentInventory
                        {
                            ProductId = SelectedProductId,
                            ProductName = worksheet.Cells[row, 1].Value.ToString(),
                            ProductCode = worksheet.Cells[row, 2].Value.ToString(),
                            ProductDescription = worksheet.Cells[row, 3].Value.ToString(),
                            Vendor = worksheet.Cells[row, 4].Value.ToString(),
                            SerialNumber = worksheet.Cells[row, 5].Value.ToString(),
                            Quantity = Convert.ToInt32(worksheet.Cells[row, 6].Value),
                            Amount = (decimal)Convert.ToDouble(worksheet.Cells[row, 7].Value),
                            LastMaintenanceDate = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            NextMaintenanceDate = DateTime.Now.AddDays(7),
                            CreatedBy = userName
                        };

                        // Check for duplicates by product name and product code
                        if (_context.EquipmentInventory.Any(e => e.ProductName == equipment.ProductName || e.ProductCode == equipment.ProductCode))
                        {
                            TempData["ErrorMessage"] = "A record with the same Product Name or Product Code already exists.";
                            return RedirectToAction("UploadExcel");
                        }

                        equipmentList.Add(equipment);
                    }

                    _context.EquipmentInventory.AddRange(equipmentList);
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "EquipmentInventory added successfully!";
            return RedirectToAction("UploadExcel");
        }

        public IActionResult Search()
        {
            return View();
        }

        public IActionResult LSearch(string searchQuery)
        {
            var results = _context.EquipmentInventory
                .Where(e => e.ProductName.Contains(searchQuery) ||
                            e.ProductCode.Contains(searchQuery) ||
                            e.SerialNumber.Contains(searchQuery))
                .Select(e => new
                {
                    e.Id,
                    e.Product,
                    e.ProductName,
                    e.ProductCode,
                    e.ProductDescription,
                    e.Vendor,
                    e.SerialNumber,
                    e.Quantity,
                    e.Amount,
                    e.LastMaintenanceDate,
                    e.UpdatedAt,
                    e.CreatedAt,
                    e.NextMaintenanceDate,
                    e.CreatedBy
                })
                .ToList();

            return Json(results);
        }

        [HttpPost]
        public JsonResult UpdateUpdatedAt(int equipmentId)
        {
            try
            {
                var equipment = _context.EquipmentInventory.Find(equipmentId);
                if (equipment != null)
                {
                    equipment.UpdatedAt = DateTime.Now;
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Equipment not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // GET: EquipmentInventory/SetNextMaintenanceDate
       
        //[HttpGet]
        //public IActionResult SetNextMaintenanceDate()
        //{
        //    var distinctProducts = _context.Products
        //                            .Select(p => p.ProductName)
        //                            .OrderBy(p => p)
        //                            .ToList();


        //    ViewBag.Products = new SelectList(distinctProducts);

        //    return View();
        //}


        //[HttpPost]
        //public IActionResult SetNextMaintenanceDate(EquipmentInventoryModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var equipmentList = _context.EquipmentInventory
        //            .Where(e => e.Product == model.Product)
        //            .ToList();

        //        foreach (var equipment in equipmentList)
        //        {
        //            equipment.NextMaintenanceDate = model.NextMaintenanceDate;
        //        }

        //        _context.SaveChanges();


        //        TempData["Message"] = "Next maintenance date updated successfully.";
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "An error occurred while updating the next maintenance date.";
        //    }
        //    // If ModelState is not valid, return to the form
        //    var distinctProducts = _context.EquipmentInventory
        //        .Select(e => e.Product)
        //        .Distinct()
        //        .ToList();

        //    ViewBag.Products = new SelectList(distinctProducts);

        //    return View(model);
        //}

        //public IActionResult NextMaintenanceDateSet()
        //{
        //    return View();
        //}
        //[HttpGet]
        //public IActionResult NextMaintenanceDateSet()
        //{
        //    return View(new EquipmentInventoryModel());
        //}

        [HttpGet]
        public IActionResult NextMaintenanceDateSet()
        {
            var distinctProducts = _context.Products
                                .Select(p => p.ProductName)
                                .OrderBy(p => p)
                                .ToList();

            ViewBag.Products = new SelectList(distinctProducts);

            return View();
        }

        [HttpPost]
        public IActionResult NextMaintenanceDateSet(EquipmentInventoryModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the selected product ID
                int productId = model.ProductId;

                // Get all products with the selected ProductId
                var products = _context.EquipmentInventory.Where(p => p.ProductId == productId);

                foreach (var product in products)
                {
                    // Update NextMaintenanceDate
                    product.NextMaintenanceDate = model.NextMaintenanceDate;
                }

                // Save changes
                _context.SaveChanges();

                TempData["Message"] = "Next maintenance date updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while updating the next maintenance date.";
            }

            // Return to the form
            return View(model);
        }




        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        //[HttpPost]
        //public IActionResult SetNextMaintenanceDate(EquipmentInventoryModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Update the NextMaintenanceDate in the EquipmentInventory table
        //        var equipment = _context.EquipmentInventory
        //            .FirstOrDefault(e => e.ProductId == model.ProductId); // Assuming there's a ProductId property in your model

        //        if (equipment != null)
        //        {
        //            equipment.NextMaintenanceDate = model.NextMaintenanceDate;
        //            _context.SaveChanges();

        //            TempData["Message"] = "Next Maintenance Date updated successfully.";
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Equipment not found.";
        //        }
        //    }

        //    return RedirectToAction("SetNextMaintenanceDate");
        //}






    }
}
