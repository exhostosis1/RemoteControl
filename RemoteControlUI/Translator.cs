using System.IO;
using System.Linq;

namespace RemoteControlTranslator
{
    public static class Translator
    {
        const string path = "www\\";
        const string resultfilename = "index-simple.html";

        static readonly string[] jsfiles = Directory.GetFiles(path, "*.js");
        static readonly string[] cssfiles = Directory.GetFiles(path, "*.css");
        static readonly string[] htmlfiles = Directory.GetFiles(path, "*.html");

        public static void Translate()
        {
            var result = new FileInfo(path + resultfilename);
            var writestream = result.Open(FileMode.Create, FileAccess.Write);

            using (var sr = new StreamWriter(writestream))
            {
                sr.WriteLine("<style>");
                foreach(var file in cssfiles)
                {
                    sr.Write(File.ReadAllText(file));
                    sr.WriteLine();
                }
                sr.WriteLine("</style>");

                sr.WriteLine("<script>");
                foreach (var file in jsfiles)
                {
                    sr.Write(File.ReadAllLines(file).Where(x => !(x.StartsWith("import") || x.StartsWith("export"))).Aggregate((prev, next) => prev + "\n" + next));
                    sr.WriteLine();
                }
                sr.WriteLine("</script>");

                sr.Write(File.ReadAllText(htmlfiles.Single(x => x.Contains("index.html"))));
            }

            writestream.Close();
        }
    }
}
