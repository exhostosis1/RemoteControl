﻿using System.IO;
using System.Linq;
using HtmlParser;

namespace RemoteControl
{
    public static class Translator
    {
        private const string Path = "www\\";
        private const string Filename = "index.html";
        private const string Resultfilename = "index-simple.html";

        private static readonly string[] Cssfiles = Directory.GetFiles(Path, "*.css");

        private const string NewLine = "\r\n";

        public static void Translate()
        {
            var input = File.ReadAllText(Path + Filename);
            var tree = Parser.GetTree(input);

            var title = tree.FindNodesByTag("title").FirstOrDefault();
            var meta = tree.FindNodesByTag("meta").FirstOrDefault();

            var head = new Node("head");

            head.AddChild(title);
            head.AddChild(meta);

            var newTree = new Tree(new Node("html"));

            newTree.Root.AddChild(head);

            var style = new Node("style");
            style.AddInnerHtml(string.Join(NewLine, Cssfiles.ToList().Select(File.ReadAllText)));

            head.AddChild(style);

            var script = Parser.ConcatDependencies(tree.FindNodesByTag("script").FirstOrDefault(), Path, new[] { "point", "touch", "text" });

            head.AddChild(script);

            var body = tree.FindNodesByTag("body").FirstOrDefault();

            Tree.RemoveNode(Tree.FindNodeById("loader", body));
            Tree.RemoveNode(Tree.FindNodeById("touch", body));
            Tree.RemoveNode(Tree.FindNodesByTag("input", body).FirstOrDefault());

            newTree.Root.AddChild(body);

            File.WriteAllText(Path + Resultfilename, newTree.ToString());
        }
    }
}
