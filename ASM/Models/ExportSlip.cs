using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    public class ExportSlip
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã phiếu xuất là bắt buộc.")]
        [StringLength(50)]
        [Display(Name = "Mã phiếu xuất")]
        // NÂNG CẤP: Thêm mã phiếu xuất để dễ dàng theo dõi và quản lý.
        // Mã này sẽ được tự động tạo trong Controller.
        public string ExportSlipCode { get; set; }

        [Required(ErrorMessage = "Ngày xuất kho là bắt buộc.")]
        [Display(Name = "Ngày xuất")]
        public DateTime ExportDate { get; set; }

        [Required(ErrorMessage = "Lý do xuất là bắt buộc.")]
        [StringLength(250, ErrorMessage = "Lý do không được vượt quá 250 ký tự.")]
        [Display(Name = "Lý do")]
        public string Reason { get; set; }

        // --- Mối quan hệ với Người dùng (Người tạo phiếu) ---
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [Display(Name = "Người tạo phiếu")]
        public virtual ApplicationUser User { get; set; }

        // --- Mối quan hệ với Khách hàng (Người nhận hàng) ---
        [Required(ErrorMessage = "Vui lòng chọn khách hàng.")]
        [Display(Name = "Khách hàng")]
        public int CustomerId { get; set; } // SỬA LỖI: Chuyển thành bắt buộc (Required)

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        // --- Mối quan hệ với Chi tiết Phiếu xuất ---
        public virtual ICollection<ExportSlipDetail> ExportSlipDetails { get; set; } = new List<ExportSlipDetail>();
    }
}