using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cell_line_laboratory.Data;
using Cell_line_laboratory.Entities;
using Cell_line_laboratory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cell_line_laboratory.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly Cell_line_laboratoryContext _context;

        public MaintenanceController(Cell_line_laboratoryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Implement any additional logic for the index page if needed
            return View();
        }

        [HttpGet]
        public IActionResult GetMaintenanceHistory(int equipmentId)
        {
            // Retrieve maintenance history for the given equipmentId
            var maintenanceHistory = _context.Maintenances
                .Where(m => m.EquipmentId == equipmentId)
                .OrderByDescending(m => m.Date)
                .ToList();

            return Json(maintenanceHistory);
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Retrieve your list of products from the database
            var products = _context.Products.ToList(); // Replace _context.Products with your actual data access code

            return Ok(products);
        }

        [HttpGet]
        public IActionResult SetNextMaintenanceDate()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateMaintenance([FromBody] MaintenanceDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var maintenance = new Maintenance
                    {
                        EquipmentId = model.EquipmentId,
                        Quantity = model.Quantity,
                        Note = model.Note,
                        Date = model.Date,
                        //NextMaintenance = equipment
                        NextMaintenance = model.NextMaintenance,
                        MaintainedById = model.MaintainedBy
                    };

                    _context.Maintenances.Add(maintenance);
                    _context.SaveChanges();

                    List<Maintenance> maintenanceHistory = _context.Maintenances
                        .Where(m => m.EquipmentId == model.EquipmentId)
                        .OrderByDescending(m => m.Date)
                        .ToList();

                    return Json(new { success = true, maintenanceHistory = maintenanceHistory });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = false, message = "Invalid data submitted." });
        }





    }
}
