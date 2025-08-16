using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASM.Data;
using ASM.Models;

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
        public async Task<IActionResult> Index(string searchString, int? categoryId, string? supplierId, int pageNumber = 1)
        {
            var productsQuery = _context.Products
                                    .Include(p => p.Category)
                                    .Include(p => p.Warehouse)
                                    .Include(p => p.Supplier)
                                    .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
            }

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(supplierId))
            {
                productsQuery = productsQuery.Where(p => p.MaNCC == supplierId);
            }

            int pageSize = 10;
            int totalItems = await productsQuery.CountAsync();
            var products = await productsQuery
                                .OrderByDescending(p => p.CreatedAt)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            var viewModel = new ProductIndexViewModel
            {
                Products = products,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                SearchString = searchString,
                CategoryId = categoryId,
                SupplierId = supplierId,
                Categories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", categoryId),
                Suppliers = new SelectList(await _context.Suppliers.OrderBy(s => s.TenNCC).ToListAsync(), "MaNCC", "TenNCC", supplierId)
            };

            if (TempData["SuccessMessage"] != null) ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null) ViewData["ErrorMessage"] = TempData["ErrorMessage"];

            return View(viewModel);
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Warehouse)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: /Product/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Quantity,Price,CategoryId,WarehouseId,MaNCC")] Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;

                // Lưu ảnh nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ImagePath = "/images/products/" + uniqueFileName;
                }

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
            await PopulateDropdownsAsync(product.CategoryId, product.WarehouseId, product.MaNCC);
            return View(product);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Quantity,Price,CategoryId,WarehouseId,MaNCC,CreatedAt,ImagePath")] Product product, IFormFile? imageFile)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu có ảnh mới thì lưu lại
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        product.ImagePath = "/images/products/" + uniqueFileName;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật sản phẩm '{product.Name}'.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync(product.CategoryId, product.WarehouseId, product.MaNCC);
            return View(product);
        }

        // GET: /Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Warehouse)
                .Include(p => p.Supplier)
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

        // POST: /Product/DeleteMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(IEnumerable<int> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sản phẩm để xóa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var productsToDelete = await _context.Products
                                                     .Where(p => productIds.Contains(p.Id))
                                                     .ToListAsync();

                if (productsToDelete.Any())
                {
                    _context.Products.RemoveRange(productsToDelete);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã xóa thành công {productsToDelete.Count} sản phẩm.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm do có dữ liệu liên quan.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private async Task PopulateDropdownsAsync(object? selectedCategory = null, object? selectedWarehouse = null, object? selectedSupplier = null)
        {
            ViewBag.CategoryList = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", selectedCategory);
            ViewBag.WarehouseList = new SelectList(await _context.Warehouses.OrderBy(w => w.Name).ToListAsync(), "Id", "Name", selectedWarehouse);
            ViewBag.SupplierList = new SelectList(await _context.Suppliers.OrderBy(s => s.TenNCC).ToListAsync(), "MaNCC", "TenNCC", selectedSupplier);
        }
    }
}
