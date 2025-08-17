using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Customer
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.OrderByDescending(c => c.Id).ToListAsync());
        }

        // GET: /Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: /Customer/Create
        public async Task<IActionResult> Create()
        {
            // Tự động tạo mã khách hàng mới và gửi ra View
            var customer = new Customer
            {
                CustomerCode = await GenerateNewCustomerCode()
            };
            return View(customer);
        }

        // POST: /Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CustomerCode,LastName,FirstName,PhoneNumber,Email,DateOfBirth,AddressLine1,Gender,Status")] Customer customer)
        {
            // NÂNG CẤP: Chuẩn hóa dữ liệu đầu vào
            customer.Email = customer.Email?.Trim().ToLower();
            customer.PhoneNumber = customer.PhoneNumber?.Trim();
            customer.FirstName = customer.FirstName?.Trim();
            customer.LastName = customer.LastName?.Trim();

            // ĐIỀU KIỆN: Kiểm tra dữ liệu trùng lặp
            if (_context.Customers.Any(c => c.Email == customer.Email))
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");

            if (_context.Customers.Any(c => c.PhoneNumber == customer.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "Số điện thoại này đã được sử dụng.");

            if (ModelState.IsValid)
            {
                // Gán mã khách hàng mới ngay trước khi lưu
                customer.CustomerCode = await GenerateNewCustomerCode();

                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo khách hàng mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, tạo lại mã mới để hiển thị lại form
            customer.CustomerCode = await GenerateNewCustomerCode();
            return View(customer);
        }

        // GET: /Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: /Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,CustomerCode,LastName,FirstName,PhoneNumber,Email,DateOfBirth,AddressLine1,Gender,Status,CreatedAt")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            // NÂNG CẤP: Chuẩn hóa dữ liệu
            customer.Email = customer.Email?.Trim().ToLower();
            customer.PhoneNumber = customer.PhoneNumber?.Trim();

            // ĐIỀU KIỆN: Kiểm tra trùng lặp khi sửa (trừ chính khách hàng đang sửa)
            if (_context.Customers.Any(c => c.Email == customer.Email && c.Id != id))
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");

            if (_context.Customers.Any(c => c.PhoneNumber == customer.PhoneNumber && c.Id != id))
                ModelState.AddModelError("PhoneNumber", "Số điện thoại này đã được sử dụng.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customers.Any(e => e.Id == customer.Id))
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
            return View(customer);
        }

        // GET: /Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: /Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // ĐIỀU KIỆN AN TOÀN: Kiểm tra xem khách hàng có phiếu xuất nào không
            var isUsedInExport = await _context.ExportSlips.AnyAsync(e => e.CustomerId == id);
            if (isUsedInExport)
            {
                TempData["ErrorMessage"] = $"Không thể xóa khách hàng '{customer.FullName}' vì đã có phiếu xuất liên kết.";
                return RedirectToAction(nameof(Index));
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa khách hàng thành công!";

            return RedirectToAction(nameof(Index));
        }

        // HÀM MỚI: Tự động tạo mã khách hàng
        private async Task<string> GenerateNewCustomerCode()
        {
            var lastCustomer = await _context.Customers
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            int newId = (lastCustomer?.Id ?? 0) + 1;

            return $"KH-{newId:D5}"; // Format: KH-00001
        }
    }
}