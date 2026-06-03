using System;
using System.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>Shared helpers used across the docking system.</summary>
    internal static class DockingHelpers
    {
        /// <summary>True when running inside the Visual Studio designer process.</summary>
        internal static bool IsWinFormsDesignerProcess()
        {
            try
            {
                string processName = Process.GetCurrentProcess().ProcessName;
                return processName.IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0 ||
                       string.Equals(processName, "devenv", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
