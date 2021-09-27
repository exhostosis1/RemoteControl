using RemoteControl.Core;
using System;
using System.Windows.Forms;

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


            var program = new Main(AppConfig.Uris.Length == 0);
            
            Application.Run(new ConfigForm(program));
        }
    }
}
