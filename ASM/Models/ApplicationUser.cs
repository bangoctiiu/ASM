using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public ICollection<ImportSlip> ImportSlips { get; set; } = new List<ImportSlip>();
        public ICollection<ExportSlip> ExportSlips { get; set; } = new List<ExportSlip>();
    }
}