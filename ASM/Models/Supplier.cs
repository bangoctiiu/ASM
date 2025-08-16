using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    [Table("Suppliers")]
    public class Supplier
    {
        [Key]
        [DisplayName("Mã NCC")]
        public string MaNCC { get; set; }

        [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên nhà cung cấp không được vượt quá 100 ký tự.")]
        [DisplayName("Tên nhà cung cấp")]
        // Yêu cầu kiểm tra tính duy nhất của TenNCC bằng cách gọi action "IsTenNCCUnique" trong SupplierController
        [Remote(action: "IsTenNCCUnique", controller: "Supplier", ErrorMessage = "Tên nhà cung cấp này đã tồn tại.")]
        public string TenNCC { get; set; }

        [StringLength(100, ErrorMessage = "Tên người liên hệ không được vượt quá 100 ký tự.")]
        [DisplayName("Người liên hệ")]
        [Remote(action: "IsContactNameUnique", controller: "Supplier", ErrorMessage = "Tên người liên hệ này đã tồn tại.")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ.")]
        [DisplayName("Số điện thoại")]
        [Remote(action: "IsSDTUnique", controller: "Supplier", ErrorMessage = "Số điện thoại này đã tồn tại.")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [StringLength(100)]
        [DisplayName("Email")]
        [Remote(action: "IsEmailUnique", controller: "Supplier", ErrorMessage = "Email này đã được sử dụng.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [DisplayName("Địa chỉ")]
        public string DiaChi { get; set; }
    }
}
