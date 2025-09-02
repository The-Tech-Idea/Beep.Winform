using System.Windows.Forms;

namespace WinformSampleApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // High DPI: lock to SystemAware to avoid per-monitor rescaling
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            // Initialize defaults (visual styles, default font, etc.)
            ApplicationConfiguration.Initialize();

            // Check for command line arguments to choose which form to show
            Form mainForm;
            if (args.Length > 0 && args[0].ToLower() == "material")
            {
                mainForm = new MaterialDesignTestForm();
            }
            else
            {
                mainForm = new Form1();
            }

            Application.Run(mainForm);
        }
    }
}