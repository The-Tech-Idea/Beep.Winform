using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// The window a <see cref="BeepNotificationGroup"/> lives in.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="BeepNotificationGroup"/> is a <c>BaseControl</c>, not a form. The manager used to
    /// call <c>group.Show()</c> on it with no parent, which makes Win32 hand back a bare top-level
    /// window: no owner, no shadow, never on top, and none of the chrome a toast gets from
    /// <see cref="BeepiFormPro"/>. That is what appeared next to real notifications as a small
    /// empty box.
    /// </para>
    /// <para>
    /// This host mirrors <see cref="BeepNotification"/>'s window configuration so a group behaves
    /// like the toasts it replaces, and resizes itself whenever the group grows or is expanded.
    /// </para>
    /// </remarks>
    internal sealed class BeepNotificationGroupHost : BeepiFormPro
    {
        private readonly BeepNotificationGroup _group;

        internal BeepNotificationGroupHost(BeepNotificationGroup group)
        {
            _group = group ?? throw new ArgumentNullException(nameof(group));

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            ShowCaptionBar = false;
            FormStyle = BeepThemesManager.CurrentStyle;
            AutoSize = false;
            KeyPreview = true;

            AccessibleRole = AccessibleRole.Grouping;
            AccessibleName = group.GroupTitle ?? "Grouped notifications";

            _group.Dock = DockStyle.Fill;
            Controls.Add(_group);

            // The group changes height when it expands or gains an item; the window has to follow
            // or the extra rows are simply clipped away.
            _group.SizeChanged += (_, __) => SyncToGroup();
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            SyncToGroup();
        }

        internal BeepNotificationGroup Group => _group;

        private void SyncToGroup()
        {
            if (IsDisposed || _group == null) return;

            var target = new Size(Math.Max(_group.Width, _group.MinimumSize.Width),
                                  Math.Max(_group.Height, _group.MinimumSize.Height));
            if (ClientSize != target) ClientSize = target;
        }

        /// <summary>
        /// Shows without stealing focus, exactly as a toast does - a notification must never take
        /// the caret out of whatever the user is typing into.
        /// </summary>
        protected override bool ShowWithoutActivation => true;
    }
}
