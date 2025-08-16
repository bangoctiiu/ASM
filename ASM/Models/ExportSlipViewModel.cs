using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class ExportSlipViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn ngày xuất kho.")]
        [Display(Name = "Ngày xuất")]
        public DateTime ExportDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng nhập lý do xuất kho.")]
        [Display(Name = "Lý do xuất")]
        public string Reason { get; set; }

        [Display(Name = "Khách hàng")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khách hàng.")]
        public int CustomerId { get; set; }

        [MinLength(1, ErrorMessage = "Phải có ít nhất một sản phẩm trong phiếu xuất.")]
        public List<ExportSlipDetailViewModel> Details { get; set; } = new List<ExportSlipDetailViewModel>();
    }

    public class ExportSlipDetailViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng xuất phải lớn hơn 0.")]
        public int Quantity { get; set; }

        public decimal ExportPrice { get; set; }
    }
}
