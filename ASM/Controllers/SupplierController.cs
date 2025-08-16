using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASM.Data;
using ASM.Models;

namespace ASM.Controllers
{
    public class SupplierController : Controller
    {
        private readonly AppDbContext _context;

        public SupplierController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Supplier
        // Action này đã được nâng cấp để xử lý tìm kiếm và phân trang.
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            var suppliersQuery = _context.Suppliers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                suppliersQuery = suppliersQuery.Where(s =>
                    s.TenNCC.ToLower().Contains(searchString.ToLower()) ||
                    s.Email.ToLower().Contains(searchString.ToLower()));
            }

            int pageSize = 10;
            int totalItems = await suppliersQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var suppliersForPage = await suppliersQuery
                .OrderByDescending(s => s.MaNCC)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new SupplierIndexViewModel
            {
                Suppliers = suppliersForPage,
                SearchString = searchString,
                PageNumber = pageNumber,
                TotalPages = totalPages
            };

            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }

            return View(viewModel);
        }
        // === CÁC PHƯƠNG THỨC VALIDATION MỚI CHO [Remote] ===

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsTenNCCUnique(string tenNCC)
        {
            var exists = _context.Suppliers.Any(s => s.TenNCC.ToLower() == tenNCC.ToLower());
            return exists ? Json($"Tên nhà cung cấp '{tenNCC}' đã tồn tại.") : Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsContactNameUnique(string contactName)
        {
            var exists = _context.Suppliers.Any(s => s.ContactName.ToLower() == contactName.ToLower());
            return exists ? Json($"Tên liên hệ '{contactName}' đã tồn tại.") : Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsSDTUnique(string sdt)
        {
            var exists = _context.Suppliers.Any(s => s.SDT == sdt);
            return exists ? Json($"Số điện thoại '{sdt}' đã tồn tại.") : Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsEmailUnique(string email)
        {
            var exists = _context.Suppliers.Any(s => s.Email.ToLower() == email.ToLower());
            return exists ? Json($"Email '{email}' đã được sử dụng.") : Json(true);
        }

        // === KẾT THÚC PHẦN VALIDATION ===

        // GET: Supplier/Details/S000001
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.MaNCC == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Supplier/Create
        public async Task<IActionResult> Create()
        {
            var supplier = new Supplier
            {
                MaNCC = await GenerateNewMaNCC()
            };
            return View(supplier);
        }

        // POST: Supplier/Create (ĐÃ NÂNG CẤP)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNCC,TenNCC,ContactName,DiaChi,SDT,Email")] Supplier supplier)
        {
            supplier.MaNCC = await GenerateNewMaNCC();

            // Lớp kiểm tra bảo vệ ở phía server (quan trọng!)
            if (_context.Suppliers.Any(s => s.TenNCC.ToLower() == supplier.TenNCC.ToLower()))
            {
                ModelState.AddModelError("TenNCC", "Tên nhà cung cấp này đã tồn tại.");
            }
            if (_context.Suppliers.Any(s => s.SDT == supplier.SDT))
            {
                ModelState.AddModelError("SDT", "Số điện thoại này đã tồn tại.");
            }
            if (_context.Suppliers.Any(s => s.Email.ToLower() == supplier.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Nhà cung cấp đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, quay lại form Create và hiển thị các lỗi
            return View(supplier);
        }

        // GET: Supplier/Edit/S000001
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Supplier/Edit/S000001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNCC,TenNCC,ContactName,DiaChi,SDT,Email")] Supplier supplier)
        {
            if (id != supplier.MaNCC)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thông tin nhà cung cấp đã được cập nhật!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.MaNCC))
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
            return View(supplier);
        }

        // GET: Supplier/Delete/S000001
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.MaNCC == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

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


        private async Task<string> GenerateNewMaNCC()
        {
            var lastSupplier = await _context.Suppliers
                .OrderByDescending(s => s.MaNCC)
                .FirstOrDefaultAsync();

            int newId = 1;
            if (lastSupplier != null && lastSupplier.MaNCC.Length > 1 && int.TryParse(lastSupplier.MaNCC.Substring(1), out int lastId))
            {
                newId = lastId + 1;
            }

            return $"S{newId:D6}";
        }

        private bool SupplierExists(string id)
        {
            return _context.Suppliers.Any(e => e.MaNCC == id);
        }
    }
}
