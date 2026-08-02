using System;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Reports a failure the filter absorbed rather than propagated.
    /// </summary>
    /// <remarks>
    /// A control cannot throw from its paint path without tearing down the host form's paint cycle,
    /// so some failures must be absorbed. They must not be invisible: the two paint guards here
    /// previously logged through <c>Debug.WriteLine</c>, which the compiler strips from Release
    /// builds, so in a shipped application a failing filter drew nothing and left no trace of why.
    /// </remarks>
    public sealed class FilterErrorEventArgs : EventArgs
    {
        public FilterErrorEventArgs(string context, Exception exception)
        {
            Context = context;
            Exception = exception;
        }

        /// <summary>What the filter was doing — for example <c>"OnPaint"</c>.</summary>
        public string Context { get; }

        /// <summary>The failure itself.</summary>
        public Exception Exception { get; }

        public override string ToString() => $"{Context}: {Exception}";
    }
}
