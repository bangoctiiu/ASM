using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

// Đảm bảo namespace này khớp với project của bạn
namespace ASM.Models
{
    public class SupplierIndexViewModel
    {
        // Danh sách các nhà cung cấp sẽ hiển thị trên trang hiện tại
        public List<Supplier> Suppliers { get; set; }

        // Chuỗi tìm kiếm người dùng nhập vào
        public string SearchString { get; set; }

        // Thông tin phân trang
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
