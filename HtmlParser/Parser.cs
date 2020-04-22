using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlParser
{
    public class Parser
    {
        private readonly string source;

        const char doublePrtChar = '"';
        const char singlePrtChar = '\'';
        const char tagStartChar = '<';
        const char tagEndChar = '>';
        const char equalityChar = '=';
        const char forwardSlashChar = '/';
        const char backSlashChar = '\\';

        private int _cursor = 0;

        private bool EoF = false;

        public Parser(string input)
        {
            source = input ?? throw new ArgumentException("input");
        }

        private enum Mode
        { 
            TagName,
            AttributeName,
            AttributeValue,
            InnerHtml,
            Init
        }

        private readonly Dictionary<string, int> depth = new Dictionary<string, int>();

        public void Reset()
        {
            _cursor = 0;
            EoF = false;
        }
        
        private Node GetNode()
        {
            var sb = new StringBuilder();
            var mode = Mode.Init;

            char currentChar;
            char prevChar;

            Node currentNode = null;

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

            void AddOrIncrease(string tagname)
            {
                if (depth.ContainsKey(tagname))
                    depth[tagname]++;
                else
                    depth.Add(tagname, 1);
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

                var newName = builder.ToString();

                if (closeTag)
                    depth[newName]--;
                else
                    AddOrIncrease(newName);

                return depth[tagname] == 0;
            }

            for (int cursor = _cursor; cursor < source.Length; cursor++)
            {
                if (cursor == source.Length - 1)
                    EoF = true;

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
                            AddOrIncrease(currentNode.Tag);
                            sb.Clear();
                            mode = Mode.InnerHtml;
                            _cursor = cursor + 1;
                        }
                        else if(currentChar == forwardSlashChar)
                        {
                            if(sb.Length > 0)
                            {
                                currentNode = new Node
                                {
                                    Tag = sb.ToString()
                                };
                                _cursor = cursor + 1;
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
                                AddOrIncrease(currentNode.Tag);
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
                        if(currentChar == tagEndChar || currentChar == forwardSlashChar)
                        {
                            if(sb.Length > 0)
                            {
                                currentNode.Attributes.Add(new Attribute(sb.ToString(), ""));
                                sb.Clear();                                
                            }

                            if (currentChar == tagEndChar)
                            {
                                mode = Mode.InnerHtml;
                                _cursor = cursor + 1;
                                continue;
                            }
                            else
                            {
                                _cursor = cursor + 1;
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
                                _cursor = cursor + 1;

                                if (currentNode.Tag.ToLower() == "link" || currentNode.Tag.ToLower() == "meta")
                                    return currentNode;
                            }
                        }
                        else if(!str && currentChar == forwardSlashChar)
                        {
                            _cursor = cursor + 1;
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
                        else if(!str && currentChar == tagStartChar)
                        {
                            if(CheckCloseTag(currentNode.Tag, cursor))
                            {
                                currentNode.InnerHtml = sb.ToString();
                                sb.Clear();
                                var s = currentNode.Tag.ToLower();
                                if(s == "script" || s == "style")
                                {
                                    _cursor = cursor + 1;
                                }
                                return currentNode;
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

            EoF = true;

            return null;
        }

        public IReadOnlyCollection<Node> GetTree()
        {
            var result = new List<Node>();

            while(!EoF)
            {
                try
                {
                    var node = GetNode();
                    if(node != null)
                        result.Add(node);
                }
                catch
                {
                    throw new FormatException($"Wrong syntax at {_cursor}");
                }
            }

            return result;
        }
    }

    public class Node
    {
        public string Tag { get; set; }
        public string InnerHtml { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }
        public ICollection<Attribute> Attributes { get; set; } = new List<Attribute>();

        public Node Parent { get; private set; }

        public IReadOnlyCollection<Node> Children => _children;

        private readonly List<Node> _children = new List<Node>();

        public Node(string tag, string innerHtml, ICollection<Attribute> attributes)
        {
            Tag = tag;
            InnerHtml = innerHtml;
            Attributes = attributes;
        }

        public Node(string tag)
        {
            Tag = tag;
        }

        public Node()
        {

        }

        public void SetParent(Node parent)
        {
            this.Parent = parent;
            parent._children.Add(this);
        }

        public void AddChild(Node child)
        {
            this._children.Add(child);
            child.Parent = this;
        }

        public void RemoveParent()
        {
            this.Parent._children.Remove(this);
            this.Parent = null;
        }

        public void RemoveChild(Node child)
        {
            this._children.Remove(child);
            child.Parent = null;
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
