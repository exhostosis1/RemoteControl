using System;
using System.Collections.Generic;

using static HtmlParser.Constants;

namespace HtmlParser
{
    public class Node
    {
        public string Tag { 
            get
            {
                return _tag;
            }
            
            set
            {
                _tag = value?.ToLower() ?? "";
            }
        }

        private string _tag;
        public string InnerHtml
        {
            get
            {
                var result = _innerHtml.Split(newLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var childCount = 0;

                for (int i = 0; i < result.Length; i++)
                {
                    if(result[i] == childPlaceholder + childCount)
                    {
                        result[i] = _children[childCount].ToString();
                        childCount++;
                    }
                }

                return String.Join(newLine, result);
            }

            set
            {
                _innerHtml = value ?? "";
            }
        }

        private string _innerHtml;

        public ICollection<Attribute> Attributes { get; set; } = new List<Attribute>();

        public Node Parent { get; private set; }

        public IReadOnlyCollection<Node> Children => _children;

        private readonly List<Node> _children = new List<Node>();

        public Node() : this(null) { }

        public Node(string tag) : this(tag, null) { }

        public Node(string tag, string innerHtml)
        {
            Tag = tag;
            InnerHtml = innerHtml;
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

        public override string ToString() => $"<{Tag} {String.Join(" ", Attributes)}>{InnerHtml ?? ""}</{Tag}>";
    }
}
