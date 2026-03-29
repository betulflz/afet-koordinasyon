using System.ComponentModel.DataAnnotations;

namespace AfetYonetim.Models.Enums
{
    public enum HelpCategory
    {
        [Display(Name = "Gıda")]
        Gida = 1,

        [Display(Name = "Su")]
        Su = 2,

        [Display(Name = "İlaç")]
        Ilac = 3,

        [Display(Name = "Çadır")]
        Cadir = 4,

        [Display(Name = "Giysi")]
        Giysi = 5,

        [Display(Name = "Diğer")]
        Diger = 6
    }
}