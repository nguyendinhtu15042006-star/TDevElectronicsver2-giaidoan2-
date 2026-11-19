using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TDevElectronics.Models.Entities;

namespace TDevElectronics.Data.Configurations
{
    /// <summary>Cấu hình bảng Quote. Giai đoạn 1 CHƯA được áp dụng (AppDbContext chưa có DbSet).
    /// Giai đoạn 2: thêm DbSet rồi gọi modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly) và Add-Migration.</summary>
    public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> b)
        {
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.AmountBeforeTax).HasPrecision(18, 2);
            b.Property(x => x.Budget).HasPrecision(18, 2);
            b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            b.HasMany(x => x.Items).WithOne(i => i.Quote).HasForeignKey(i => i.QuoteId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
