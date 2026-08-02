using System;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Reports a failure the docking manager absorbed rather than propagated.
    /// </summary>
    /// <remarks>
    /// Restoring a saved layout is deliberately best-effort: a panel that can no longer be floated
    /// or auto-hidden must not abort the whole restore and leave the user with nothing. But
    /// absorbing it silently is what produces "it forgot my windows again" — the layout comes back
    /// subtly wrong and nothing anywhere says which part failed or why.
    /// </remarks>
    public sealed class DockingErrorEventArgs : EventArgs
    {
        public DockingErrorEventArgs(string context, string panelKey, Exception exception)
        {
            Context = context;
            PanelKey = panelKey;
            Exception = exception;
        }

        /// <summary>What the manager was doing — for example <c>"RestoreLayout.Float"</c>.</summary>
        public string Context { get; }

        /// <summary>The panel involved, where one is known.</summary>
        public string PanelKey { get; }

        /// <summary>The failure itself.</summary>
        public Exception Exception { get; }

        public override string ToString()
            => string.IsNullOrEmpty(PanelKey)
                ? $"{Context}: {Exception}"
                : $"{Context} [{PanelKey}]: {Exception}";
    }
}
