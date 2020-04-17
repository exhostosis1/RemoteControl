using System.Collections.Generic;

namespace HtmlParser
{
    public class Parser
    {
        private readonly string source;
        private int index = 0;

        public bool EoF = false;

        public Parser(string input)
        {
            source = input;
        }

        public void Reset()
        {
            index = 0;
        }
        
        public Node GetNextNode()
        {
            var start = source.IndexOf('<', index);

            if (start == -1)
            {
                return null;
            }

            var end = source.IndexOf('>', start);

            index = end;

            if (source[start + 1] == '/') return GetNextNode();

            var tagString = source.Substring(start, end - start + 1);

            var tagEndIndex = tagString.IndexOf(' ');
            if (tagEndIndex == -1) tagEndIndex = tagString.IndexOf('>');

            var tagName = tagString.Substring(1, tagEndIndex - 1).Trim();
            var attributes = ParseAttributes(tagString.Substring(tagEndIndex));

            var blockEnd = source.IndexOf($"</{tagName}>");

            var innerHtml = blockEnd != -1 ? source.Substring(end + 1, blockEnd - end - 1) : string.Empty;

            if (tagName.ToLower() == "script" || tagName.ToLower() == "style")
                index = blockEnd + 1;

            if (source.IndexOf('<', index) == -1) EoF = true;

            return new Node(tagName, innerHtml, attributes);
        }

        private ICollection<Attribute> ParseAttributes(string input)
        {
            var result = new List<Attribute>();

            var globalIndex = 0;
            var spaceIndex = input.IndexOf(' ');

            while (true)
            {
                var equalIndex = input.IndexOf('=', globalIndex);

                if (equalIndex == -1) break;

                var name = input.Substring(spaceIndex, equalIndex - spaceIndex).Trim();

                var valueStart = input.IndexOf('"', equalIndex);
                var valueEnd = input.IndexOf('"', valueStart + 1);

                globalIndex = valueEnd;
                spaceIndex = input.IndexOf(' ', valueEnd);

                var value = input.Substring(valueStart, valueEnd - valueStart + 1).Trim();

                if (name != string.Empty) result.Add(new Attribute(name, value));
            }

            return result;
        }
    }

    public class Node
    {
        public string Tag { get; }
        public string InnerHtml { get; }
        public IReadOnlyCollection<Attribute> Attributes { get; }

        public Node Parent { get; }

        public IReadOnlyCollection<Node> Children { get; }

        public Node(string tag, string innerHtml, ICollection<Attribute> attributes)
        {
            Tag = tag;
            InnerHtml = innerHtml;
            Attributes = (IReadOnlyCollection<Attribute>)attributes;
        }

        public override string ToString()
        {
            return Tag;
        }
    }

    public class Attribute
    {
        public string Name { get; }
        public string Value { get; }

        public Attribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}
