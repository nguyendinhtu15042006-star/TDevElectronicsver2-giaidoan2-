using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TDevElectronics.Models.Entities;

namespace TDevElectronics.Data.Configurations
{
    /// <summary>Cấu hình bảng SiteSetting. Giai đoạn 1 CHƯA được áp dụng (AppDbContext chưa có DbSet).
    /// Giai đoạn 2: thêm DbSet rồi gọi modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly) và Add-Migration.</summary>
    public class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
    {
        public void Configure(EntityTypeBuilder<SiteSetting> b)
        {
            // Key là khóa chính (đã đánh dấu [Key])
        }
    }
}
