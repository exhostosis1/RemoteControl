using System;
using System.Windows.Forms;
using RemoteControlTranslator;

namespace RemoteControlUI
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Translator.Translate();
            
            Application.Run(new ConfigForm());
        }
    }
}
