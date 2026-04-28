using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AfetYonetim.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString());
            if (member.Length == 0)
                return enumValue.ToString();

            var displayAttr = member[0].GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.Name ?? enumValue.ToString();
        }
    }
}