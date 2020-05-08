using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static HtmlParser.Constants;

namespace HtmlParser
{
    public static class Parser
    {
        public static Tree GetTree(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("input");

            var nodes = new List<Node>();
            var index = 0;

            bool next;

            do
            {
                nodes.Add(GetNode(input, index, out index, out next));
            }
            while (next);

            if (nodes.Count > 1)
            {
                var parentNode = new Node();
                foreach (var node in nodes)
                    parentNode.AddChild(node);

                return new Tree(parentNode);
            }
            else
            {
                return new Tree(nodes[0]);
            }
        }

        public static Node GetNode(string source, int startIndex = 0) => GetNode(source, startIndex, out _, out _);

        internal static Node GetNode(string source, int startIndex, out int currentIndex, out bool next)
        {
            currentIndex = startIndex;

            var sb = new StringBuilder();
            var mode = Mode.Init;

            char currentChar;
            char prevChar;

            Node currentNode = new Node();
            next = false;

            var attributeName = "";

            var str = false;
            char strChar = default;

            void SetStr()
            {
                if (!str)
                {
                    str = true;
                    strChar = currentChar;
                }
                else if (currentChar == strChar && prevChar != backSlashChar)
                {
                    str = false;
                }
            }

            bool CheckCloseTag(string tagname, int index)
            {
                var tag = source.MySubstring(index, source.IndexOf(tagEndChar, index), false);

                if (!tag.Contains("/"))
                {
                    return false;
                }

                var builder = new StringBuilder();

                var closeTag = false;

                for (int i = 0; i < tag.Length; i++)
                {
                    if (!Char.IsWhiteSpace(tag[i]))
                    {
                        if (tag[i] == forwardSlashChar)
                        {
                            closeTag = true;
                            continue;
                        }
                        if (Char.IsLetterOrDigit(tag[i]))
                            builder.Append(tag[i]);
                    }
                    else
                    {
                        if (builder.Length == 0)
                            continue;
                        else
                            break;
                    }
                }

                return closeTag && tagname == builder.ToString().ToLower();
            }

            for (int cursor = startIndex; cursor < source.Length; cursor++)
            {
                currentIndex = cursor;

                currentChar = source[cursor];
                prevChar = cursor > 0 ? source[cursor - 1] : default;

                switch (mode)
                {
                    case Mode.Init:
                        switch (currentChar)
                        {
                            case tagStartChar:
                                mode = Mode.TagName;
                                continue;

                            case var c when !char.IsWhiteSpace(c):
                                mode = Mode.InnerHtml;
                                cursor--;
                                continue;

                            default:
                                continue;
                        }

                    case Mode.TagName:
                        switch (currentChar)
                        {
                            case tagEndChar:
                                currentNode.Tag = sb.ToString();
                                sb.Clear();

                                cursor = source.IndexOf(tagEndChar, cursor);
                                mode = Mode.InnerHtml;

                                continue;

                            case forwardSlashChar:
                                if (sb.Length > 0)
                                {
                                    currentNode.Tag = sb.ToString();
                                    sb.Clear();

                                    currentIndex = source.IndexOf(tagEndChar, cursor);
                                    next = source.IndexOf(tagStartChar, currentIndex) > -1;

                                    return currentNode;
                                }
                                else
                                {
                                    cursor = source.IndexOf(tagEndChar, cursor);
                                    mode = Mode.Init;
                                    continue;
                                }

                            case var c when char.IsWhiteSpace(c):
                                if (sb.Length > 0)
                                {
                                    currentNode.Tag = sb.ToString();
                                    sb.Clear();

                                    mode = Mode.AttributeName;

                                    continue;
                                }
                                else
                                {
                                    continue;
                                }

                            default:
                                sb.Append(currentChar);

                                continue;
                        }

                    case Mode.AttributeName:
                        switch (currentChar)
                        {
                            case var c when char.IsWhiteSpace(c):
                                continue;

                            case equalityChar:
                                attributeName = sb.ToString();
                                sb.Clear();
                                mode = Mode.AttributeValue;

                                continue;

                            case forwardSlashChar:
                                if (sb.Length > 0)
                                {
                                    currentNode.Attributes.Add(new Attribute(sb.ToString()));
                                    sb.Clear();
                                }

                                currentIndex = source.IndexOf(tagEndChar, cursor);
                                next = source.IndexOf(tagStartChar, currentIndex) > -1;

                                return currentNode;

                            case tagEndChar:
                                if (sb.Length > 0)
                                {
                                    currentNode.Attributes.Add(new Attribute(sb.ToString()));
                                    sb.Clear();
                                }

                                if (currentNode.Tag == "link" || currentNode.Tag == "meta")
                                {
                                    next = source.IndexOf(tagStartChar, cursor) > -1;
                                    currentIndex = cursor;

                                    return currentNode;
                                }

                                cursor = source.IndexOf(tagEndChar, cursor);
                                mode = Mode.InnerHtml;

                                continue;

                            default:
                                sb.Append(currentChar);
                                continue;
                        }

                    case Mode.AttributeValue:
                        switch (currentChar)
                        {
                            case var c when c == singlePrtChar || c == doublePrtChar:
                                SetStr();
                                continue;

                            case var c when !str && char.IsWhiteSpace(c):
                                if (sb.Length > 0)
                                {
                                    currentNode.Attributes.Add(new Attribute(attributeName, sb.ToString()));
                                    sb.Clear();

                                    mode = Mode.AttributeName;
                                }

                                continue;

                            case var c when !str && c == forwardSlashChar:
                                currentNode.Attributes.Add(new Attribute(attributeName, sb.ToString()));
                                sb.Clear();

                                currentIndex = source.IndexOf(tagEndChar, cursor);
                                next = source.IndexOf(tagStartChar, currentIndex) > -1;

                                return currentNode;

                            case var c when !str && c == tagEndChar:
                                currentNode.Attributes.Add(new Attribute(attributeName, sb.ToString()));
                                sb.Clear();

                                if (currentNode.Tag == "link" || currentNode.Tag == "meta")
                                {
                                    currentIndex = source.IndexOf(tagEndChar, cursor);
                                    next = source.IndexOf(tagStartChar, currentIndex) > -1;

                                    return currentNode;
                                }

                                mode = Mode.InnerHtml;

                                continue;

                            default:
                                sb.Append(currentChar);
                                continue;
                        }

                    case Mode.InnerHtml:
                        switch (currentChar)
                        {
                            case var c when c == singlePrtChar || c == doublePrtChar:
                                SetStr();
                                sb.Append(currentChar);
                                continue;

                            case var c when !str && c == tagStartChar:
                                if (CheckCloseTag(currentNode.Tag, cursor))
                                {
                                    currentNode.AddInnerHtml(sb.ToString());
                                    sb.Clear();

                                    currentIndex = source.IndexOf(tagEndChar, cursor);
                                    next = source.IndexOf(tagStartChar, currentIndex) > -1;

                                    return currentNode;
                                }
                                else if (currentNode.Tag != "script" && currentNode.Tag != "style")
                                {
                                    currentNode.AddInnerHtml(sb.ToString());
                                    sb.Clear();

                                    var child = GetNode(source, cursor, out var newIndex, out _);
                                    currentNode.AddChild(child);

                                    cursor = newIndex;

                                    continue;
                                }
                                else
                                {
                                    sb.Append(currentChar);
                                    continue;
                                }

                            case var c when !str && char.IsWhiteSpace(c):
                                continue;
                            default:
                                sb.Append(currentChar);
                                continue;
                        }
                }
            }

            return currentNode;
        }

