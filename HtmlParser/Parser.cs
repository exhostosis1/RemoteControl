using System;
using System.Collections.Generic;
using System.Text;

using static HtmlParser.Constants;

namespace HtmlParser
{
    public class Parser
    {
        private readonly string source;

        public Parser(string input)
        {
            source = input ?? throw new ArgumentException("input");
        }

        private Node GetNode(int startIndex, out int currentIndex)
        {
            currentIndex = startIndex;

            var sb = new StringBuilder();
            var mode = Mode.Init;

            char currentChar;
            char prevChar;

            Node currentNode = null;

            var attributeName = "";

            var str = false;
            char strChar = default;

            var childCount = 0;

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
                    case Mode.Init:
                        if (currentChar == tagStartChar)
                        {
                            mode = Mode.TagName;
                        }
                        continue;

                    case Mode.TagName:
                        if (currentChar == tagEndChar)
                        {
                            currentNode = new Node
                            {
                                Tag = sb.ToString()
                            };
                            
                            sb.Clear();
                            mode = Mode.InnerHtml;
                        }
                        else if (currentChar == forwardSlashChar)
                        {
                            if (sb.Length > 0)
                            {
                                currentNode = new Node
                                {
                                    Tag = sb.ToString()
                                };

                                return currentNode;
                            }
                            else
                            {
                                mode = Mode.Init;
                                continue;
                            }
                        }
                        else if (Char.IsWhiteSpace(currentChar))
                        {
                            if (sb.Length > 0)
                            {
                                currentNode = new Node
                                {
                                    Tag = sb.ToString()
                                };
                                
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
                                currentNode.InnerHtml = sb.ToString();
                                sb.Clear();

                                return currentNode;
                            }
                            else if (currentNode.Tag.ToLower() != "script" && currentNode.Tag.ToLower() != "style")
                            {
                                var child = GetNode(cursor, out var newIndex);
                                if (child != null)
                                {
                                    currentNode.AddChild(child);
                                    sb.Append(childPlaceholder + childCount);
                                    childCount++;
                                }

                                cursor = newIndex + 1;
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
                }
            }

            return null;
        }

        private Node GetNode(int startIndex = 0)
        {
            return GetNode(startIndex, out _);
        }

        public Tree GetTree()
        {
            return new Tree(GetNode());
        }
    }
}
