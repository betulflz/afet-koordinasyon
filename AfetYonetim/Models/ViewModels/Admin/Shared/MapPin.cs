namespace AfetYonetim.Models.ViewModels.Admin.Shared
{
    public class MapPin
    {
        public Guid Id { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Urgency { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string DetailUrl { get; set; } = string.Empty;
    }
}