using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class ImportSlip
    {
        public int Id { get; set; }
        [Required]
        public DateTime ImportDate { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ImportSlipDetail> ImportSlipDetails { get; set; } = new List<ImportSlipDetail>();
    }
}
