using System.Net;

namespace RemoteControl.Core.Utility
{
    internal static class Strings
    {
        internal static (string method, string methodParams) ParseAddresString(string input)
        {
            var str = input.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            return str.Length < 2 ? (string.Empty, string.Empty) : (str[^2], WebUtility.UrlDecode(str[^1]));
        }
    }
}
