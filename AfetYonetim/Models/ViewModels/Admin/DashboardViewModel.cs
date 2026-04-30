using AfetYonetim.Models.ViewModels.Admin.Shared;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class DashboardViewModel
    {
        // Mevcut sayaçlar (Faz 2)
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int AssignedRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int TotalVolunteers { get; set; }
        public int TotalRegions { get; set; }
        public int ActiveAssignments { get; set; }

        // Faz 3 yeni alanlar
        public List<MapPin> ActivePins { get; set; } = new();
        public List<CategoryStat> CategoryStats { get; set; } = new();
        public List<DailyTrend> DailyTrend { get; set; } = new();
        public List<RegionStat> RegionStats { get; set; } = new();
        public List<ActivityItem> RecentActivities { get; set; } = new();
    }
}