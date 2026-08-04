using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// A section of the grid failed to paint.
    /// </summary>
    /// <remarks>
    /// Painting is absorbed rather than allowed to propagate, and that part is deliberate: an
    /// exception escaping a paint handler does not reach the caller. It travels the window procedure
    /// to <c>NativeWindow.Callback</c> and WinForms raises it as an <i>unhandled</i> exception,
    /// which becomes a modal dialog — on a window that is minimized, off-screen, or mid-drag, an
    /// invisible and unclickable one. The application appears to hang.
    /// <para>
    /// What was wrong was absorbing it <b>silently</b>. A failure in one section left that band of
    /// the grid blank with nothing to say why, and the one instance that logged used
    /// <c>Debug.WriteLine</c>, which is compiled out of Release — silent where it matters most.
    /// </para>
    /// </remarks>
    public sealed class GridRenderErrorEventArgs : EventArgs
    {
        public GridRenderErrorEventArgs(string section, Exception exception)
        {
            Section = section ?? string.Empty;
            Exception = exception;
        }

        /// <summary>
        /// Which part of the grid failed — "Toolbar", "ColumnHeaders", "Rows" and so on. Named
        /// rather than numbered so a handler can report something a user can act on.
        /// </summary>
        public string Section { get; }

        /// <summary>The failure. Never null in practice.</summary>
        public Exception Exception { get; }

        public override string ToString()
            => $"{Section}: {Exception?.GetType().Name}: {Exception?.Message}";
    }
}
