using AfetYonetim.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class HelpRequestDetailViewModel
    {
        public HelpRequest Request { get; set; } = null!;
        public List<Assignment> Assignments { get; set; } = new();
        public List<SelectListItem> AvailableVolunteers { get; set; } = new();
    }
}