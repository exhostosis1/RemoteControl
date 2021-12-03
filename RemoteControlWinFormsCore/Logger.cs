namespace RemoteControl
{
    internal enum ErrorLevel
    {
        Notify = 0,
        Warn = 1,
        Debug = 2,
        Error = 3,
        Critical = 4
    }

    internal interface ILogger
    {
        void Log(ErrorLevel errorLevel, string message);
        void Flush();
    }

    internal abstract class Logger
    {
        public static ErrorLevel ErrorLevel = ErrorLevel.Debug;
        public static ILogger GetFileLogger(Type type)
        {
            return new FileLogger(type);
        }

        public static ILogger GetDefaultLogger(Type type) => GetFileLogger(type);        
    }

    internal class FileLogger : ILogger, IDisposable
    {
        private readonly FileInfo _file = new(AppContext.BaseDirectory + "error.log");
        private readonly Type _type;
        private readonly List<(ErrorLevel, string)> _strings = new(100);

        public FileLogger(Type type)
        {
            _type = type;
        }

        public void Log(ErrorLevel errorLevel, string message)
        {
            if (_strings.Count == 100) Flush();


            _strings.Add((errorLevel, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss:ffff} - {errorLevel} - {_type.Name} - {message}"));
        }

        public void Flush()
        {
            using var sw = _file.AppendText();
            foreach (var s in _strings.Where(x => x.Item1 >= Logger.ErrorLevel).Select(x => x.Item2))
            {
                sw.WriteLine(s);
            }
            sw.Flush();
            sw.Close();
            _strings.Clear();
        }

        public void Dispose() => Flush();

        ~FileLogger()
        {
            Flush();
        }
    }
}

