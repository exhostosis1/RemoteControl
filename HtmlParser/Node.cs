using System.Collections.Generic;
using System.Linq;
using static HtmlParser.Constants;

namespace HtmlParser
{
    public class Node
    {
        public string Tag
        {
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
                var result = string.Join(newLine, _innerHtml.Where(x => !string.IsNullOrWhiteSpace(x.ToString())));
                return result.Trim();
            }
        }

        public void ParseInnerHtml(string input)
        {
            var node = Parser.GetTree(input).Root;

            this._innerHtml.Clear();

            this._innerHtml = node._innerHtml;

            foreach(var child in Children)
            {
                child.Parent = this;
            }
        }

        public void AddInnerHtml(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
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
            foreach (var node in nodes)
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

        private string FormatInnerHtml()
        {
            var result = new List<string>();

            foreach (var entry in _innerHtml)
            {
                var temp = entry.ToString().Split(newLine.ToCharArray()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => tab + x);

                if (temp.Any())
                {
                    result.Add(string.Join(newLine, temp));
                }
            }

            return newLine + string.Join(newLine, result) + newLine;
        }

        public override string ToString()
        {
            var inner = FormatInnerHtml();
            var t = !string.IsNullOrWhiteSpace(Tag);

            var att = Attributes.Count > 0 ? space + string.Join(space, Attributes) : string.Empty;
            var ot = t ? $"<{Tag}{att}>" : string.Empty;
            var ct = t ? $"</{Tag}>" : string.Empty;
            var inn = string.IsNullOrWhiteSpace(inner) ? "" : inner;

            return $"{ot}{inn}{ct}";
        }
    }
}
