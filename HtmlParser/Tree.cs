namespace HtmlParser
{
    public class Tree
    {
        public Node Root { get; set; }

        public int Count
        {
            get
            {
                return CountChildren(Root);
            }
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

        public Tree(Node node)
        {
            Root = node;
        }
    }
}
