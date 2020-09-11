using HtmlParser;
using System.IO;
using System.Linq;

namespace RemoteControl
{
    public static class Translator
    {
        private const string path = "www\\";
        private const string filename = "index.html";
        private const string resultfilename = "index-simple.html";

        private static readonly string[] Cssfiles = Directory.GetFiles(path, "*.css");

        private const string newLine = "\r\n";

        public static void Translate()
        {
            var input = File.ReadAllText(path + filename);

            var code = File.ReadAllLines(path + "code.js");

            for (var i = 0; i < code.Length; i++)
            {
                if (code[i].Trim().StartsWith("let socketEnabled"))
                    code[i] = $"let socketEnabled = {AppConfig.Socket.ToString().ToLower()};";
                if (code[i].Trim().StartsWith("let host"))
                    code[i] = $"let host = '{AppConfig.ApiHost}';";
                if (code[i].Trim().StartsWith("let port"))
                    code[i] = $"let port = '{AppConfig.ApiPort}';";
            }

            File.WriteAllLines(path + "code.js", code);

            var tree = Parser.GetTree(input);

            var title = tree.FindNodesByTag("title").FirstOrDefault();
            var meta = tree.FindNodesByTag("meta").FirstOrDefault();

            var head = new Node("head");

            head.AddChild(title);
            head.AddChild(meta);

            var newTree = new Tree(new Node("html"));

            newTree.Root.AddChild(head);

            var style = new Node("style");
            style.AddInnerHtml(string.Join(newLine, Cssfiles.ToList().Select(File.ReadAllText)));

            head.AddChild(style);

            var script = Parser.ConcatDependencies(tree.FindNodesByTag("script").FirstOrDefault(), path, new[] { "point", "touch", "text" });

            head.AddChild(script);

            var body = tree.FindNodesByTag("body").FirstOrDefault();

            Tree.RemoveNode(Tree.FindNodeById("loader", body));
            Tree.RemoveNode(Tree.FindNodeById("touch", body));
            Tree.RemoveNode(Tree.FindNodesByTag("input", body).FirstOrDefault());

            newTree.Root.AddChild(body);

            File.WriteAllText(path + resultfilename, newTree.ToString());
        }
    }
}
