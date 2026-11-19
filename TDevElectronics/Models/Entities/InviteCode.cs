using System.ComponentModel.DataAnnotations;
using TDevElectronics.Models.Enums;

namespace TDevElectronics.Models.Entities
{
    public class InviteCode
    {
        public int Id { get; set; }

        [Required, MaxLength(128)]
        public string CodeHash { get; set; } = string.Empty;

        [Required, MaxLength(4)]
        public string LastFourChars { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? TargetEmail { get; set; }

        public InviteStatus Status { get; set; } = InviteStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        public int? UsedByUserId { get; set; }
    }
}