namespace AfetYonetim.Models.ViewModels.Admin
{
    public class UserIndexViewModel
    {
        public List<UserListItem> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class UserListItem
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Eklenen eksik alanlar:
        public List<string> Roles { get; set; } = new();
        public string? RegionName { get; set; }
        
        public bool IsActive { get; set; }
    }
}