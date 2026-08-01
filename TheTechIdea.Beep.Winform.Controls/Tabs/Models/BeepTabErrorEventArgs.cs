using System;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Carries a failed <see cref="BeepTabs"/> operation to the host.
    /// </summary>
    /// <remarks>
    /// This exists because <c>Debug.WriteLine</c> is compiled out of Release builds: without it, a
    /// tab operation that failed in a shipped application told the host nothing at all. It is a
    /// diagnostic channel, not error handling — the failing operation still throws.
    /// </remarks>
    public sealed class BeepTabErrorEventArgs : EventArgs
    {
        public BeepTabErrorEventArgs(string context, Exception? exception)
        {
            Context = context ?? string.Empty;
            Exception = exception;
        }

        /// <summary>The operation that failed, e.g. <c>"AddPage failed for page 'Reports'."</c></summary>
        public string Context { get; }

        /// <summary>The exception that caused the failure, or <see langword="null"/>.</summary>
        public Exception? Exception { get; }

        /// <summary>Context and exception combined — the same text painted on the control surface.</summary>
        public string Message => Exception == null
            ? Context
            : $"{Context}{Environment.NewLine}{Exception.GetType().Name}: {Exception.Message}";
    }
}
