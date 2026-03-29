using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Enums
{
    public enum RequestStatus
    {
        [Display(Name = "Bekliyor")]
        Bekliyor = 1,

        [Display(Name = "Onaylandı")]
        Onaylandi = 2,

        [Display(Name = "Atandı")]
        Atandi = 3,

        [Display(Name = "Yolda")]
        Yolda = 4,

        [Display(Name = "Teslim Edildi")]
        TeslimEdildi = 5,

        [Display(Name = "Reddedildi")]
        Reddedildi = 6
    }
}