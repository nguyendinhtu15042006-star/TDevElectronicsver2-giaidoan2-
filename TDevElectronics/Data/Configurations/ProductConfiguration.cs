using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TDevElectronics.Models.Entities;

namespace TDevElectronics.Data.Configurations
{
    /// <summary>Cấu hình bảng Product. Giai đoạn 1 CHƯA được áp dụng (AppDbContext chưa có DbSet).
    /// Giai đoạn 2: thêm DbSet rồi gọi modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly) và Add-Migration.</summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> b)
        {
            b.HasIndex(x => x.Sku).IsUnique();
            b.HasOne(x => x.Category).WithMany(c => c.Products).HasForeignKey(x => x.CategoryId);
            b.HasMany(x => x.Images).WithOne(i => i.Product).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.PriceTiers).WithOne(t => t.Product).HasForeignKey(t => t.ProductId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
