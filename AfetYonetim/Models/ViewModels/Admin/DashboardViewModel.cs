namespace AfetYonetim.Models.ViewModels.Admin
{
    public class DashboardViewModel
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int AssignedRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int TotalVolunteers { get; set; }
        public int TotalRegions { get; set; }
        public int ActiveAssignments { get; set; }
        
    }
}