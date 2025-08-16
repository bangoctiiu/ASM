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
    public class ExportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExportController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Export
        public async Task<IActionResult> Index()
        {
            var exportSlips = await _context.ExportSlips
                .Include(e => e.User)
                .Include(e => e.Customer)
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
                .Include(e => e.ExportSlipDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exportSlip == null) return NotFound();
            return View(exportSlip);
        }

        // GET: /Export/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ProductList = new SelectList(
                await _context.Products
                    .Where(p => p.Quantity > 0)
                    .OrderBy(p => p.Name)
                    .ToListAsync(),
                "Id",
                "Name"
            );

            ViewBag.CustomerList = new SelectList(
                await _context.Customers
                    .OrderBy(c => c.FullName)
                    .ToListAsync(),
                "Id",
                "FullName"
            );

            return View(new ExportSlipViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExportSlipViewModel viewModel)
        {
            var availableProducts = await _context.Products.Where(p => p.Quantity > 0).ToListAsync();

            if (!ModelState.IsValid || viewModel.Details == null || !viewModel.Details.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm.");
                ViewBag.ProductList = new SelectList(availableProducts, "Id", "Name");
                ViewBag.CustomerList = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "Id", "FullName", viewModel.CustomerId);
                return View(viewModel);
            }

            // Kiểm tra tồn kho
            foreach (var detail in viewModel.Details)
            {
                var product = availableProducts.FirstOrDefault(p => p.Id == detail.ProductId);
                if (product == null)
                    ModelState.AddModelError("", $"Sản phẩm ID {detail.ProductId} không tồn tại.");
                else if (detail.Quantity > product.Quantity)
                    ModelState.AddModelError("", $"Sản phẩm '{product.Name}' chỉ còn {product.Quantity} trong kho.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProductList = new SelectList(availableProducts, "Id", "Name");
                ViewBag.CustomerList = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "Id", "FullName", viewModel.CustomerId);
                return View(viewModel);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var exportSlip = new ExportSlip
                {
                    ExportDate = viewModel.ExportDate,
                    Reason = viewModel.Reason,
                    UserId = _userManager.GetUserId(User),
                    CustomerId = viewModel.CustomerId
                };
                _context.ExportSlips.Add(exportSlip);
                await _context.SaveChangesAsync();

                foreach (var detail in viewModel.Details)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product == null)
                        throw new Exception($"Sản phẩm ID {detail.ProductId} không tồn tại.");

                    _context.ExportSlipDetails.Add(new ExportSlipDetail
                    {
                        ExportSlipId = exportSlip.Id,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        ExportPrice = product.Price
                    });

                    // Trừ tồn kho
                    product.Quantity -= detail.Quantity;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Tạo phiếu xuất kho thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Đã xảy ra lỗi khi tạo phiếu xuất: {errorMessage}");
                ViewBag.ProductList = new SelectList(availableProducts, "Id", "Name");
                ViewBag.CustomerList = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "Id", "FullName", viewModel.CustomerId);
                return View(viewModel);
            }
        }

        // AJAX: Lấy tồn kho sản phẩm
        [HttpGet]
        public async Task<JsonResult> GetProductStock(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { error = "Không tìm thấy sản phẩm." });
            }
            return Json(new { quantity = product.Quantity });
        }
    }
}
