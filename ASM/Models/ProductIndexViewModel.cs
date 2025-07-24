using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ASM.Models
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; }
        public SelectList Categories { get; set; }
        public string? SearchString { get; set; }
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}