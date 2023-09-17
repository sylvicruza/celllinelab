using Cell_line_laboratory.Data;
using Cell_line_laboratory.Entities;
using Cell_line_laboratory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cell_line_laboratory.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Cell_line_laboratoryContext _context;

        public ProductsController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }
        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Products/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(ProductModel product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}

        //// Check for duplicate product names
        //[HttpGet]
        //public IActionResult CheckDuplicate(string productName)
        //{
        //    bool isDuplicate = _context.Products.Any(p => p.ProductName == productName);
        //    return Json(isDuplicate);
        //}
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                // Check if a product with the same name already exists
                if (_context.Products.Any(p => p.ProductName == productModel.ProductName))
                {
                    ModelState.AddModelError("ProductName", "A product with this name already exists.");
                }
                else
                {
                    // Convert ProductModel to Product entity
                    var product = new Product
                    {
                        ProductName = productModel.ProductName
                    };

                    // Add the product to the database
                    _context.Products.Add(product);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Product created successfully.";
                    ModelState.Clear(); // Clear the model state
                }
            }

            return View(productModel);
        }

    }
}
