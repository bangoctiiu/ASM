using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Models
{
    public class ImportSlip
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày nhập kho.")]
        [Display(Name = "Ngày nhập")]
        public DateTime ImportDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn người tạo phiếu.")]
        [Display(Name = "Người tạo phiếu")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public virtual ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp.")]
        [Display(Name = "Nhà cung cấp")]
        public string MaNCC { get; set; }



        // NEW: Liên kết khách hàng
        public int? CustomerId { get; set; }   // để khớp migration nullable hiện có
        public Customer Customer { get; set; }




        [ForeignKey("MaNCC")]
        [ValidateNever]
        public virtual Supplier Supplier { get; set; }

        [ValidateNever]
        public ICollection<ImportSlipDetail> ImportSlipDetails { get; set; } = new List<ImportSlipDetail>();


    }
}
