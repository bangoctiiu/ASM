using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ExportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExportController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // AJAX: Tạo nhanh khách hàng từ popup (ĐÃ NÂNG CẤP)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickCreateCustomer([FromBody] QuickCreateCustomerViewModel viewModel)
        {
            if (!TryValidateModel(viewModel))
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
                return BadRequest(new { message = error?.ErrorMessage ?? "Dữ liệu không hợp lệ." });
            }

            var email = viewModel.Email.Trim().ToLower();
            var phoneNumber = viewModel.PhoneNumber.Trim();

            if (await _context.Customers.AnyAsync(c => c.Email == email))
                return BadRequest(new { message = "Email đã tồn tại." });

            if (await _context.Customers.AnyAsync(c => c.PhoneNumber == phoneNumber))
                return BadRequest(new { message = "Số điện thoại đã tồn tại." });

            // Map từ ViewModel sang Model chính, bao gồm cả AddressLine1
            var newCustomer = new Customer
            {
                FirstName = viewModel.FirstName.Trim(),
                LastName = viewModel.LastName.Trim(),
                Email = email,
                PhoneNumber = phoneNumber,
                AddressLine1 = viewModel.AddressLine1.Trim(), // THÊM MỚI: Gán giá trị địa chỉ
                CustomerCode = await GenerateNewCustomerCode(),
                Status = CustomerStatus.Active,
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = newCustomer.Id,
                text = newCustomer.FullName + " - " + newCustomer.PhoneNumber
            });
        }

        #region Các action và hàm hỗ trợ khác (Không thay đổi)
        // GET: /Export
        public async Task<IActionResult> Index()
        {
            var exportSlips = await _context.ExportSlips
                                        .Include(e => e.Customer)
                                        .Include(e => e.User)
                                        .OrderByDescending(e => e.ExportDate)
                                        .ToListAsync();
            return View(exportSlips);
        }

        // GET: /Export/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var exportSlip = await _context.ExportSlips
                .Include(e => e.User)
                .Include(e => e.Customer)
                .Include(e => e.ExportSlipDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exportSlip == null) return NotFound();
            return View(exportSlip);
        }

        // GET: /Export/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ExportSlipViewModel
            {
                ExportDate = DateTime.Now,
                ExportSlipCode = await GenerateNewExportCode()
            };
            ViewBag.ProductList = new SelectList(
                await _context.Products.Where(p => p.Quantity > 0).OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            return View(viewModel);
        }

        // POST: /Export/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExportSlipViewModel viewModel)
        {
            viewModel.Details.RemoveAll(d => d.ProductId == 0 || d.Quantity <= 0);

            if (viewModel.Details.Count == 0)
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm vào phiếu xuất.");
            else
            {
                var requestedProductIds = viewModel.Details.Select(d => d.ProductId).ToList();
                var productsInDb = await _context.Products.Where(p => requestedProductIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
                foreach (var detail in viewModel.Details)
                {
                    if (!productsInDb.TryGetValue(detail.ProductId, out var product) || product.Quantity < detail.Quantity)
                    {
                        var productName = productsInDb.ContainsKey(detail.ProductId) ? productsInDb[detail.ProductId].Name : "Không xác định";
                        var stockQuantity = productsInDb.ContainsKey(detail.ProductId) ? productsInDb[detail.ProductId].Quantity : 0;
                        ModelState.AddModelError("", $"Số lượng tồn kho của sản phẩm '{productName}' không đủ (còn {stockQuantity}).");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var exportSlip = new ExportSlip
                {
                    ExportSlipCode = await GenerateNewExportCode(),
                    ExportDate = viewModel.ExportDate,
                    Reason = viewModel.Reason,
                    CustomerId = viewModel.CustomerId,
                    UserId = currentUser.Id,
                };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _context.ExportSlips.Add(exportSlip);
                        await _context.SaveChangesAsync();

                        foreach (var detail in viewModel.Details)
                        {
                            var productToUpdate = await _context.Products.FindAsync(detail.ProductId);
                            productToUpdate.Quantity -= detail.Quantity;
                            var exportDetail = new ExportSlipDetail { ExportSlipId = exportSlip.Id, ProductId = detail.ProductId, Quantity = detail.Quantity, Price = productToUpdate.Price };
                            _context.ExportSlipDetails.Add(exportDetail);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = "Tạo phiếu xuất kho thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình lưu phiếu xuất: " + ex.Message);
                    }
                }
            }

            ViewBag.ProductList = new SelectList(await _context.Products.Where(p => p.Quantity > 0).OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            return View(viewModel);
        }

        // AJAX: Tìm kiếm khách hàng cho Select2
        [HttpGet]
        public async Task<IActionResult> SearchCustomers(string term)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(term))
            {
                var lowerTerm = term.ToLower();
                query = query.Where(c => (c.LastName + " " + c.FirstName).ToLower().Contains(lowerTerm) || c.PhoneNumber.Contains(term));
            }

            var customers = await query.Take(20).Select(c => new
            {
                id = c.Id,
                text = c.FullName + " - " + c.PhoneNumber
            }).ToListAsync();

            return Json(new { results = customers });
        }

        // --- Các hàm hỗ trợ khác ---
        [HttpGet]
        public async Task<JsonResult> GetProductStock(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return Json(new { error = "Không tìm thấy sản phẩm." });
            return Json(new { quantity = product.Quantity });
        }

        private async Task<string> GenerateNewExportCode()
        {
            var lastSlip = await _context.ExportSlips.OrderByDescending(e => e.Id).FirstOrDefaultAsync();
            int newId = (lastSlip?.Id ?? 0) + 1;
            return $"PXK-{DateTime.Now:yyyyMMdd}-{newId:D4}";
        }

        private async Task<string> GenerateNewCustomerCode()
        {
            var lastCustomer = await _context.Customers.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
            int newId = (lastCustomer?.Id ?? 0) + 1;
            return $"KH-{newId:D5}";
        }
        #endregion
    }
}