using HtmlParser;
using System.IO;
using System.Linq;

namespace RemoteControlTranslator
{
    public static class Translator
    {
        const string path = "www\\";
        const string filename = "index.html";
        const string resultfilename = "index-simple.html";

        static readonly string[] cssfiles = Directory.GetFiles(path, "*.css");

        const string newLine = "\r\n";

        public static void Translate()
        {
            var input = File.ReadAllText(path + filename);
            var tree = Parser.GetTree(input);

            var title = tree.FindNodesByTag("title").FirstOrDefault();
            var meta = tree.FindNodesByTag("meta").FirstOrDefault();

            var head = new Node("head");

            head.AddChild(title);
            head.AddChild(meta);

            var newTree = new Tree(new Node("html"));

            newTree.Root.AddChild(head);

            var style = new Node("style");
            style.AddInnerHtml(string.Join(newLine, cssfiles.ToList().Select(x => File.ReadAllText(x))));

            head.AddChild(style);

            var script = Parser.ConcatDependencies(tree.FindNodesByTag("script").FirstOrDefault(), path, new[] { "point", "touch" });

            head.AddChild(script);

            var body = tree.FindNodesByTag("body").FirstOrDefault();

            tree.RemoveNode(tree.FindNodeById("loader", body));
            tree.RemoveNode(tree.FindNodeById("touch", body));
            tree.RemoveNode(tree.FindNodesByTag("input", body).FirstOrDefault());

            newTree.Root.AddChild(body);

            File.WriteAllText(path + resultfilename, newTree.ToString());
        }
    }
}
