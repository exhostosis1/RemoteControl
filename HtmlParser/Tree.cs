using System.Collections.Generic;

namespace HtmlParser
{
    public class Tree
    {
        public Node Root { get; set; }

        public int Count => CountChildren(Root);

        public IEnumerable<Node> FindNodesByTag(string Tag) => FindByTag(Tag.ToLower(), Root);

        public Tree(Node node)
        {
            Root = node;
        }

        private int CountChildren(Node node)
        {
            var result = 1;

            if (node.Children.Count == 0)
                return result;

            foreach (var child in node.Children)
            {
                result += CountChildren(child);
            }

            return result;
        }

        private IEnumerable<Node> FindByTag(string tag, Node node)
        {
            if (node.Tag == tag) yield return node;

            foreach (var child in node.Children)
                foreach (var childNode in FindByTag(tag, child))
                    yield return childNode;
        }
    }
}
