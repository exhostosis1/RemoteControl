using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyLogger
{
    internal class FileWriter : ILogWriter
    {
        private readonly string _fileName;
        private const long maxLength = 100 * 1024 * 1024;

        private static readonly Dictionary<string, FileWriter> Cache = new Dictionary<string, FileWriter>();

        private FileWriter(string fileName)
        {
            _fileName = fileName;

            var file = new FileInfo(fileName);

            if (file.Exists && file.Length >= maxLength)
            {
                file.Delete();
                file.Create();
            }
        }

        internal static ILogWriter GetFileWriter(string fileName)
        {
            var key = fileName.ToLower();

            if (Cache.ContainsKey(key)) return Cache[key];

            var result = new FileWriter(fileName);
            Cache.Add(key, result);
            return result;
        }

        public void WriteData(IEnumerable<string> data)
        {
            File.AppendAllLines(_fileName, data);
        }

        public async void WriteDataAsync(IEnumerable<string> data)
        {
            await Task.Run(() => WriteData(data));
        }
    }
}