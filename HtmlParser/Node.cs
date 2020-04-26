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
                return String.Join(newLine, _innerHtml);
            }
        }

        private List<string> _innerHtml = new List<string>();

        public void AddInnerNode(string node)
        {
            if(!string.IsNullOrEmpty(node))
                _innerHtml.Add(node);
        }

        public ICollection<Attribute> Attributes { get; set; } = new List<Attribute>();

        public Node Parent { get; private set; }

        public IReadOnlyCollection<Node> Children => _children;

        private readonly List<Node> _children = new List<Node>();

        public string Before { get; set; } = "";
        public string After { get; set; } = "";

        public Node() : this("") { }

        public Node(string tag)
        {
            Tag = tag;
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
            var bf = string.IsNullOrEmpty(Before) ? "" : Before + newLine;
            var af = string.IsNullOrEmpty(After) ? "" : newLine + After;
            var inn = string.IsNullOrEmpty(InnerHtml) ? "" : newLine + InnerHtml + newLine;
            var att = Attributes.Count > 0 ? " " + string.Join(" ", Attributes) : "";
            var ot = string.IsNullOrEmpty(Tag) ? "" : $"<{Tag}{att}>";
            var ct = string.IsNullOrEmpty(Tag) ? "" : $"</{Tag}>";

            return $"{bf}{ot}{inn}{ct}{af}";
        }
    }
}
