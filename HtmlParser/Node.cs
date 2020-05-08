using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SetInnerHtml(string input)
        {
            var node = Parser.GetTree(input).Root;

            this._innerHtml.Clear();

            this._innerHtml = node._innerHtml;
        }

        public void AddInnerHtml(string value)
        {
            _innerHtml.Add(value);
        }

        private List<object> _innerHtml = new List<object>();

        public Attributes Attributes { get; set; } = new Attributes();

        public Node Parent { get; private set; }

        public IReadOnlyCollection<Node> Children => _innerHtml.Where(x => x.GetType() == typeof(Node)).Select(x => (Node)x).ToList();

        public Node() : this("") { }

        public Node(string tag)
        {
            Tag = tag;
        }

        public void SetParent(Node parent)
        {
            this.Parent = parent;
            parent._innerHtml.Add(this);
        }

        public void AddChild(Node child)
        {
            this._innerHtml.Add(child);
            child.Parent = this;
        }

        public void AddChildren(ICollection<Node> nodes)
        {
            this._innerHtml.AddRange(nodes);
            foreach(var node in nodes)
            {
                node.Parent = this;
            }
        }

        public void RemoveParent()
        {
            this.Parent._innerHtml.Remove(this);
            this.Parent = null;
        }

        public void RemoveChild(Node child)
        {
            this._innerHtml.Remove(child);
            child.Parent = null;
        }

        public void RemoveChildren()
        {
            this._innerHtml.RemoveAll(x => x.GetType() == typeof(Node));
        }

        public override string ToString()
        {
            var inn = string.IsNullOrEmpty(InnerHtml) ? "" : newLine + InnerHtml + newLine;
            var att = Attributes.Count > 0 ? " " + string.Join(" ", Attributes) : "";
            var ot = string.IsNullOrEmpty(Tag) ? "" : $"<{Tag}{att}>";
            var ct = string.IsNullOrEmpty(Tag) ? "" : $"</{Tag}>";

            return $"{ot}{inn}{ct}";
        }
    }
}
