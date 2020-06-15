﻿using System;
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
            
            return new Tree(nodes[0]);
        }

        public static Node GetNode(string source, int startIndex = 0) => GetNode(source, startIndex, out _, out _);

        private static Node GetNode(string source, int startIndex, out int currentIndex, out bool next)
        {
            currentIndex = startIndex;

            var sb = new StringBuilder();
            var mode = Mode.Init;

            char currentChar;
            char prevChar;

            var currentNode = new Node();
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
                var tag = source.MySubstring(index, source.IndexOf(tagEndChar, index));

                if (!tag.Contains("/"))
                {
                    return false;
                }

                var builder = new StringBuilder();

                var closeTag = false;

                foreach (var t in tag)
                {
                    if (!char.IsWhiteSpace(t))
                    {
                        if (t == forwardSlashChar)
                        {
                            closeTag = true;
                            continue;
                        }
                        if (char.IsLetterOrDigit(t))
                            builder.Append(t);
                    }
                    else
                    {
                        if (builder.Length != 0)
                            break;
                    }
                }

                return closeTag && tagname == builder.ToString().ToLower();
            }

            for (var cursor = startIndex; cursor < source.Length; cursor++)
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

        public static Node ConcatDependencies(Node input, string path, ICollection<string> reject)
        {
            var deps = new List<string>();
            var script = new Node("script");

            script.AddInnerHtml(LocalConcat(input.InnerHtml, path));

            return script;

            string GetPath(string localInput, string localPath)
            {
                var start = localInput.Contains(singlePrtChar) ? localInput.IndexOf(singlePrtChar) : localInput.IndexOf(doublePrtChar);
                var end = localInput.Contains(singlePrtChar) ? localInput.LastIndexOf(singlePrtChar) : localInput.LastIndexOf(doublePrtChar);

                var result = localInput.MySubstring(start, end);

                foreach(var r in reject)
                {
                    if (result.Contains(r))
                        return null;
                }

                return localPath + (localPath.EndsWith("\\") ? "" : "\\") + result;
            }

            string LocalConcat(string localInput, string localPath)
            {

                var strings = localInput.Split(newLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

                for (var i = 0; i < strings.Count; i++)
                {
                    var temp = strings[i].Trim();

                    if (string.IsNullOrEmpty(temp) || temp.StartsWith("export"))
                    {
                        strings[i] = "";
                    }

                    if (!temp.StartsWith("import")) continue;

                    var p = GetPath(temp, localPath);

                    if (p != null && !deps.Contains(p))
                    {
                        deps.Add(p);
                        strings[i] = LocalConcat(File.ReadAllText(p), localPath);
                    }
                    else
                    {
                        strings[i] = "";
                    }
                }

                strings.RemoveAll(x => x == "");

                return string.Join(newLine, strings);
            }
        }
    }
}

