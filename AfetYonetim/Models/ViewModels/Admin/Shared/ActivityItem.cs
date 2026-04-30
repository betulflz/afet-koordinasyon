namespace AfetYonetim.Models.ViewModels.Admin.Shared
{
    public class ActivityItem
    {
        public string Type { get; set; } = string.Empty;       // Create/Approve/Reject/Assign/Deliver
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public DateTime When { get; set; }
        public string IconClass { get; set; } = string.Empty;  // FontAwesome class (ör: fa-plus)
        public string ColorClass { get; set; } = string.Empty; // bg-info / bg-success vb.
    }
}