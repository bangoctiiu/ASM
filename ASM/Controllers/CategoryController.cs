using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền truy cập
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            // Chỉ hiển thị các danh mục đang hoạt động, sắp xếp theo lần cập nhật gần nhất
            var activeCategories = await _context.Categories
                                                 .Where(c => c.IsActive)
                                                 .OrderByDescending(c => c.UpdatedAt)
                                                 .ToListAsync();
            return View(activeCategories);
        }

        // GET: /Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,IsActive")] Category category)
        {
            // Loại bỏ khoảng trắng thừa ở đầu và cuối chuỗi
            category.Name = category.Name.Trim();

            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.Now;
                category.UpdatedAt = DateTime.Now;

                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo danh mục '{category.Name}' thành công!";
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
        // Loại bỏ CreatedAt khỏi Bind để tránh tấn công overposting
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IsActive")] Category category)
        {
            if (id != category.Id) return NotFound();

            category.Name = category.Name.Trim();

            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != id))
            {
                ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _context.Categories.FindAsync(id);
                    if (existingCategory == null) return NotFound();

                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    existingCategory.IsActive = category.IsActive;
                    existingCategory.UpdatedAt = DateTime.Now;

                    _context.Update(existingCategory);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật danh mục '{category.Name}' thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id)) return NotFound();
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
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            // SỬA LỖI: Quay lại kiểm tra xem có sản phẩm nào tồn tại không, thay vì kiểm tra IsActive
            if (category.Products.Any())
            {
                TempData["ErrorMessage"] = $"Không thể xóa '{category.Name}' vì vẫn còn sản phẩm liên kết trong danh mục này.";
                return RedirectToAction(nameof(Index));
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.Now;
            _context.Update(category);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã chuyển danh mục '{category.Name}' sang trạng thái không hoạt động.";
            return RedirectToAction(nameof(Index));
        }

        // Phương thức private để kiểm tra sự tồn tại của danh mục
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
