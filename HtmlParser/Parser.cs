using System;
using System.Text;
using System.Collections.Generic;

using static HtmlParser.Constants;

namespace HtmlParser
{
    public class Parser
    {
        private readonly string source;
        private bool next = false;

        public Parser(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("input");

            source = input;
        }

        public Tree GetTree()
        {
            var nodes = new List<Node>();
            var index = 0;

            do
            {
                nodes.Add(GetNode(index, out index));
            }
            while (next);

            if(nodes.Count > 1)
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

        private Node GetNode(int startIndex = 0) => GetNode(startIndex, out _);
        
        private Node GetNode(int startIndex, out int currentIndex)
        {
            currentIndex = startIndex;

            var sb = new StringBuilder();
            var mode = Mode.Before;

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
                else
                {
                    sb.Append(currentChar);
                }
            }

            bool CheckCloseTag(string tagname, int index)
            {
                var tag = source.Substring(index, source.IndexOf(tagEndChar, index) - index + 1);

                 if(!tag.Contains("/"))
                {
                    return false;
                }

                var builder = new StringBuilder();

                var closeTag = false;

                for (int i = 1; i < tag.Length; i++)
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
                
                return closeTag && tagname == builder.ToString();
            }

            for (int cursor = startIndex; cursor < source.Length; cursor++)
            {
                currentIndex = cursor;

                currentChar = source[cursor];
                prevChar = cursor > 0 ? source[cursor - 1] : default;

                switch (mode)
                {
                    case Mode.Before:
                        if (currentChar == tagStartChar)
                        {
                            currentNode.Before = sb.ToString();
                            sb.Clear();
                            mode = Mode.TagName;
                        }
                        else
                        {
                            sb.Append(currentChar);
                        }
                        continue;

                    case Mode.TagName:
                        if (currentChar == tagEndChar)
                        {
                            currentNode.Tag = sb.ToString();
                            
                            sb.Clear();
                            mode = Mode.InnerHtml;
                        }
                        else if (currentChar == forwardSlashChar)
                        {
                            if (sb.Length > 0)
                            {
                                currentNode.Tag = sb.ToString();

                                currentIndex = source.IndexOf(tagEndChar, cursor);

                                return currentNode;
                            }
                            else
                            {
                                cursor = source.IndexOf(tagEndChar, cursor);
                                mode = Mode.Before;
                                continue;
                            }
                        }
                        else if (Char.IsWhiteSpace(currentChar))
                        {
                            if (sb.Length > 0)
                            {
                                currentNode.Tag = sb.ToString();
                                
                                sb.Clear();
                                mode = Mode.AttributeName;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            sb.Append(currentChar);
                        }
                        continue;

                    case Mode.AttributeName:
                        if (char.IsWhiteSpace(currentChar))
                            continue;
                        if (currentChar == tagEndChar || currentChar == forwardSlashChar)
                        {
                            if (sb.Length > 0)
                            {
                                currentNode.Attributes.Add(new Attribute(sb.ToString(), ""));
                                sb.Clear();
                            }

                            if (currentChar == tagEndChar)
                            {
                                mode = Mode.InnerHtml;
                                continue;
                            }
                            else
                            {
                                currentIndex = source.IndexOf(tagEndChar, cursor);

                                return currentNode;
                            }
                        }
                        else if (currentChar == equalityChar)
                        {
                            attributeName = sb.ToString();
                            sb.Clear();
                            mode = Mode.AttributeValue;
                            continue;
                        }
                        else
                            sb.Append(currentChar);
                        continue;

                    case Mode.AttributeValue:
                        if (!str && (char.IsWhiteSpace(currentChar) || currentChar == tagEndChar))
                        {
                            if (sb.Length > 0)
                            {
                                currentNode.Attributes.Add(new Attribute(attributeName, sb.ToString()));
                                sb.Clear();
                                mode = Mode.AttributeName;
                            }
                            if (currentChar == tagEndChar)
                            {
                                mode = Mode.InnerHtml;

                                if (currentNode.Tag.ToLower() == "link" || currentNode.Tag.ToLower() == "meta")
                                    return currentNode;
                            }
                        }
                        else if (!str && currentChar == forwardSlashChar)
                        {
                            currentIndex = source.IndexOf(tagEndChar, cursor);

                            return currentNode;
                        }
                        else if (currentChar == singlePrtChar || currentChar == doublePrtChar)
                        {
                            SetStr();
                        }
                        else
                        {
                            sb.Append(currentChar);
                        }
                        continue;

                    case Mode.InnerHtml:
                        if (currentChar == singlePrtChar || currentChar == doublePrtChar)
                        {
                            SetStr();
                        }
                        else if (!str && currentChar == tagStartChar)
                        {
                            if (CheckCloseTag(currentNode.Tag, cursor))
                            {
                                currentNode.AddInnerNode(sb.ToString());
                                sb.Clear();

                                cursor = source.IndexOf(tagEndChar, cursor);
                                mode = Mode.After;

                                continue;
                            }
                            else if (currentNode.Tag.ToLower() != "script" && currentNode.Tag.ToLower() != "style")
                            {
                                currentNode.AddInnerNode(sb.ToString());
                                sb.Clear();

                                var child = GetNode(cursor, out var newIndex);
                                currentNode.AddChild(child);
                                currentNode.AddInnerNode(child.ToString());

                                cursor = newIndex;
                            }
                            else
                            {
                                sb.Append(currentChar);
                            }
                        }
                        else
                        {
                            sb.Append(currentChar);
                        }
                        continue;

                    case Mode.After:
                        if(currentChar == tagStartChar)
                        {
                            next = true;
                            currentIndex = cursor;
                            break;
                        }
                        else
                        {
                            sb.Append(currentChar);
                        }
                        continue;
                }
            }

            currentNode.After = sb.ToString();

            return currentNode;
        }
    }
}
