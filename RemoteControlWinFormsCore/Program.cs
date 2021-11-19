namespace RemoteControlWinFormsCore
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Console.WriteLine("yo");

            Application.Run(new ConfigForm());
        }
    }
}