using HtmlParser;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoteControlTranslator
{
    public static class Translator
    {
        const string path = "www\\";
        const string filename = "index.html";
        const string resultfilename = "index-simple.html";

        static readonly string[] jsfiles = Directory.GetFiles(path, "*.js");
        static readonly string[] cssfiles = Directory.GetFiles(path, "*.css");
        static readonly string[] htmlfiles = Directory.GetFiles(path, "*.html");

        const string newLine = "\r\n";

        public static void Translate()
        {
            var input = File.ReadAllText(htmlfiles.Single(x => x.Contains(filename)));
            var tree = Parser.GetTree(input);

            var newTree = new Tree(new Node("html"));

            var head = tree.FindNodesByTag("head").First();

            var scriptAndStyle = Enumerable.Concat(tree.FindNodesByTag("script"), tree.FindNodesByTag("style")).ToList();

            foreach(var node in scriptAndStyle)
            {
                head.RemoveChild(node);
            }

            var script = new Node("script");

            var scriptLines = new List<string>();
            scriptLines.AddRange(File.ReadAllLines(jsfiles.First(x => x.Contains("constants"))));
            scriptLines.AddRange(File.ReadAllLines(jsfiles.First(x => x.Contains("slider"))));
            scriptLines.AddRange(File.ReadAllLines(jsfiles.First(x => x.Contains("keys"))));
            scriptLines.AddRange(File.ReadAllLines(jsfiles.First(x => x.Contains("touch"))));
            scriptLines.AddRange(File.ReadAllLines(jsfiles.First(x => x.Contains("code"))));

            scriptLines.RemoveAll(x => x.Trim().StartsWith("module") || x.Trim().StartsWith("import") || x.Trim().StartsWith("export"));

            script.AddInnerHtml(string.Join(newLine, scriptLines));

            var style = new Node("style");

            style.AddInnerHtml(File.ReadAllText(cssfiles.First()));

            head.AddChild(style);
            head.AddChild(script);

            newTree.Root.AddChild(head);

            var body = tree.FindNodesByTag("body").First();

            var divs = tree.FindNodesByTag("div").Where(x => x.Attributes["id"] == "loader" || x.Attributes["id"] == "touch").ToList();

            foreach(var div in divs)
            {
                tree.RemoveNode(div);
            }

            var inputNode = tree.FindNodesByTag("input").First();
            tree.RemoveNode(inputNode);

            newTree.Root.AddChild(body);

            var result = string.Join(newLine, newTree.ToString().Split(newLine.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x)));

            File.WriteAllText(path + resultfilename, result);
        }
    }
}
