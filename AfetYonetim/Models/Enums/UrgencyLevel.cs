using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Enums
{
    public enum UrgencyLevel
    {
        [Display(Name = "Düşük")]
        Dusuk = 1,

        [Display(Name = "Orta")]
        Orta = 2,

        [Display(Name = "Yüksek")]
        Yuksek = 3
    }
}