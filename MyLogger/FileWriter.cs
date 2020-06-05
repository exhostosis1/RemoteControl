using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyLogger
{
    internal class FileWriter : AbstractLogWriter
    {
        private readonly string _fileName;
        private const long MaxLength = 100 * 1024 * 1024;

        private static readonly Dictionary<string, FileWriter> Cache = new Dictionary<string, FileWriter>();

        private FileWriter(string fileName)
        {
            _fileName = fileName;

            var file = new FileInfo(fileName);

            if (file.Exists && file.Length >= MaxLength)
            {
                file.Delete();
                file.Create();
            }
        }

        internal static AbstractLogWriter GetLogger(string fileName)
        {
            var key = fileName.ToLower();

            if (Cache.ContainsKey(key)) return Cache[key];

            var result = new FileWriter(fileName);
            Cache.Add(key, result);
            return result;
        }

        internal override void WriteData(IEnumerable<string> data)
        {
            File.AppendAllLines(_fileName, data);
        }

        internal override async void WriteDataAsync(IEnumerable<string> data)
        {
            await Task.Run(() => WriteData(data));
        }
    }
}