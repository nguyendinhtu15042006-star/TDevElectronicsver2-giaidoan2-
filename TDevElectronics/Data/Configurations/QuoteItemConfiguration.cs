using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TDevElectronics.Models.Entities;

namespace TDevElectronics.Data.Configurations
{
    /// <summary>Cấu hình bảng QuoteItem. Giai đoạn 1 CHƯA được áp dụng (AppDbContext chưa có DbSet).
    /// Giai đoạn 2: thêm DbSet rồi gọi modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly) và Add-Migration.</summary>
    public class QuoteItemConfiguration : IEntityTypeConfiguration<QuoteItem>
    {
        public void Configure(EntityTypeBuilder<QuoteItem> b)
        {
            b.Property(x => x.QuotedUnitPrice).HasPrecision(18, 2);
        }
    }
}
