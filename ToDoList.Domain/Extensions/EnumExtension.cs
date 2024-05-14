using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ToDoList.Domain.Extensions {
    public static class EnumExtension {
        public static string GetDisplayName(this System.Enum enumVal) {
            return enumVal.GetType()
                .GetMember(enumVal.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.GetName() ?? "Неопределенный";
        }
    }
}
