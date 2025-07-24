using ASM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASM.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ImportSlip> ImportSlips { get; set; }
        public DbSet<ImportSlipDetail> ImportSlipDetails { get; set; }
        public DbSet<ExportSlip> ExportSlips { get; set; }
        public DbSet<ExportSlipDetail> ExportSlipDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =================================================================
            // SỬA LỖI: Cấu hình tường minh các mối quan hệ (Fluent API)
            // =================================================================

            // Mối quan hệ: Category <--> Product (Một Category có nhiều Product)
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                // Quan trọng: Khi xóa Category, không cho phép xóa nếu còn Product liên quan.
                // Điều này an toàn hơn 'ON DELETE CASCADE' trong script SQL của bạn.
                .OnDelete(DeleteBehavior.Restrict);

            // Mối quan hệ: Warehouse <--> Product (Một Warehouse có nhiều Product)
            builder.Entity<Product>()
                .HasOne(p => p.Warehouse)
                .WithMany(w => w.Products)
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình cho các bảng giao dịch (để đảm bảo tính nhất quán)
            builder.Entity<ImportSlipDetail>()
                .HasOne(id => id.Product)
                .WithMany(p => p.ImportSlipDetails)
                .HasForeignKey(id => id.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa Product, xóa luôn lịch sử nhập

            builder.Entity<ExportSlipDetail>()
                .HasOne(ed => ed.Product)
                .WithMany(p => p.ExportSlipDetails)
                .HasForeignKey(ed => ed.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa Product, xóa luôn lịch sử xuất

            // Cấu hình rút ngắn key của Identity (giữ nguyên)
            builder.Entity<IdentityUserLogin<string>>(b => { b.Property(l => l.LoginProvider).HasMaxLength(128); b.Property(l => l.ProviderKey).HasMaxLength(128); });
            builder.Entity<IdentityUserRole<string>>(b => { b.Property(r => r.UserId).HasMaxLength(128); b.Property(r => r.RoleId).HasMaxLength(128); });
            builder.Entity<IdentityUserToken<string>>(b => { b.Property(t => t.UserId).HasMaxLength(128); b.Property(t => t.LoginProvider).HasMaxLength(128); b.Property(t => t.Name).HasMaxLength(128); });
        }
    }
}
