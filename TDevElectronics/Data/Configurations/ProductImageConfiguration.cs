using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TDevElectronics.Models.Entities;

namespace TDevElectronics.Data.Configurations
{
    /// <summary>Cấu hình bảng ProductImage. Giai đoạn 1 CHƯA được áp dụng (AppDbContext chưa có DbSet).
    /// Giai đoạn 2: thêm DbSet rồi gọi modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly) và Add-Migration.</summary>
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> b)
        {
            b.HasIndex(x => new { x.ProductId, x.SortOrder });
        }
    }
}
