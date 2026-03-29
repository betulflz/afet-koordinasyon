using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Enums
{
    public enum AssignmentStatus
    {
        [Display(Name = "Atandı")]
        Atandi = 1,

        [Display(Name = "Yolda")]
        Yolda = 2,

        [Display(Name = "Teslim Edildi")]
        TeslimEdildi = 3,

        [Display(Name = "İptal")]
        Iptal = 4
    }
}