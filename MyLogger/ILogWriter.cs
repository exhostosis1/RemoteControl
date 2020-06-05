using System.Collections.Generic;

namespace MyLogger
{
    public interface ILogWriter
    {
        void WriteData(IEnumerable<string> data);
        void WriteDataAsync(IEnumerable<string> data);
    }
}
