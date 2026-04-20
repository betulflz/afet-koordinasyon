namespace AfetYonetim.Models.ViewModels.Admin
{
    public class AnnouncementIndexViewModel
    {
        public List<AnnouncementListItem> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class AnnouncementListItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsUrgent { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AdminFullName { get; set; } = string.Empty;
    }
}