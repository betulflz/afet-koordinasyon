namespace AfetYonetim.Models.ViewModels.Home
{
    public class PublicAnnouncement
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsUrgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}