        public static Node ConcatDependencies(Node Input, string Path, IEnumerable<string> Reject)
        {
            var deps = new List<string>();
            var script = new Node("script");

            script.AddInnerHtml(LocalConcat(Input.InnerHtml, Path));

            return script;

            string GetPath(string input, string path)
            {
                var start = input.Contains(singlePrtChar) ? input.IndexOf(singlePrtChar) : input.IndexOf(doublePrtChar);
                var end = input.Contains(singlePrtChar) ? input.LastIndexOf(singlePrtChar) : input.LastIndexOf(doublePrtChar);

                var result = input.MySubstring(start, end);

                foreach(var r in Reject)
                {
                    if (result.Contains(r))
                        return null;
                }

                return path + (path.EndsWith("\\") ? "" : "\\") + result;
            }

            string LocalConcat(string input, string path)
            {

                var strings = input.Split(newLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                for (int i = 0; i < strings.Count; i++)
                {
                    var temp = strings[i].Trim();

                    if (string.IsNullOrEmpty(temp) || temp.StartsWith("export"))
                    {
                        strings[i] = "";
                    }

                    if (temp.StartsWith("import"))
                    {
                        var p = GetPath(temp, path);

                        if (p != null && !deps.Contains(p))
                        {
                            deps.Add(p);
                            strings[i] = LocalConcat(File.ReadAllText(p), path);
                        }
                        else
                        {
                            strings[i] = "";
                        }
                    }
                }

                strings.RemoveAll(x => x == "");

                return string.Join(newLine, strings);
            }
        }
    }
}

