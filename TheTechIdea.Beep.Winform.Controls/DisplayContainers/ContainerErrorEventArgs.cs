using System;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    /// <summary>
    /// Reports a failure the container absorbed rather than propagated.
    /// </summary>
    /// <remarks>
    /// A painting or layout failure inside a control cannot be allowed to tear down the form's paint
    /// cycle, but it must not vanish either. This folder previously held 35 <c>catch</c> blocks, most
    /// of them bare, so a container that failed to draw simply drew wrongly and nothing said so.
    /// Where a failure genuinely has to be absorbed, it is reported here instead.
    /// </remarks>
    public sealed class ContainerErrorEventArgs : EventArgs
    {
        public ContainerErrorEventArgs(string context, Exception exception)
        {
            Context = context;
            Exception = exception;
        }

        /// <summary>What the container was doing — for example <c>"OnPaint"</c>.</summary>
        public string Context { get; }

        /// <summary>The failure itself.</summary>
        public Exception Exception { get; }

        public override string ToString() => $"{Context}: {Exception}";
    }
}
