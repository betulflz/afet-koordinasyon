using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Entities
{
    public class HelpRequest : BaseEntity
    {
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; } 

        [Required]
        [Display(Name = "Kategori")]
        public Enums.HelpCategory Category { get; set; }

        [Required]
        [MaxLength(1000)]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Konum(Adres)")]
        public string? Location { get; set; }

        [Display(Name = "Enlem")]
        public double Latitude { get; set; }

        [Display(Name = "Boylam")]
        public double Longitude { get; set; }

        [Display(Name = "Bölge")]
        public Guid RegionId { get; set; }
        public Region? Region { get; set; }


        [Required]
        [Display(Name = "Durum")]
        public Enums.RequestStatus Status { get; set; } = Enums.RequestStatus.Bekliyor;

        [Required]
        [Display(Name = "Aciliyet")]
        public Enums.UrgencyLevel Urgency { get; set; } = Enums.UrgencyLevel.Orta;
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    }
}
     