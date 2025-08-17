using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class ExportSlipViewModel
    {
        [Display(Name = "Mã phiếu xuất")]
        // NÂNG CẤP: Thêm mã phiếu xuất để hiển thị trên form Create.
        // Mã này sẽ được Controller tạo tự động và không cần validation ở đây.
        public string ExportSlipCode { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày xuất kho.")]
        [Display(Name = "Ngày xuất")]
        public DateTime ExportDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng nhập lý do xuất kho.")]
        [StringLength(250, ErrorMessage = "Lý do không được vượt quá 250 ký tự.")]
        [Display(Name = "Lý do xuất")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khách hàng.")]
        [Display(Name = "Khách hàng")]
        // SỬA LẠI: Dùng [Required] là đủ và rõ ràng hơn [Range].
        public int CustomerId { get; set; }

        // Validation cho danh sách này sẽ được thực hiện trong Controller
        // để đưa ra thông báo lỗi thân thiện hơn.
        public List<ExportSlipDetailViewModel> Details { get; set; } = new List<ExportSlipDetailViewModel>();
    }

    public class ExportSlipDetailViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn sản phẩm hợp lệ.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng xuất phải lớn hơn 0.")]
        public int Quantity { get; set; }

        // LOẠI BỎ: Không cần trường Price ở đây. 
        // Giá sẽ được lấy từ database ở phía server (trong Controller)
        // tại thời điểm tạo phiếu để đảm bảo tính chính xác và an toàn.
    }
}