namespace HtmlParser
{
    internal static class Constants
    {
        public const char DoublePrtChar = '"';
        public const char SinglePrtChar = '\'';
        public const char TagStartChar = '<';
        public const char TagEndChar = '>';
        public const char EqualityChar = '=';
        public const char ForwardSlashChar = '/';
        public const char BackSlashChar = '\\';
        public const string NewLine = "\r\n";
        public const string Tab = "\t";
        public const string Space = " ";

        public enum Mode
        {
            TagName,
            AttributeName,
            AttributeValue,
            InnerHtml,
            Init
        }

        public enum SearchTypes
        {
            Tag,
            Class,
            Directive,
            Attribute
        }

        public static string MySubstring(this string input, int startIndex, int endIndex, bool include = false)
        {
            return input.Substring(startIndex + (include ? 0 : 1), endIndex - startIndex + 1 - (include ? 0 : 2));
        }
    }
}
