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
        static readonly string[] htmlfiles = Directory.GetFiles(path, "*.html");

        const string newLine = "\r\n";

        public static void Translate()
        {
            var input = File.ReadAllText(htmlfiles.Single(x => x.Contains(filename)));
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

            var script = new Node("script");
            script.AddInnerHtml(Parser.ConcatDependencies(tree.FindNodesByTag("script").FirstOrDefault().InnerHtml, path, new[] { "point", "touch" }));

            head.AddChild(script);

            var body = tree.FindNodesByTag("body").FirstOrDefault();

            var loader = tree.FindNodeById("loader", body);
            var touch = tree.FindNodeById("touch", body);
            var inp = tree.FindNodesByTag("input", body).FirstOrDefault();

            tree.RemoveNode(loader);
            tree.RemoveNode(touch);
            tree.RemoveNode(inp);

            newTree.Root.AddChild(body);

            var result = newTree.ToString().Split(newLine.ToCharArray()).ToList();
            result.RemoveAll(x => string.IsNullOrWhiteSpace(x));

            File.WriteAllText(path + resultfilename, string.Join(newLine, result));
        }
    }
}
