using AfetYonetim.Models.Enums;

namespace AfetYonetim.Models.ViewModels.Gonullu
{
    public class AssignmentListViewModel
    {
        public List<AssignmentListItem> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class AssignmentListItem
    {
        public Guid Id { get; set; }
        public HelpCategory RequestCategory { get; set; }
        public string RequestDescription { get; set; } = string.Empty;
        public string? RequestLocation { get; set; }
        public AssignmentStatus Status { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}