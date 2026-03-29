using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Entities
{
    public class Assignment : BaseEntity
    {
        [Required]
        [Display(Name = "Yardım Talebi")]
        public Guid HelpRequestId { get; set; }
        public HelpRequest? HelpRequest { get; set; }

        [Required]
        [MaxLength(450)]
        [Display(Name = "Gönüllü")]
        public string VolunteerId { get; set; } = string.Empty;
        public ApplicationUser? Volunteer { get; set; }

        [Required]
        [MaxLength(450)]
        [Display(Name = "Atayan Admin")]
        public string AssignedByAdminId { get; set; } = string.Empty;
        public ApplicationUser? AssignedByAdmin { get; set; }

        [Required]
        [Display(Name = "Atanma Tarihi")]
        public DateTime AssignedDate { get; set; }

        [Display(Name = "Teslim Tarihi")]
        public DateTime? DeliveryDate { get; set; }

        [Required]
        [Display(Name = "Durum")]
        public Enums.AssignmentStatus Status { get; set; } = Enums.AssignmentStatus.Atandi;

        [MaxLength(1000)]
        [Display(Name = "Notlar")]
        public string? Notes { get; set; }
    }
}