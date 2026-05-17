// BeepMenuBar.Lifecycle.cs
// Phase 02 — Partial-Class Split.
//
// Owns resize, parent-attach, thread-marshalling, and disposal. Handle
// create/destroy hooks for the popup subsystem live in BeepMenuBar.Popup.cs.
//
// See .plans/Menus-Phase-02-PartialClassSplit.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RefreshHitAreas();
            Invalidate();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent is Form parentForm)
            {
                parentForm.Resize -= ParentForm_Resize;
                parentForm.Resize += ParentForm_Resize;
            }
        }

        private void ParentForm_Resize(object sender, EventArgs e)
        {
            SafeInvoke(() =>
            {
                RefreshHitAreas();
            });
        }

        /// <summary>
        /// Safely invokes <paramref name="action"/> on the UI thread,
        /// forcing handle creation first if required.
        /// </summary>
        private void SafeInvoke(Action action)
        {
            if (IsDisposed) return;

            if (!IsHandleCreated)
            {
                var forceHandle = this.Handle; // Force handle creation.
            }

            if (InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Phase 01 — guarantee the MenuDismissed unsubscribe runs
                // even when OnHandleDestroyed wasn't reached (designer
                // teardown can skip it).
                EnsureMenuDismissedUnsubscribed();

                // Phase 07 — same belt-and-suspenders unsubscribe for the
                // High Contrast user-preference event.
                UnsubscribeHighContrastEvents();

                if (Parent is Form parentForm)
                {
                    parentForm.Resize -= ParentForm_Resize;
                }

                CloseAllPopups();
            }
            base.Dispose(disposing);
        }
    }
}
