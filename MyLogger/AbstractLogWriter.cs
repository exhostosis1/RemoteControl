using System.Collections.Generic;

namespace MyLogger
{
    internal abstract class AbstractLogWriter
    {
        internal abstract void WriteData(IEnumerable<string> data);
        internal abstract void WriteDataAsync(IEnumerable<string> data);
    }
}
