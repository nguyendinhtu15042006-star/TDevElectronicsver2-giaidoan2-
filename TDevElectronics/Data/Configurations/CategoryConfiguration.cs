using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TDevElectronics.Models.Entities;

namespace TDevElectronics.Data.Configurations
{
    /// <summary>Cấu hình bảng Category. Giai đoạn 1 CHƯA được áp dụng (AppDbContext chưa có DbSet).
    /// Giai đoạn 2: thêm DbSet rồi gọi modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly) và Add-Migration.</summary>
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.HasIndex(x => x.Slug).IsUnique();
        }
    }
}
