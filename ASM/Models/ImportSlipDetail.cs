using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    public class ImportSlipDetail
    {
        public int Id { get; set; }
        public int ImportSlipId { get; set; }
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ImportPrice { get; set; }
        public ImportSlip ImportSlip { get; set; }
        public Product Product { get; set; }
    }
}
