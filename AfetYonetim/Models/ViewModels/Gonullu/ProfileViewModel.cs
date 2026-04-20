using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfetYonetim.Models.ViewModels.Gonullu
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        [MaxLength(100)]
        [Display(Name = "Ad")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [MaxLength(100)]
        [Display(Name = "Soyad")]
        public string Surname { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Bölge")]
        public Guid? RegionId { get; set; }

        public List<SelectListItem> RegionList { get; set; } = new();

        public string Email { get; set; } = string.Empty;
    }
}