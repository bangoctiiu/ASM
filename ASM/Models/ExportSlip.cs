using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class ExportSlip
    {
        public int Id { get; set; }

        [Required]
        public DateTime ExportDate { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Thêm mới để fix lỗi
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }


        public ICollection<ExportSlipDetail> ExportSlipDetails { get; set; } = new List<ExportSlipDetail>();
    }
}
