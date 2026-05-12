namespace AfetYonetim.Models.ViewModels.Home
{
    public class PublicLandingViewModel
    {
        public int TotalRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int ActiveAssignments { get; set; }
        public int ActiveVolunteers { get; set; }
        public List<PublicMapPoint> MapPoints { get; set; } = new();
        public List<PublicAnnouncement> RecentAnnouncements { get; set; } = new();
    }
}