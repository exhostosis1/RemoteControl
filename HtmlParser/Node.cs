using System;
using System.Collections.Generic;

namespace HtmlParser
{
    public class Node
    {
        public string Tag { get; set; }
        public string InnerHtml
        {
            get
            {
                var result = innerHtml;

                for (int i = 0; i < Children.Count; i++)
                {
                    result = result.Replace(Constants.childPlaceholder + i.ToString(), _children[i].ToString());
                }

                return result;
            }

            set
            {
                innerHtml = value;
            }
        }

        private string innerHtml;

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
            return $"<{Tag} {String.Join(" ", Attributes)}>{InnerHtml}</{Tag}>";
        }
    }
}
