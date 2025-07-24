using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    [Table("Suppliers")]
    public class Supplier
    {
        [Key]
        // ❌ KHÔNG có [Required] vì mã được sinh tự động
        public string MaNCC { get; set; }

        [Required]
        [StringLength(100)]
        public string TenNCC { get; set; }

        [Required]
        [Phone]
        public string SDT { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string DiaChi { get; set; }
    }
}
