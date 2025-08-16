using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ImportController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Import
        public async Task<IActionResult> Index()
        {
            var importSlips = await _context.ImportSlips
                .Include(i => i.User)
                .Include(i => i.Supplier)
                .OrderByDescending(i => i.ImportDate)
                .ToListAsync();

            return View(importSlips);
        }

        // GET: /Import/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var importSlip = await _context.ImportSlips
                .Include(i => i.User)
                .Include(i => i.Supplier)
                .Include(i => i.ImportSlipDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (importSlip == null) return NotFound();

            return View(importSlip);
        }

        // GET: /Import/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.SupplierList = new SelectList(
                await _context.Suppliers.OrderBy(s => s.TenNCC).ToListAsync(),
                "MaNCC", "TenNCC");

            return View(new ImportSlipViewModel
            {
                ImportDate = DateTime.Now // Mặc định hôm nay
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImportSlipViewModel viewModel)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ModelState.IsValid || viewModel.Details == null || !viewModel.Details.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm.");
                ViewBag.SupplierList = new SelectList(
                    await _context.Suppliers.OrderBy(s => s.TenNCC).ToListAsync(),
                    "MaNCC", "TenNCC", viewModel.MaNCC);
                return View(viewModel);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {


                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    // Bắt logout và quay về login
                    return RedirectToAction("Login", "Account");
                }

                var importSlip = new ImportSlip
                {
                    ImportDate = viewModel.ImportDate,
                    MaNCC = viewModel.MaNCC,
                    UserId = userId
                };

                _context.ImportSlips.Add(importSlip);
                await _context.SaveChangesAsync();

                // Thêm chi tiết phiếu nhập
                foreach (var detail in viewModel.Details)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product == null)
                        throw new Exception($"Sản phẩm ID {detail.ProductId} không tồn tại.");

                    _context.ImportSlipDetails.Add(new ImportSlipDetail
                    {
                        ImportSlipId = importSlip.Id,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        ImportPrice = detail.ImportPrice
                    });

                    // Cập nhật tồn kho
                    product.Quantity += detail.Quantity;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Tạo phiếu nhập thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Đã xảy ra lỗi khi tạo phiếu nhập: {errorMessage}");

                ViewBag.SupplierList = new SelectList(
                    await _context.Suppliers.OrderBy(s => s.TenNCC).ToListAsync(),
                    "MaNCC", "TenNCC", viewModel.MaNCC);
                return View(viewModel);
            }
        }


        // AJAX: Lấy sản phẩm theo nhà cung cấp
        [HttpGet]
        public async Task<JsonResult> GetProductsBySupplier(string supplierId)
        {
            if (string.IsNullOrEmpty(supplierId))
            {
                return Json(new SelectList(Enumerable.Empty<SelectListItem>()));
            }

            var products = await _context.Products
                .Where(p => p.MaNCC == supplierId)
                .Select(p => new { Value = p.Id, Text = p.Name })
                .OrderBy(p => p.Text)
                .ToListAsync();

            return Json(products);
        }
    }
}
