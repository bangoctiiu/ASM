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
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Product
        public async Task<IActionResult> Index(string searchString, int? categoryId, int pageNumber = 1)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Warehouse)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            // Debug số lượng sản phẩm
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Tổng sản phẩm trong DB: {_context.Products.Count()}");

            const int pageSize = 8;
            var totalProducts = await productsQuery.CountAsync();
            var totalPages = (int)System.Math.Ceiling(totalProducts / (double)pageSize);
            var paginatedProducts = await productsQuery
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
            return View(new Product { Quantity = 0 });
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Quantity,Price,CategoryId,WarehouseId,MaNCC")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã tạo thành công sản phẩm '{product.Name}'.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync(product.CategoryId, product.WarehouseId, product.MaNCC);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Quantity,Price,CategoryId,WarehouseId,MaNCC")] Product productFromForm)
        {
            if (id != productFromForm.Id) return NotFound();

            var productToUpdate = await _context.Products.FindAsync(id);
            if (productToUpdate == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm để cập nhật.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    productToUpdate.Name = productFromForm.Name;
                    productToUpdate.Description = productFromForm.Description;
                    productToUpdate.Quantity = productFromForm.Quantity;
                    productToUpdate.Price = productFromForm.Price;
                    productToUpdate.CategoryId = productFromForm.CategoryId;
                    productToUpdate.WarehouseId = productFromForm.WarehouseId;
                    productToUpdate.MaNCC = productFromForm.MaNCC;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật sản phẩm '{productToUpdate.Name}'.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Không thể lưu thay đổi. Vui lòng thử lại.");
                }
            }

            await PopulateDropdownsAsync(productFromForm.CategoryId, productFromForm.WarehouseId, productFromForm.MaNCC);
            return View(productFromForm);
        }


        // GET: /Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.Id == id);

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

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        private async Task PopulateDropdownsAsync(object? selectedCategory = null, object? selectedWarehouse = null, object? selectedMaNCC = null)
        {
            ViewBag.CategoryId = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", selectedCategory);
            ViewBag.WarehouseId = new SelectList(await _context.Warehouses.OrderBy(w => w.Name).ToListAsync(), "Id", "Name", selectedWarehouse);
            ViewBag.MaNCC = new SelectList(await _context.Suppliers.OrderBy(s => s.TenNCC).ToListAsync(), "MaNCC", "TenNCC", selectedMaNCC);
        }


    }
}
