using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Enums
{
    public enum AuditAction
    {
        [Display(Name = "Talep Oluşturuldu")]
        RequestCreated = 1,

        [Display(Name = "Talep İptal Edildi")]
        RequestCancelled = 2,

        [Display(Name = "Talep Onaylandı")]
        RequestApproved = 3,

        [Display(Name = "Talep Reddedildi")]
        RequestRejected = 4,

        [Display(Name = "Gönüllü Atandı")]
        AssignmentCreated = 5,

        [Display(Name = "Görev Yola Çıktı")]
        AssignmentAccepted = 6,

        [Display(Name = "Görev Teslim Edildi")]
        AssignmentCompleted = 7,

        [Display(Name = "Görev Reddedildi")]
        AssignmentRejected = 8,

        [Display(Name = "Bölge Oluşturuldu")]
        RegionCreated = 9,

        [Display(Name = "Bölge Silindi")]
        RegionDeleted = 10,

        [Display(Name = "Kullanıcı Aktif/Pasif")]
        UserToggledActive = 11
    }
}