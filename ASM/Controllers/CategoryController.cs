using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize(Roles = "Admin")] // CHỈ ADMIN MỚI CÓ QUYỀN TRUY CẬP
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        // Danh sách danh mục hợp lệ
        private readonly List<string> allowedCategories = new List<string>
        {
            "Bút",
            "Vở và giấy",
            "Đồ dùng học sinh",
            "Họa phẩm",
            "Sách và tài liệu",
            "Máy tính",
                        "test"

        };

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (!allowedCategories.Contains(category.Name))
            {
                ModelState.AddModelError("Name", "Chỉ được chọn các danh mục có sẵn.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id) return NotFound();

            if (!allowedCategories.Contains(category.Name))
            {
                ModelState.AddModelError("Name", "Chỉ được chọn các danh mục có sẵn.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.Id == category.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products) // nạp danh sách sản phẩm liên quan
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            // Nếu vẫn còn sản phẩm thì không cho xóa
            if (category.Products.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục này vì vẫn còn sản phẩm liên kết.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
