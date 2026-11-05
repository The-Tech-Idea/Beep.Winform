using System;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Single reporting entry point for GridX exceptions. Grid-aware callers
    /// route through <see cref="BeepGridPro.ReportOperationError"/> or
    /// <see cref="BeepGridPro.ReportRenderError"/>; code without a grid uses
    /// the shared <see cref="BeepLog"/> once-per-key channel.
    /// </summary>
    internal static class GridErrorReporter
    {
        public static void Operation(BeepGridPro? grid, string section, Exception exception)
        {
            if (exception == null) return;
            if (grid != null)
            {
                grid.ReportOperationError(section, exception);
                return;
            }

            BeepLog.Failure(section, section, exception);
        }

        public static void Render(BeepGridPro? grid, string section, Exception exception)
        {
            if (exception == null) return;
            if (grid != null)
            {
                grid.ReportRenderError(section, exception);
                return;
            }

            BeepLog.FailureOnce(section, section, section, exception);
        }

        public static void FallbackOnce(string key, object? source, string action, Exception exception)
        {
            if (exception == null) return;
            BeepLog.FallbackOnce(key, source, action, exception);
        }
    }
}
