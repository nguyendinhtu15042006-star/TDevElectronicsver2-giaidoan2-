using Microsoft.EntityFrameworkCore;
using TDevElectronics.Models.Entities;

namespace TDevElectronics.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InviteCode> InviteCodes { get; set; }

        // Các thành viên B, C, D sẽ gửi Entity và bổ sung DbSet tại đây ở các đợt sau

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasDefaultValue("Customer");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });

            // Cấu hình bảng InviteCodes
            modelBuilder.Entity<InviteCode>(entity =>
            {
                entity.HasIndex(i => i.CodeHash).IsUnique();
                entity.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
            });
        }
    }
}