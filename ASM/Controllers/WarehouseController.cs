using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize(Roles = "Admin")]
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
            return View(await _context.Warehouses.OrderByDescending(w => w.Id).ToListAsync());
        }

        // GET: /Warehouse/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(m => m.Id == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // GET: /Warehouse/Create
        public async Task<IActionResult> Create()
        {
            var warehouse = new Warehouse
            {
                Code = await GenerateNewWarehouseCode()
            };
            return View(warehouse);
        }

        // POST: /Warehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            /*
                SỬA Ở ĐÂY: Thêm "Code" vào để nhận giá trị từ hidden input
            */
            [Bind("Code,Name,AddressLine1,Province,District,ContactPerson,PhoneNumber")] Warehouse warehouse)
        {
            warehouse.Name = warehouse.Name?.Trim();

            if (!string.IsNullOrEmpty(warehouse.Name) && !string.IsNullOrEmpty(warehouse.Province) &&
                _context.Warehouses.Any(w => w.Name == warehouse.Name && w.Province == warehouse.Province))
            {
                ModelState.AddModelError("Name", $"Tên kho '{warehouse.Name}' đã tồn tại trong tỉnh/thành phố '{warehouse.Province}'.");
            }

            if (ModelState.IsValid)
            {
                // Gán lại mã kho mới để đảm bảo tính duy nhất, tránh race condition
                warehouse.Code = await GenerateNewWarehouseCode();

                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo kho hàng thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, tạo lại mã mới để hiển thị lại form
            warehouse.Code = await GenerateNewWarehouseCode();
            return View(warehouse);
        }

        // GET: /Warehouse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        // POST: /Warehouse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name,AddressLine1,Province,District,ContactPerson,PhoneNumber")] Warehouse warehouse)
        {
            if (id != warehouse.Id)
            {
                return NotFound();
            }

            warehouse.Name = warehouse.Name?.Trim();

            if (!string.IsNullOrEmpty(warehouse.Name) && !string.IsNullOrEmpty(warehouse.Province) &&
                _context.Warehouses.Any(w => w.Name == warehouse.Name && w.Province == warehouse.Province && w.Id != id))
            {
                ModelState.AddModelError("Name", $"Tên kho '{warehouse.Name}' đã tồn tại trong tỉnh/thành phố '{warehouse.Province}'.");
            }

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
                    if (!_context.Warehouses.Any(e => e.Id == warehouse.Id))
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
            return View(warehouse);
        }

        // GET: /Warehouse/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FirstOrDefaultAsync(m => m.Id == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: /Warehouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            var hasProducts = await _context.Products.AnyAsync(p => p.WarehouseId == id);
            if (hasProducts)
            {
                TempData["ErrorMessage"] = $"Không thể xóa kho '{warehouse.Name}' vì vẫn còn sản phẩm trong kho.";
                return RedirectToAction(nameof(Index));
            }

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa kho hàng thành công!";

            return RedirectToAction(nameof(Index));
        }

        // HÀM TẠO MÃ KHO
        private async Task<string> GenerateNewWarehouseCode()
        {
            var lastWarehouse = await _context.Warehouses
                .OrderByDescending(w => w.Id)
                .FirstOrDefaultAsync();

            int newId = (lastWarehouse?.Id ?? 0) + 1;

            return $"KHO-{newId:D5}"; // Format: KHO-00001
        }
    }
}