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
        public string? Location { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}