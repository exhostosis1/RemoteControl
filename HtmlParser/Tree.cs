using System.Collections.Generic;
using System.Linq;
using static HtmlParser.Constants;

namespace HtmlParser
{
    public class Tree
    {
        public Node Root { get; }

        public int Count => CountChildren(Root);

        public IEnumerable<Node> FindNodesByTag(string tag) => FindNodesByTag(tag, Root);

        public IEnumerable<Node> FindNodesByTag(string tag, Node parent) => Find(SearchTypes.Tag, parent, tag, null).ToList();

        public Node FindNodeById(string id) => FindNodeById(id, Root);

        public static Node FindNodeById(string id, Node parent) => Find(SearchTypes.Attribute, parent, id, "id").FirstOrDefault();

        public IReadOnlyCollection<Node> FindNodesByAttribute(string attribute, string value) => FindNodesByAttribute(attribute, value, Root);

        public static IReadOnlyCollection<Node> FindNodesByAttribute(string attribute, string value, Node parent) => Find(SearchTypes.Attribute, parent, value, attribute).ToList();

        public IReadOnlyCollection<Node> FindNodesByDirective(string value) => FindNodesByDirective(value, Root);

        public static IReadOnlyCollection<Node> FindNodesByDirective(string value, Node parent) => Find(SearchTypes.Directive, parent, null, value).ToList();

        public IReadOnlyCollection<Node> FindNodesByClassName(string value) => FindNodesByClassName(value, Root);

        public static IReadOnlyCollection<Node> FindNodesByClassName(string value, Node parent) => Find(SearchTypes.Class, parent, value, null).ToList();

        public Tree(Node node)
        {
            Root = node;
        }

        private static int CountChildren(Node node)
        {
            var result = 1;

            if (node.Children.Count == 0)
                return result;

            result += node.Children.Sum(CountChildren);

            return result;
        }

        private static IEnumerable<Node> Find(SearchTypes type, Node node, string value, string attribute)
        {
            if (Predicate(type, node, value, attribute)) yield return node;

            foreach (var child in node.Children)
                foreach (var childNode in Find(type, child, value, attribute))
                    yield return childNode;
        }

        private static bool Predicate(SearchTypes type, Node node, string value, string attribute)
        {
            string left;

            switch(type)
            {
                case SearchTypes.Tag:
                    left = node.Tag;
                    break;
                case SearchTypes.Class:
                    left = node.Attributes["class"]?.ToLower();
                    break;
                case SearchTypes.Attribute:
                    left = node.Attributes[attribute]?.ToLower();
                    break;
                case SearchTypes.Directive:
                    return node.Attributes.Keys.Contains(attribute);
                default:
                    return false;
            }

            return left == value.ToLower();
        }

        public static void RemoveNode(Node node)
        {
            node.Parent.RemoveChild(node);
        }

        public void AddNode(Node parent, Node child)
        {
            parent.AddChild(child);
        }

        public override string ToString()
        {
            return Root.ToString();
        }
    }
}
