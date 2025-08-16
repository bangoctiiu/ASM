using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(250)]
        public string? Location { get; set; } // Địa chỉ kho (cụ thể)

        public string? Region { get; set; } // Bắc, Trung, Nam, Tây Nguyên
        public string? Province { get; set; } // Tỉnh/Thành phố
        public string? District { get; set; } // Quận/Huyện

        [StringLength(500)]
        public string? MapCoordinates { get; set; } // Lưu lat,lng từ Google Maps

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}