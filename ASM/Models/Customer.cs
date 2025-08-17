using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    // ENUM để quản lý giới tính một cách nhất quán
    public enum GenderType
    {
        [Display(Name = "Nam")]
        Male,

        [Display(Name = "Nữ")]
        Female,

        [Display(Name = "Khác")]
        Other
    }

    // ENUM để quản lý trạng thái của khách hàng
    public enum CustomerStatus
    {
        [Display(Name = "Đang hoạt động")]
        Active,

        [Display(Name = "Ngừng hoạt động")]
        Inactive,

        [Display(Name = "Tiềm năng")]
        Potential
    }

    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(20)]
        [Display(Name = "Mã khách hàng")]
        // NÂNG CẤP: Thêm mã khách hàng để dễ dàng quản lý và tránh trùng lặp.
        // Controller cần kiểm tra tính duy nhất (unique) của mã này.
        public string CustomerCode { get; set; }

        [Required(ErrorMessage = "Họ và tên đệm là bắt buộc.")]
        [StringLength(50)]
        [Display(Name = "Họ và tên đệm")]
        // NÂNG CẤP: Tách riêng Họ và Tên để dễ dàng sắp xếp, tìm kiếm và cá nhân hóa (VD: "Chào anh/chị [Tên]")
        public string LastName { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc.")]
        [StringLength(50)]
        [Display(Name = "Tên")]
        public string FirstName { get; set; }

        [NotMapped] // Thuộc tính này không được tạo trong database
        [Display(Name = "Họ và tên đầy đủ")]
        // TIỆN ÍCH: Tự động ghép họ và tên lại khi cần hiển thị
        public string FullName => $"{LastName} {FirstName}";

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [StringLength(15)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số điện thoại")]
        // Controller cần kiểm tra tính duy nhất (unique) của SĐT.
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [Display(Name = "Địa chỉ Email")]
        // Controller cần kiểm tra tính duy nhất (unique) của Email.
        public string Email { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        // THAY ĐỔI: Lưu ngày sinh thay vì tuổi. Tuổi sẽ được tính toán khi cần.
        // Đây là cách làm đúng chuẩn vì tuổi của khách hàng thay đổi theo thời gian.
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(250)]
        [Display(Name = "Địa chỉ chi tiết")]
        // CẢI TIẾN: Đổi tên 'Address' thành tên rõ ràng hơn.
        public string AddressLine1 { get; set; }

        [Display(Name = "Giới tính")]
        // NÂNG CẤP: Sử dụng Enum để đảm bảo dữ liệu chỉ có thể là Nam, Nữ hoặc Khác.
        public GenderType? Gender { get; set; }

        [Display(Name = "Trạng thái")]
        // NÂNG CẤP: Thêm trạng thái để quản lý khách hàng (VD: khách hàng thân thiết, khách hàng mới, đã khóa...).
        public CustomerStatus Status { get; set; } = CustomerStatus.Active; // Mặc định là "Đang hoạt động"

        [Display(Name = "Ngày tạo")]
        // NÂNG CẤP: Tự động lưu lại ngày khách hàng được tạo.
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}