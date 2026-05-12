using AfetYonetim.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfetYonetim.Models.ViewModels.Admin
{
    public class AuditLogIndexViewModel
    {
        public List<AuditLogListItem> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Filtreler
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? UserId { get; set; }
        public AuditAction? Action { get; set; }

        // Dropdown verileri
        public List<SelectListItem> UserList { get; set; } = new();
    }

    public class AuditLogListItem
    {
        public Guid Id { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public AuditAction Action { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}