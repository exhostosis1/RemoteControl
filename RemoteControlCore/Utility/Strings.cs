using System;
using System.Text;

namespace RemoteControlCore.Utility
{
    internal static class Strings
    {
        internal static bool TryGetString(string input, Encoding encoding, out string output)
        {
            output = "";
            if (string.IsNullOrWhiteSpace(input)) return false;

            if (encoding.Equals(Encoding.UTF8))
            {
                output = input;
                return true;
            }

            try
            {
                output = Encoding.UTF8.GetString(encoding.GetBytes(input));
            }
            catch
            {
                return false;
            }

            return true;
        }

        internal static (string method, string methodParams) ParseAddresString(string input)
        {
            var str = input.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            return str.Length <= 2 ? (null, null) : (str[1], str[2]);
        }
    }
}
