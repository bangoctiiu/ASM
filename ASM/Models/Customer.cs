using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required, Phone]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Range(18, 65, ErrorMessage = "Tuổi phải từ 18 đến 65")]
        public int Age { get; set; }

        public string Address { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
    }
}
