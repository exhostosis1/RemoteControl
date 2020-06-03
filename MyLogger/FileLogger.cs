using System;
using System.IO;

namespace MyLogger
{
    public class FileLogger : AbstractLogProvider
    {
        private readonly string _fileName;
        private const long MaxLength = 100 * 1024 * 1024;

        public FileLogger(string fileName, Type type) : base(type)
        {
            _fileName = fileName;

            var file = new FileInfo(fileName);

            if (file.Exists && file.Length >= MaxLength)
            {
                file.Delete();
                file.Create();
            }
        }

        internal override void WriteData()
        {
            File.AppendAllLines(_fileName, Data);
            Data.Clear();
        }
    }
}
