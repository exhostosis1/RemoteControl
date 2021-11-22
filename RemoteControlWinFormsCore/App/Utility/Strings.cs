using System.Net;

namespace RemoteControl.App.Utility
{
    internal static class Strings
    {
        /// <summary>
        /// parse last 2 params in address string. e.g. "http://host/api/method/methodParam 
        /// </summary>
        internal static (string method, string methodParams) ParseAddresString(string input)
        {
            var str = input.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            return str.Length < 2 ? (string.Empty, string.Empty) : (str[^2], WebUtility.UrlDecode(str[^1]));
        }
    }
}
