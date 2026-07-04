using System;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Phase 5.6 / G27 — Best-effort detection of a Windows 11 Focus Session
    /// for the notification system. Win 11's first-class
    /// <c>Windows.UI.Shell.FocusSessionManager</c> requires the Windows App SDK
    /// (we want this code to stay Win-App-SDK-free at runtime); this layer
    /// therefore uses a layered fallback:
    ///
    ///   1. <c>BeepNotificationManager.DoNotDisturbMode</c> — application-level
    ///      flag the host sets when it knows it is in a focus context.
    ///   2. <c>SystemInformation.UserInteractive</c> — false when the session is
    ///      locked (no notifications make sense on a locked workstation).
    ///   3. Conservative default: NOT in focus session.
    ///
    /// If the host enables WinAppSDK later (Phase 8), replace this with a
    /// direct call to <c>FocusSessionManager.GetDefault().IsFocused</c>.
    /// </summary>
    internal static class FocusSessionDetector
    {
        public static bool IsInFocusSession()
        {
            try
            {
                // Layer 2 — locked workstation implies focus session.
                if (!System.Windows.Forms.SystemInformation.UserInteractive)
                    return true;

                // Layer 1 — application-managed DND.
                // Read via reflection to avoid a static dependency cycle with the
                // manager; FieldAccess returns null if the property is missing.
                return FieldAccess.TryGetDoNotDisturbMode() == true;
            }
            catch
            {
                return false;
            }
        }
    }
}
