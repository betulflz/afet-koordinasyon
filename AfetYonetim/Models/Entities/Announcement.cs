using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Entities
{
    public class Announcement : BaseEntity
    {

        [Required]
        [MaxLength(450)]
        public string CreatedByAdminId { get; set; } = string.Empty;
        public ApplicationUser? CreatedByAdmin { get; set; }

        [Required]
        [MaxLength(300)]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Acil Duyuru")]
        public bool IsUrgent { get; set; } = false;

        [Display(Name = "Bitiş Tarihi")]
        public DateTime? ExpiresAt{ get; set; }


       
    }
}