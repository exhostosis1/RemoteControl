namespace HtmlParser
{
    internal static class Constants
    {
        public const char doublePrtChar = '"';
        public const char singlePrtChar = '\'';
        public const char tagStartChar = '<';
        public const char tagEndChar = '>';
        public const char equalityChar = '=';
        public const char forwardSlashChar = '/';
        public const char backSlashChar = '\\';
        public const string newLine = "\r\n";
        public const string tab = "\t";
        public const string space = " ";

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
