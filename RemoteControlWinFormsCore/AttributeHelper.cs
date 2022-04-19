using System.ComponentModel;
using System.Reflection;

namespace RemoteControl
{
    internal static class AttributeHelper
    {
        internal static string? GetDisplayName(this Type type)
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }

        internal static string? GetDisplayName(this PropertyInfo prop)
        {
            return prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }
    }
}
