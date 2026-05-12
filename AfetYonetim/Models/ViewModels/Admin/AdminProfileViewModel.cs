using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class AdminProfileViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        [Display(Name = "Ad")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [Display(Name = "Soyad")]
        public string Surname { get; set; } = string.Empty;

        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;
    }
}