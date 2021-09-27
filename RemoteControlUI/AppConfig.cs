using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoteControl
{
    public static class AppConfig
    {
        private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";
    
        public static string[] Uris { get; private set; } = Array.Empty<string>();

        static AppConfig()
        {
            ReadFile();
        }

        public static void ReadConfig(IEnumerable<string> input)
        {
            Uris = input.Where(x => Uri.TryCreate(x, UriKind.Absolute, out _)).ToArray();
        }

        public static void WriteFile()
        {
            File.WriteAllLines(ConfigPath, Uris);
        }

        public static void ReadFile()
        {
            if (!File.Exists(ConfigPath))
                return;

            ReadConfig(File.ReadAllLines(ConfigPath));
        }

        public static string GetFileConfig()
        {
            if (!File.Exists(ConfigPath))
                return "";

            return File.ReadAllText(ConfigPath);
        }
    }
}