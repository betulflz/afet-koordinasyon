using System.ComponentModel.DataAnnotations;
using AfetYonetim.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfetYonetim.Models.ViewModels.Afetzede
{
    public class HelpRequestCreateViewModel
    {
        [Required(ErrorMessage = "Kategori seçiniz.")]
        [Display(Name = "Kategori")]
        public HelpCategory Category { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [MaxLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Konum en fazla 500 karakter olabilir.")]
        [Display(Name = "Konum (Adres)")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Bölge seçiniz.")]
        [Display(Name = "Bölge")]
        public Guid RegionId { get; set; }

        [Required(ErrorMessage = "Aciliyet seviyesi seçiniz.")]
        [Display(Name = "Aciliyet")]
        public UrgencyLevel Urgency { get; set; } = UrgencyLevel.Orta;

        // Faz 3: Haritadan seçilen koordinat (opsiyonel — null ise Region koordinatı kullanılır)
        [Display(Name = "Enlem")]
        public double? Latitude { get; set; }

        [Display(Name = "Boylam")]
        public double? Longitude { get; set; }

        // Dropdown için
        public List<SelectListItem> RegionList { get; set; } = new();
    }
}