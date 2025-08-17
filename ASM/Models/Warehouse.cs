using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [ScaffoldColumn(false)] // không render trong form
        [Display(Name = "Mã kho")]
        public string Code { get; set; }


        [Required(ErrorMessage = "Tên kho là bắt buộc.")]
        [Display(Name = "Tên kho")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Địa chỉ chi tiết là bắt buộc.")]
        [Display(Name = "Địa chỉ chi tiết (Số nhà, đường, phường/xã)")]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage = "Tỉnh/Thành phố là bắt buộc.")]
        [Display(Name = "Tỉnh / Thành phố")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Quận/Huyện là bắt buộc.")]
        [Display(Name = "Quận / Huyện")]
        public string District { get; set; }

        [Display(Name = "Người liên hệ")]
        public string? ContactPerson { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }
    }
}