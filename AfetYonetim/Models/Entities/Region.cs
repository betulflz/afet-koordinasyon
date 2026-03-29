using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Entities
{
    public class Region : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Bölge Adı")]
        public string RegionName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "İl")]
        public string City { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "İlçe")]
        public string? District { get; set; }

        [Required]
        [Display(Name = "Risk Seviyesi")]
        public Enums.RiskLevel RiskLevel { get; set; }

        [Required]
        [Display(Name = "Enlem")]
        public double Latitude { get; set; }

        [Required]
        [Display(Name = "Boylam")]
        public double Longitude { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<HelpRequest> HelpRequests { get; set; } = new List<HelpRequest>();
       
    }
}