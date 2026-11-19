using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TDevElectronics.Models.Entities;

namespace TDevElectronics.Data.Configurations
{
    /// <summary>Cấu hình bảng Order. Giai đoạn 1 CHƯA được áp dụng (AppDbContext chưa có DbSet).
    /// Giai đoạn 2: thêm DbSet rồi gọi modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly) và Add-Migration.</summary>
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.Subtotal).HasPrecision(18, 2);
            b.Property(x => x.Shipping).HasPrecision(18, 2);
            b.Property(x => x.Tax).HasPrecision(18, 2);
            b.Property(x => x.Total).HasPrecision(18, 2);
            b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            b.Property(x => x.Payment).HasConversion<string>().HasMaxLength(20);
            b.HasMany(x => x.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.CancelRequests).WithOne(c => c.Order).HasForeignKey(c => c.OrderId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
