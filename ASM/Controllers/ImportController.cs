using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize] // Yêu cầu đăng nhập
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
                .Include(i => i.User) // Lấy thông tin người tạo phiếu
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
                .Include(i => i.ImportSlipDetails)
                    .ThenInclude(d => d.Product) // Lấy thông tin sản phẩm trong chi tiết phiếu
                .FirstOrDefaultAsync(m => m.Id == id);

            if (importSlip == null) return NotFound();

            return View(importSlip);
        }

        // GET: /Import/Create
        public async Task<IActionResult> Create()
        {
            // Chuẩn bị danh sách sản phẩm cho dropdown
            ViewBag.ProductList = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            var model = new ImportSlipViewModel();
            return View(model);
        }

        // POST: /Import/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImportSlipViewModel viewModel)
        {
            // Sử dụng transaction để đảm bảo toàn vẹn dữ liệu
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        // 1. Tạo phiếu nhập chính
                        var importSlip = new ImportSlip
                        {
                            ImportDate = viewModel.ImportDate,
                            UserId = _userManager.GetUserId(User) // Lấy ID của người dùng đang đăng nhập
                        };
                        _context.Add(importSlip);
                        await _context.SaveChangesAsync(); // Lưu để lấy Id của phiếu nhập

                        // 2. Xử lý từng chi tiết sản phẩm và cập nhật tồn kho
                        foreach (var detailViewModel in viewModel.Details)
                        {
                            var product = await _context.Products.FindAsync(detailViewModel.ProductId);
                            if (product == null)
                            {
                                // Nếu sản phẩm không tồn tại, rollback và báo lỗi
                                await transaction.RollbackAsync();
                                ModelState.AddModelError("", $"Sản phẩm với ID {detailViewModel.ProductId} không tồn tại.");
                                ViewBag.ProductList = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
                                return View(viewModel);
                            }

                            // Tạo chi tiết phiếu nhập
                            var importDetail = new ImportSlipDetail
                            {
                                ImportSlipId = importSlip.Id,
                                ProductId = detailViewModel.ProductId,
                                Quantity = detailViewModel.Quantity,
                                ImportPrice = detailViewModel.ImportPrice
                            };
                            _context.Add(importDetail);

                            // CẬP NHẬT SỐ LƯỢNG TỒN KHO
                            product.Quantity += detailViewModel.Quantity;
                        }

                        await _context.SaveChangesAsync(); // Lưu tất cả thay đổi
                        await transaction.CommitAsync(); // Hoàn tất transaction

                        TempData["SuccessMessage"] = "Tạo phiếu nhập kho thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (System.Exception)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình tạo phiếu nhập. Vui lòng thử lại.");
                }
            }

            // Nếu có lỗi, tải lại danh sách sản phẩm và hiển thị lại form
            ViewBag.ProductList = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            return View(viewModel);
        }
    }
}
