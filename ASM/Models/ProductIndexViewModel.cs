using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ASM.Models
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; }
        public string? SearchString { get; set; }

        // Dữ liệu cho các bộ lọc Dropdown
        public SelectList Categories { get; set; }
        public SelectList Suppliers { get; set; } // <-- THÊM MỚI

        // Lưu lại giá trị đã chọn của bộ lọc
        public int? CategoryId { get; set; }
        public string? SupplierId { get; set; } // <-- THÊM MỚI

        // Thông tin phân trang
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
