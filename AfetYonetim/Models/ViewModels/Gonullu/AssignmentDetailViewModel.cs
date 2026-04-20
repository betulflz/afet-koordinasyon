using AfetYonetim.Models.Entities;

namespace AfetYonetim.Models.ViewModels.Gonullu
{
    public class AssignmentDetailViewModel
    {
        public Assignment Assignment { get; set; } = null!;

        // Talep eden afetzedenin bilgileri
        public string RequesterFullName { get; set; } = string.Empty;
        public string? RequesterPhone { get; set; }
    }
}