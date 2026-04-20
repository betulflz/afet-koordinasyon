using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class AnnouncementFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [MaxLength(300, ErrorMessage = "Başlık en fazla 300 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur.")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Acil Duyuru")]
        public bool IsUrgent { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime? ExpiresAt { get; set; }
    }
}