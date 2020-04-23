using System.IO;
using System.Linq;
using HtmlParser;

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
            var result = new FileInfo(path + resultfilename);

            using (var sr = new StreamWriter(result.Open(FileMode.Create, FileAccess.Write)))
            {
                sr.Write(ParseHtml());
            }
        }

        private static string ParseHtml()
        {
            var result = "";

            var parser = new Parser(File.ReadAllText(htmlfiles.Single(x => x.Contains(filename))));

            var tree = parser.GetTree();

            return result;
        }
    }
}
