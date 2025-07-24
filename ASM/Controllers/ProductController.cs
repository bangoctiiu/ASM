using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize] // Yêu cầu người dùng phải đăng nhập để truy cập
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Product
        // Hiển thị danh sách sản phẩm với chức năng tìm kiếm và phân trang
        public async Task<IActionResult> Index(string searchString, int? categoryId, int pageNumber = 1)
        {
            // Bắt đầu xây dựng truy vấn mà chưa thực thi
            var productsQuery = _context.Products
                                        .Include(p => p.Category)
                                        .Include(p => p.Warehouse)
                                        .AsQueryable();

            // Lọc theo tên sản phẩm nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
            }

            // Lọc theo danh mục nếu có
            if (categoryId.HasValue && categoryId > 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            // Cấu hình phân trang
            const int pageSize = 8;
            var totalProducts = await productsQuery.CountAsync();
            var totalPages = (int)System.Math.Ceiling(totalProducts / (double)pageSize);

            var paginatedProducts = await productsQuery
                                          .OrderByDescending(p => p.Id) // Sắp xếp sản phẩm mới nhất lên đầu
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync();

            // Tạo ViewModel để gửi dữ liệu sang View
            var viewModel = new ProductIndexViewModel
            {
                Products = paginatedProducts,
                Categories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", categoryId),
                SearchString = searchString,
                CategoryId = categoryId,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        // GET: /Product/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: /Product/Create (PHIÊN BẢN ĐÃ ĐƯỢC SỬA LẠI)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            // SỬA 1: Dùng [Bind] để chỉ định rõ các trường được phép nhận dữ liệu từ form.
            // Đây là một biện pháp bảo mật quan trọng để chống tấn công over-posting.
            [Bind("Name,Description,Quantity,Price,CategoryId,WarehouseId")] Product product)
        {
            try
            {
                // Kiểm tra lại ModelState sau khi binding
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã tạo thành công sản phẩm '{product.Name}'.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                // SỬA 2: Bắt lỗi nếu có vấn đề khi lưu vào CSDL và đưa ra thông báo thân thiện.
                // Ghi log lỗi (nếu cần): LogError(ex);
                ModelState.AddModelError("", "Không thể lưu thay đổi. " +
                    "Hãy thử lại, và nếu vấn đề vẫn tiếp diễn, " +
                    "hãy liên hệ người quản trị hệ thống.");
            }

            // Nếu code chạy đến đây, nghĩa là có lỗi xảy ra.
            // Tải lại dropdowns và hiển thị lại form với dữ liệu người dùng đã nhập.
            await PopulateDropdownsAsync(product.CategoryId, product.WarehouseId);
            return View(product);
        }

        // GET: /Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            await PopulateDropdownsAsync(product.CategoryId, product.WarehouseId);
            return View(product);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Quantity,Price,CategoryId,WarehouseId")] Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật thành công sản phẩm '{product.Name}'.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(product.CategoryId, product.WarehouseId);
            return View(product);
        }

        // GET: /Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // --- CÁC HÀM HỖ TRỢ ---
        private bool ProductExists(int id) => _context.Products.Any(e => e.Id == id);

        private async Task PopulateDropdownsAsync(object? selectedCategory = null, object? selectedWarehouse = null)
        {
            ViewBag.CategoryId = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", selectedCategory);
            ViewBag.WarehouseId = new SelectList(await _context.Warehouses.OrderBy(w => w.Name).ToListAsync(), "Id", "Name", selectedWarehouse);
        }
    }
}
