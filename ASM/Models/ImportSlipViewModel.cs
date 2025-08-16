using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    /// <summary>
    /// ViewModel này đại diện cho dữ liệu trên form "Tạo Phiếu Nhập Kho".
    /// Nó là một "gói" dữ liệu tạm thời, không được lưu trực tiếp vào database.
    /// </summary>
    public class ImportSlipViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn ngày nhập kho.")]
        [Display(Name = "Ngày nhập")]
        public DateTime ImportDate { get; set; } = DateTime.Now;

        // --- THÊM MỚI ---
        // Thuộc tính này để lưu MaNCC của nhà cung cấp được chọn từ dropdown.
        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp.")]
        [Display(Name = "Nhà cung cấp")]
        public string MaNCC { get; set; }
        // --- KẾT THÚC THÊM MỚI ---

        // Danh sách các sản phẩm chi tiết sẽ được nhập kho.
        // Phải có ít nhất một sản phẩm.
        [Required]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một sản phẩm trong phiếu nhập.")]
        public List<ImportSlipDetailViewModel> Details { get; set; } = new List<ImportSlipDetailViewModel>();
    }

    /// <summary>
    /// ViewModel này đại diện cho một dòng sản phẩm trong bảng chi tiết phiếu nhập.
    /// </summary>
    public class ImportSlipDetailViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn sản phẩm hợp lệ.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá nhập phải lớn hơn 0.")]
        [Display(Name = "Giá nhập")]
        public decimal ImportPrice { get; set; }
    }
}
