using ASM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASM.Data
{
    public class AppDbContext : IdentityDbContext<
        ApplicationUser,
        IdentityRole,
        string,
        IdentityUserClaim<string>,
        IdentityUserRole<string>,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Khai báo tất cả các bảng trong cơ sở dữ liệu
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ImportSlip> ImportSlips { get; set; }
        public DbSet<ImportSlipDetail> ImportSlipDetails { get; set; }
        public DbSet<ExportSlip> ExportSlips { get; set; }
        public DbSet<ExportSlipDetail> ExportSlipDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình cho các bảng Identity
            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
            });

            // --- Cấu hình mối quan hệ cho Product ---
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // SỬA LỖI: Bỏ phần .WithMany(w => w.Products) vì nó không còn tồn tại trong Model Warehouse
            builder.Entity<Product>()
                .HasOne(p => p.Warehouse)
                .WithMany() // <-- Bỏ trống phần này
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany()
                .HasForeignKey(p => p.MaNCC)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình mối quan hệ cho ImportSlip ---
            builder.Entity<ImportSlip>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ImportSlip>()
                .HasOne(i => i.Supplier)
                .WithMany()
                .HasForeignKey(i => i.MaNCC)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình mối quan hệ cho ExportSlip ---
            builder.Entity<ExportSlip>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình chi tiết phiếu nhập ---
            builder.Entity<ImportSlipDetail>()
                .HasOne(id => id.Product)
                .WithMany(p => p.ImportSlipDetails)
                .HasForeignKey(id => id.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cấu hình chi tiết phiếu xuất ---
            builder.Entity<ExportSlipDetail>()
                .HasOne(ed => ed.Product)
                .WithMany(p => p.ExportSlipDetails)
                .HasForeignKey(ed => ed.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}