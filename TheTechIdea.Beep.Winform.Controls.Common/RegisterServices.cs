
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Winform.Controls.Managers;

namespace TheTechIdea.Beep.Desktop.Common
{
    public  static partial class RegisterBeepWinformServices
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(int awareness);

        // DPI Awareness levels
        private const int PROCESS_DPI_UNAWARE = 0;
        private const int PROCESS_SYSTEM_DPI_AWARE = 1;
        private const int PROCESS_PER_MONITOR_DPI_AWARE = 2;
        public static void SetHighDpiMode()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.SetDefaultFont(new Font(new FontFamily("Microsoft Sans Serif"), 8.25f));

               
                // Try the modern approach first (Windows 8.1+)
                if (Environment.OSVersion.Version >= new Version(6, 3))
                {
                    SetProcessDpiAwareness(PROCESS_PER_MONITOR_DPI_AWARE);
                }
                else if (Environment.OSVersion.Version >= new Version(6, 0))
                {
                    // Fallback for Windows Vista/7/8
                    SetProcessDPIAware();
                }
               

              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DPI awareness setting failed: {ex.Message}");
                // Continue anyway - the manifest should handle it
            }
           
        }
        public static IServiceCollection RegisterDialogManager(this IServiceCollection services)
        {
         
            services.AddSingleton<IDialogManager, DialogManager>();
            return services;
        }
       
    }
}
