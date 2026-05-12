using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.Enums;
using AfetYonetim.Services;

namespace AfetYonetim.Extensions
{
    public static class AuditLogExtensions
    {
        public static void AddAuditLog(
            this ApplicationDbContext db,
            ICurrentUserService currentUser,
            AuditAction action,
            string entityName,
            string? entityId = null,
            string? description = null)
        {
            db.AuditLogs.Add(new AuditLog
            {
                UserId = currentUser.UserId ?? "system",
                UserFullNameSnapshot = currentUser.FullName ?? "Sistem",
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Description = description,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}