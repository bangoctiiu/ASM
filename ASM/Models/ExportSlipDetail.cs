using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    public class ExportSlipDetail
    {
        [Key]
        public int Id { get; set; }

        // --- Mối quan hệ với Phiếu Xuất (ExportSlip) ---
        [Required]
        public int ExportSlipId { get; set; }

        [ForeignKey("ExportSlipId")]
        public virtual ExportSlip ExportSlip { get; set; }

        // --- Mối quan hệ với Sản phẩm (Product) ---
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required(ErrorMessage = "Số lượng xuất là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng xuất phải lớn hơn 0.")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Đơn giá tại thời điểm xuất")]
        // NÂNG CẤP: Đổi tên 'ExportPrice' thành 'Price' cho ngắn gọn và đúng chuẩn.
        // Việc lưu lại giá sản phẩm tại đúng thời điểm xuất kho là rất quan trọng để đảm bảo tính chính xác của dữ liệu lịch sử.
        public decimal Price { get; set; }
    }
}