// File: Controllers/WarehouseController.cs
using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize(Roles = "Admin")] // CHỈ ADMIN MỚI CÓ QUYỀN TRUY CẬP
    public class WarehouseController : Controller
    {
        private readonly AppDbContext _context;

        public WarehouseController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Warehouse
        public async Task<IActionResult> Index()
        {
            return View(await _context.Warehouses.ToListAsync());
        }

        // GET: /Warehouse/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Warehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo kho hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(warehouse);
        }

        // GET: /Warehouse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null) return NotFound();
            return View(warehouse);
        }

        // POST: /Warehouse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location")] Warehouse warehouse)
        {
            if (id != warehouse.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warehouse);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật kho hàng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Warehouses.Any(e => e.Id == warehouse.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(warehouse);
        }

        // GET: /Warehouse/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(m => m.Id == id);
            if (warehouse == null) return NotFound();
            return View(warehouse);
        }

        // POST: /Warehouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa kho hàng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}