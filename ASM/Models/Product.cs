using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    public class Product
    {
        // === THUỘC TÍNH DỮ LIỆU CHÍNH ===
        [Display(Name = "Ảnh sản phẩm")]
        public string? ImagePath { get; set; }


        [Key]
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
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        // === KHÓA NGOẠI (Foreign Keys) ===

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn kho hàng.")]
        [Display(Name = "Kho hàng")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp.")]
        [Display(Name = "Nhà cung cấp")]
        public string MaNCC { get; set; } = null!;

        // === THUỘC TÍNH ĐIỀU HƯỚNG (Navigation Properties) ===
        // [ValidateNever] để ngăn ASP.NET Core kiểm tra các đối tượng này, tránh lỗi lặp vô hạn.

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public virtual Category Category { get; set; }

        [ForeignKey("WarehouseId")]
        [ValidateNever]
        public virtual Warehouse Warehouse { get; set; }

        [ForeignKey("MaNCC")]
        [ValidateNever]
        public virtual Supplier Supplier { get; set; }

        [ValidateNever]
        public virtual ICollection<ImportSlipDetail> ImportSlipDetails { get; set; } = new List<ImportSlipDetail>();

        [ValidateNever]
        public virtual ICollection<ExportSlipDetail> ExportSlipDetails { get; set; } = new List<ExportSlipDetail>();
    }
}
