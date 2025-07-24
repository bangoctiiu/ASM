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

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ImportSlip> ImportSlips { get; set; }
        public DbSet<ImportSlipDetail> ImportSlipDetails { get; set; }
        public DbSet<ExportSlip> ExportSlips { get; set; }
        public DbSet<ExportSlipDetail> ExportSlipDetails { get; set; }

        // ✅ Đổi tên DbSet tránh lỗi trùng
        public DbSet<Supplier> Suppliers { get; set; } // Đặt tên khớp với bảng SQL

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.Property(r => r.UserId).HasColumnType("nvarchar(450)");
                b.Property(r => r.RoleId).HasColumnType("nvarchar(450)");
            });

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Warehouse)
                .WithMany(w => w.Products)
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ImportSlipDetail>()
                .HasOne(id => id.Product)
                .WithMany(p => p.ImportSlipDetails)
                .HasForeignKey(id => id.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExportSlipDetail>()
                .HasOne(ed => ed.Product)
                .WithMany(p => p.ExportSlipDetails)
                .HasForeignKey(ed => ed.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
