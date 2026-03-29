using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AfetYonetim.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Ad")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Soyad")]
        public string Surname { get; set; } = string.Empty;

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Bölge")]
        public Guid? RegionId { get; set; }
        public Region? Region { get; set; }

        [Display(Name = "Enlem")]
        public double? Latitude { get; set; }

        [Display(Name = "Boylam")]
        public double? Longitude { get; set; }

        // Audit alanları (BaseEntity'den türetemediğimiz için manuel)
        public DateTime CreatedAt { get; set; }

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(450)]
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [MaxLength(450)]
        public string? DeletedBy { get; set; }

        // Navigation properties
        public ICollection<HelpRequest> HelpRequests { get; set; } = new List<HelpRequest>();
        public ICollection<Assignment> VolunteerAssignments { get; set; } = new List<Assignment>();
        public ICollection<Assignment> AdminAssignments { get; set; } = new List<Assignment>();
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        // Yardımcı property
        public string FullName => $"{Name} {Surname}";
    }
}