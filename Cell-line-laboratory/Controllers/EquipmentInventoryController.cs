using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cell_line_laboratory.Data;
using Cell_line_laboratory.Entities;

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
              return _context.EquipmentInventory != null ? 
                          View(await _context.EquipmentInventory.ToListAsync()) :
                          Problem("Entity set 'Cell_line_laboratoryContext.EquipmentInventory'  is null.");
        }

        // GET: EquipmentInventory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EquipmentInventory == null)
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

        // POST: EquipmentInventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Product,ProductCode,ProductName,ProductDescription,Quantity,LastMaintenanceDate,NextMaintenanceDate,Amount,CreatedAt,UpdatedAt")] EquipmentInventory equipmentInventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipmentInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipmentInventory);
        }

        // GET: EquipmentInventory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EquipmentInventory == null)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Product,ProductCode,ProductName,ProductDescription,Quantity,LastMaintenanceDate,NextMaintenanceDate,Amount,CreatedAt,UpdatedAt")] EquipmentInventory equipmentInventory)
        {
            if (id != equipmentInventory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipmentInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentInventoryExists(equipmentInventory.Id))
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
            return View(equipmentInventory);
        }

        // GET: EquipmentInventory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EquipmentInventory == null)
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
            if (_context.EquipmentInventory == null)
            {
                return Problem("Entity set 'Cell_line_laboratoryContext.EquipmentInventory'  is null.");
            }
            var equipmentInventory = await _context.EquipmentInventory.FindAsync(id);
            if (equipmentInventory != null)
            {
                _context.EquipmentInventory.Remove(equipmentInventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentInventoryExists(int id)
        {
          return (_context.EquipmentInventory?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
