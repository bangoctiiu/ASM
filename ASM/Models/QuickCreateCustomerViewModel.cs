using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class QuickCreateCustomerViewModel
    {
        [Required(ErrorMessage = "Họ và tên đệm là bắt buộc.")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc.")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(15)]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})\b$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        // THÊM MỚI: Thêm trường địa chỉ vào ViewModel
        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(250)]
        public string AddressLine1 { get; set; }
    }
}