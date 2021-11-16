using System;
using System.Net;

namespace RemoteControl.Core.Utility
{
    internal static class Strings
    {
        internal static (string method, string methodParams) ParseAddresString(string input)
        {
            var str = input.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            return str.Length < 2 ? (null, null) : (str[str.Length - 2], WebUtility.UrlDecode(str[str.Length - 1]));
        }
    }
}
