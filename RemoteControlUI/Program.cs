using System;
using System.Windows.Forms;
using MyLogger;

namespace RemoteControl
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Translator.Translate();

            Application.ApplicationExit += Logger.Flush;
            Application.ThreadException += Logger.Flush;
            
            Application.Run(new ConfigForm());
        }
    }
}
