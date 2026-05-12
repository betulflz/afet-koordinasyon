using AfetYonetim.Models.Enums;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class HelpRequestIndexViewModel
    {
        public List<HelpRequestListItem> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Filtreler
        public RequestStatus? Status { get; set; }
        public HelpCategory? Category { get; set; }
        public UrgencyLevel? Urgency { get; set; }
        public string? Search { get; set; }

       
    }

    public class HelpRequestListItem
    {
        public Guid Id { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public HelpCategory Category { get; set; }
        public UrgencyLevel Urgency { get; set; }
        public RequestStatus Status { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}