using System;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    /// <summary>
    /// Carries a dialog-state persistence failure to whoever is listening.
    /// </summary>
    /// <remarks>
    /// This exists because both halves of <see cref="DialogStateStore"/> used to catch everything
    /// and continue — one of them commented "Silently fail — persistence is non-critical". The
    /// feature is indeed optional, but a read-only state directory meant dialog positions never
    /// persisted for the life of an installation with nothing anywhere recording why.
    /// </remarks>
    public sealed class DialogStateStoreErrorEventArgs : EventArgs
    {
        public DialogStateStoreErrorEventArgs(string context, Exception exception)
        {
            Context = context ?? string.Empty;
            Exception = exception;
        }

        /// <summary>What was being attempted, e.g. "could not write dialog state to '…'".</summary>
        public string Context { get; }

        /// <summary>The failure. Never null.</summary>
        public Exception Exception { get; }
    }
}
