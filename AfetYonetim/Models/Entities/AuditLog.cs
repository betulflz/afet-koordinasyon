using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Entities
{
    public class AuditLog : BaseEntity
    {
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        [Display(Name = "İşlem")]
        public Enums.AuditAction Action { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Tablo Adı")]
        public string EntityName { get; set; } = string.Empty;

        [MaxLength(50)]
        [Display(Name = "Kayıt Id")]
        public string? EntityId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [MaxLength(100)]
        [Display(Name = "Kullanıcı Anlık")]
        public string? UserFullNameSnapshot { get; set; }

        [Required]
        [Display(Name = "Zaman")]
        public DateTime Timestamp { get; set; }
    }
}