using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceTier = TDevElectronics.Models.Entities.PriceTier;

namespace TDevElectronics.Data.Configurations
{
    /// <summary>Cấu hình bảng PriceTier. Giai đoạn 1 CHƯA được áp dụng (AppDbContext chưa có DbSet).
    /// Giai đoạn 2: thêm DbSet rồi gọi modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly) và Add-Migration.</summary>
    public class PriceTierConfiguration : IEntityTypeConfiguration<PriceTier>
    {
        public void Configure(EntityTypeBuilder<PriceTier> b)
        {
            //b.Property(x => x.UnitPrice).HasPrecision(18, 2);
        }
    }
}
