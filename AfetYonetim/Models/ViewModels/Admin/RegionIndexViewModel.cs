using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Admin.Shared;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class RegionIndexViewModel
    {
        public List<RegionListItem> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Faz 3 yeni alan — risk haritası için
        public List<RegionMapPin> RiskMapPins { get; set; } = new();
    }

    public class RegionListItem
    {
        public Guid Id { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? District { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public bool IsActive { get; set; }
    }
}