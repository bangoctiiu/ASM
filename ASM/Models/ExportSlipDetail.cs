using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    public class ExportSlipDetail
    {
        public int Id { get; set; }
        public int ExportSlipId { get; set; }
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ExportPrice { get; set; }
        public ExportSlip ExportSlip { get; set; }
        public Product Product { get; set; }
    }
}
