using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    // Dùng để hiển thị trên form tạo phiếu nhập
    public class ImportSlipViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập ngày nhập kho.")]
        [Display(Name = "Ngày nhập")]
        public DateTime ImportDate { get; set; } = DateTime.Now;

        // Danh sách các sản phẩm sẽ được nhập
        public List<ImportSlipDetailViewModel> Details { get; set; } = new List<ImportSlipDetailViewModel>();
    }

    public class ImportSlipDetailViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn sản phẩm.")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá nhập phải lớn hơn 0.")]
        public decimal ImportPrice { get; set; }
    }
}
