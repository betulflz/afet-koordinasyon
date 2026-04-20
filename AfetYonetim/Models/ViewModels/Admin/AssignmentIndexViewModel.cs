using AfetYonetim.Models.Enums;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class AssignmentIndexViewModel
    {
        public List<AssignmentListItem> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public AssignmentStatus? Status { get; set; }
    }

    public class AssignmentListItem
    {
        public Guid Id { get; set; }
        public string VolunteerFullName { get; set; } = string.Empty;
        public HelpCategory RequestCategory { get; set; }
        public string RequestDescription { get; set; } = string.Empty;
        public AssignmentStatus Status { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}