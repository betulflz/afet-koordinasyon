using System.ComponentModel.DataAnnotations;
using AfetYonetim.Models.Enums;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class RegionFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Bölge adı zorunludur.")]
        [MaxLength(200, ErrorMessage = "Bölge adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Bölge Adı")]
        public string RegionName { get; set; } = string.Empty;

        [Required(ErrorMessage = "İl zorunludur.")]
        [MaxLength(100, ErrorMessage = "İl en fazla 100 karakter olabilir.")]
        [Display(Name = "İl")]
        public string City { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "İlçe en fazla 100 karakter olabilir.")]
        [Display(Name = "İlçe")]
        public string? District { get; set; }

        [Required(ErrorMessage = "Risk seviyesi seçiniz.")]
        [Display(Name = "Risk Seviyesi")]
        public RiskLevel RiskLevel { get; set; }

        [Required(ErrorMessage = "Enlem zorunludur.")]
        [Display(Name = "Enlem")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Boylam zorunludur.")]
        [Display(Name = "Boylam")]
        public double Longitude { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;
    }
}