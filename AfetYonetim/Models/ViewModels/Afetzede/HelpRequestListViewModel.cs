using AfetYonetim.Models.Enums;

namespace AfetYonetim.Models.ViewModels.Afetzede
{
    public class HelpRequestListViewModel
    {
        public List<HelpRequestListItem> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class HelpRequestListItem
    {
        public Guid Id { get; set; }
        public HelpCategory Category { get; set; }
        public UrgencyLevel Urgency { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Location { get; set; }
    }
}