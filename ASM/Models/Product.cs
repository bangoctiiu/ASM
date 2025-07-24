using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        [StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được là số âm.")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        // SỬA LỖI Ở ĐÂY: Thêm [Range] để bắt buộc người dùng phải chọn một mục
        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn một danh mục hợp lệ.")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        // SỬA LỖI Ở ĐÂY: Thêm [Range] để bắt buộc người dùng phải chọn một mục
        [Required(ErrorMessage = "Vui lòng chọn kho hàng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn một kho hàng hợp lệ.")]
        [Display(Name = "Kho hàng")]
        public int WarehouseId { get; set; }

        // Navigation Properties (giữ nguyên)
        public virtual Category Category { get; set; }
        public virtual Warehouse Warehouse { get; set; }

        public virtual ICollection<ImportSlipDetail> ImportSlipDetails { get; set; } = new List<ImportSlipDetail>();
        public virtual ICollection<ExportSlipDetail> ExportSlipDetails { get; set; } = new List<ExportSlipDetail>();
    }
}
