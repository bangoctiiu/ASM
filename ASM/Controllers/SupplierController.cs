using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
        public IActionResult Index()
        {
            var suppliers = _context.Suppliers.ToList();
            return View(suppliers);
        }

        // GET: Supplier/Create
        public IActionResult Create()
        {
            var supplier = new Supplier
            {
                MaNCC = GenerateNewMaNCC() // ✅ Gán mã ngay từ đầu
            };
            return View(supplier);
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {
            supplier.MaNCC = GenerateNewMaNCC();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Suppliers.Add(supplier);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Ghi log nếu có lỗi lưu
                    ModelState.AddModelError("", "Lỗi khi lưu vào cơ sở dữ liệu: " + ex.Message);
                }
            }
            else
            {
                // Log lỗi model nếu dữ liệu nhập không hợp lệ
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var err in errors)
                {
                    Debug.WriteLine("Model error: " + err.ErrorMessage);
                }
            }

            return View(supplier);
        }

        // GET: Supplier/Edit/{id}
        public IActionResult Edit(string id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        // POST: Supplier/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Supplier supplier)
        {
            if (id != supplier.MaNCC) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }

            return View(supplier);
        }

        // GET: Supplier/Delete/{id}
        public IActionResult Delete(string id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        // POST: Supplier/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        private string GenerateNewMaNCC()
        {
            var lastSupplier = _context.Suppliers
                .OrderByDescending(s => s.MaNCC)
                .FirstOrDefault();

            int newId = 1;
            if (lastSupplier != null &&
                lastSupplier.MaNCC.Length > 1 &&
                int.TryParse(lastSupplier.MaNCC.Substring(1), out int lastId))
            {
                newId = lastId + 1;
            }

            return $"S{newId.ToString("D6")}";
        }

    }
}
