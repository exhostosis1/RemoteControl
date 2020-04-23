using System;

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

        public static readonly string childPlaceholder = new Random().Next(1000).ToString() + "_CHILD_";

        public enum Mode
        {
            TagName,
            AttributeName,
            AttributeValue,
            InnerHtml,
            Init
        }
    }
}
