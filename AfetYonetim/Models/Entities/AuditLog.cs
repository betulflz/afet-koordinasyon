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
        [MaxLength(200)]
        [Display(Name = "İşlem")]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Display(Name = "Tablo Adı")]
        public string EntityName { get; set; } = string.Empty;

        [MaxLength(450)]
        [Display(Name = "Kayıt Id")]
        public string? EntityId { get; set; }

        [Required]
        [Display(Name = "Zaman")]
        public DateTime Timestamp { get; set; }

        
    }
}