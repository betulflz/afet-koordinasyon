namespace AfetYonetim.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        string? FullName { get; }
        bool IsAuthenticated { get; }
    }
}