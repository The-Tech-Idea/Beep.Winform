// BeepMenuBar.Popup.cs
// Phase 01 — Dismissal/Re-open Hot-Fix.
//
// Owns the bookkeeping needed to suppress the immediate "close then
// re-open" echo that a top-level menu exhibits when the very click that
// dismisses the popup is also delivered to BeepMenuBar.OnMouseClick.
//
// Two complementary mechanisms ship here:
//   1. A short dismissal cool-down (kDismissalCoolDownMs) — clicks
//      landing on a menubar item within the window after a popup
//      dismissal are swallowed.
//   2. An "open top-level index" tracker — clicking the SAME item that
//      owns the currently open popup is treated as a toggle-close.
//
// Coordination with ContextMenuManager happens through the new
// MenuDismissed event added in Phase 01. We subscribe in OnHandleCreated
// and unsubscribe in OnHandleDestroyed / Dispose, so there is exactly
// one subscription per live menubar.
//
// See .plans/Menus-Phase-01-DismissalReopenHotFix.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ContextMenus;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        // ─────────────────────────────────────────────────────────────────
        // Cool-down state
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Default suppression window after a popup dismissal. 250 ms is
        /// long enough to absorb the dismissing click but short enough
        /// that an intentional re-open feels instant — same magnitude as
        /// WPF Menu, WinUI MenuBarItem, and Win32's stock menu echo
        /// suppression.
        /// </summary>
        private const int kDismissalCoolDownMsDefault = 250;

        private int _dismissalCoolDownMs = kDismissalCoolDownMsDefault;
        private DateTime _popupDismissedAtUtc = DateTime.MinValue;
        private Point    _popupDismissedAtScreen;
        private int      _openTopLevelIndex = -1;
        private bool     _dismissedSubscribed;
        private int      _suppressedReopenCount;

        /// <summary>
        /// Cool-down window (milliseconds) during which a menubar click is
        /// treated as the dismissal echo and swallowed. Hidden from the
        /// designer because the default is correct for every standard
        /// scenario; exposed for advanced consumers and unit tests.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal int DismissalCoolDownMs
        {
            get => _dismissalCoolDownMs;
            set => _dismissalCoolDownMs = Math.Max(0, value);
        }

        /// <summary>
        /// Index of the top-level menu item whose popup is currently open,
        /// or -1 when no popup is up. Used by the input pipeline to
        /// detect "same-item-click means toggle-close" intent.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal int OpenTopLevelIndex => _openTopLevelIndex;

        /// <summary>
        /// Number of menubar clicks the cool-down guard has swallowed
        /// since the menubar was created. Useful for diagnostics + the
        /// Phase 09 demo readout.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal int SuppressedReopenCount => _suppressedReopenCount;

        // ─────────────────────────────────────────────────────────────────
        // Cool-down evaluation
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// True when a menubar click should be treated as a dismissal echo
        /// (the WM_LBUTTONDOWN that dismissed the popup being delivered to
        /// OnMouseClick) and silently consumed.
        /// </summary>
        internal bool IsInDismissalCoolDown()
        {
            if (_popupDismissedAtUtc == DateTime.MinValue) return false;
            var elapsedMs = (DateTime.UtcNow - _popupDismissedAtUtc).TotalMilliseconds;
            return elapsedMs >= 0 && elapsedMs < _dismissalCoolDownMs;
        }

        // ─────────────────────────────────────────────────────────────────
        // ContextMenuManager subscription
        // ─────────────────────────────────────────────────────────────────

        private void EnsureMenuDismissedSubscribed()
        {
            if (_dismissedSubscribed) return;
            ContextMenuManager.MenuDismissed += OnContextMenuDismissed;
            _dismissedSubscribed = true;
        }

        private void EnsureMenuDismissedUnsubscribed()
        {
            if (!_dismissedSubscribed) return;
            try { ContextMenuManager.MenuDismissed -= OnContextMenuDismissed; }
            catch { /* non-fatal */ }
            _dismissedSubscribed = false;
        }

        private void OnContextMenuDismissed(object sender, MenuDismissedEventArgs e)
        {
            if (e == null) return;
            // Only react to dismissals of menus we ourselves owned.
            if (!ReferenceEquals(e.Owner, this)) return;

            _popupDismissedAtUtc    = e.UtcTimestamp;
            _popupDismissedAtScreen = e.ScreenPoint;

            // Phase 07-E — announce the popup close to AT clients BEFORE
            // we clear the tracker so the event payload still points at
            // the menubar item that owned the closing popup. The correct
            // MSAA event name is SystemMenuPopupEnd (EVENT_SYSTEM_MENUPOPUPEND).
            if (_openTopLevelIndex >= 0)
            {
                RaiseItemAccessibleEvent(AccessibleEvents.SystemMenuPopupEnd, _openTopLevelIndex);
            }

            _openTopLevelIndex      = -1;

            // Phase 04B — the non-blocking handle is now stale; drop our
            // reference so a later CloseNonBlockingPopup() call is a no-op
            // (the handle itself is already idempotent — this also lets
            // HasNonBlockingPopup report accurately for hover-swap).
            _nonBlockingPopupHandle = null;

            // Phase 04A — Activation state machine transition.
            //   ActiveWithPopup -> ActiveNoPopup when the popup
            //   dismissal came from inside the menubar (toggle close,
            //   item pick, Escape inside popup). Code-driven shutdown
            //   (Esc on the menubar, focus loss) routes through
            //   DeactivateKeyboard() instead.
            if (_activation == MenubarActivation.ActiveWithPopup)
            {
                SetActivation(MenubarActivation.ActiveNoPopup);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Diagnostics
        //
        // Records that the cool-down guard swallowed a click. Centralised
        // so the Phase 09 demo readout has a single field to subscribe to.
        // ─────────────────────────────────────────────────────────────────
        internal void NoteSuppressedReopen()
        {
            _suppressedReopenCount++;
            Debug.WriteLine(
                $"BeepMenuBar: swallowed re-open click within {_dismissalCoolDownMs}ms of dismissal");
        }

        // ─────────────────────────────────────────────────────────────────
        // Handle lifecycle hooks
        // ─────────────────────────────────────────────────────────────────

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            EnsureMenuDismissedSubscribed();
            SubscribeHighContrastEvents();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            EnsureMenuDismissedUnsubscribed();
            UnsubscribeHighContrastEvents();
            base.OnHandleDestroyed(e);
        }

        // ─────────────────────────────────────────────────────────────────
        // Popup launch / teardown
        //
        // ShowMenuItemPopup is the single entry-point used by the input
        // pipeline. It records the open top-level index BEFORE handing
        // off to the (blocking) ContextMenuManager.Show via
        // BaseControl.ShowContextMenu — and clears that tracker on a
        // throw. The normal-close path clears the tracker via the
        // MenuDismissed handler in OnContextMenuDismissed above.
        // ─────────────────────────────────────────────────────────────────

        private void ShowMenuItemPopup(Models.SimpleItem item, int index)
        {
            var menuRects = CalculateMenuItemRects();
            if (index >= menuRects.Count) return;

            var buttonRect     = menuRects[index];
            var screenLocation = this.PointToScreen(new Point(buttonRect.Left, buttonRect.Bottom + ScaleUi(2)));

            if (item.Children == null || item.Children.Count == 0) return;

            // Phase 01: tracker bookkeeping so OnMouseClick can detect a
            // same-item toggle and so OnContextMenuDismissed can clear it
            // on the normal close path.
            _openTopLevelIndex = index;

            // Phase 04A — popup now drives activation. Hover-swap in 04B
            // depends on Activation == ActiveWithPopup to gate its swap
            // logic; the toggle path in Input.cs also reads this state.
            SetActivation(MenubarActivation.ActiveWithPopup);

            // Phase 07-E — announce popup open so screen readers move
            // their reading cursor onto the new popup tree. The correct
            // MSAA event is SystemMenuPopupStart (EVENT_SYSTEM_MENUPOPUPSTART).
            RaiseItemAccessibleEvent(AccessibleEvents.SystemMenuPopupStart, index);

            Models.SimpleItem selectedItem;
            try
            {
                selectedItem = base.ShowContextMenu(item.Children.ToList(), screenLocation, multiSelect: false);
            }
            catch
            {
                _openTopLevelIndex = -1;
                // Defensive: if Show throws we never received MenuDismissed
                // either; bring activation back to a clean state.
                if (_activation == MenubarActivation.ActiveWithPopup)
                {
                    SetActivation(MenubarActivation.ActiveNoPopup);
                }
                throw;
            }

            if (selectedItem != null)
            {
                SelectedItem = selectedItem;
                
                if (SelectedItem.MethodName != null)
                {
                   
                    RunMethodFromGlobalFunctions(SelectedItem, SelectedItem.MethodName);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Phase 04B — Non-blocking popup adapter
        //
        // Hover-swap (Phase 04B2) needs the menubar's UI thread to keep
        // pumping OnMouseMove while a popup is up; the blocking Show in
        // ShowMenuItemPopup above prevents that. Phase 05 added
        // ContextMenuManager.ShowNonBlocking returning an IDisposable
        // handle. We retain that handle here so Input.cs can:
        //   * Close the current popup on hover-swap
        //   * Close it on toggle-close
        //   * Defensively close it on Dispose
        // ─────────────────────────────────────────────────────────────────

        private IDisposable _nonBlockingPopupHandle;

        /// <summary>
        /// True while a non-blocking popup launched via
        /// <see cref="ShowMenuItemPopupNonBlocking"/> is currently active.
        /// Designer-hidden; consulted by hover-swap in Input.cs.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool HasNonBlockingPopup => _nonBlockingPopupHandle != null;

        /// <summary>
        /// Non-blocking variant of <see cref="ShowMenuItemPopup"/> used by
        /// the hover-swap path. Returns immediately; the
        /// <c>IDisposable</c> handle stored on the menubar lets us close
        /// or replace the popup as the hover crosses to other items.
        ///
        /// The selected-item callback hands off to the same
        /// <see cref="RunMethodFromGlobalFunctions"/> dispatch that the
        /// blocking path uses, so the user-visible side effect is
        /// identical.
        /// </summary>
        private void ShowMenuItemPopupNonBlocking(Models.SimpleItem item, int index)
        {
            if (item == null) return;
            if (item.Children == null || item.Children.Count == 0) return;

            var menuRects = CalculateMenuItemRects();
            if (index < 0 || index >= menuRects.Count) return;

            var buttonRect     = menuRects[index];
            var screenLocation = this.PointToScreen(new Point(buttonRect.Left, buttonRect.Bottom + ScaleUi(2)));

            // Close any previous non-blocking popup before opening the new
            // one — the manager itself also closes root menus, but
            // disposing our handle clears the local tracker too.
            CloseNonBlockingPopup();

            _openTopLevelIndex = index;
            SetActivation(MenubarActivation.ActiveWithPopup);

            // Phase 07-E — same AT announcement as the blocking path.
            RaiseItemAccessibleEvent(AccessibleEvents.SystemMenuPopupStart, index);

            try
            {
                _nonBlockingPopupHandle = ContextMenuManager.ShowNonBlocking(
                    item.Children.ToList(),
                    screenLocation,
                    owner: this,
                    style: TheTechIdea.Beep.Vis.Modules.FormStyle.Modern,
                    theme: null,
                    onItemSelected: selectedItem =>
                    {
                        // Mirror the synchronous path's post-select dispatch.
                        if (selectedItem == null) return;
                        SelectedItem = selectedItem;
                        //OnMenuItemSelected(selectedItem);
                        if (SelectedItem.MethodName != null)
                        {
                            RunMethodFromGlobalFunctions(SelectedItem, SelectedItem.Text);
                        }
                    });
            }
            catch
            {
                _openTopLevelIndex = -1;
                if (Activation == MenubarActivation.ActiveWithPopup)
                {
                    SetActivation(MenubarActivation.ActiveNoPopup);
                }
                throw;
            }
        }

        /// <summary>
        /// Closes any non-blocking popup currently open via
        /// <see cref="ShowMenuItemPopupNonBlocking"/>. Idempotent.
        /// Activation state is left to the
        /// <see cref="OnContextMenuDismissed"/> handler so the same
        /// transition logic applies for both code-driven and
        /// user-driven dismissal.
        /// </summary>
        internal void CloseNonBlockingPopup()
        {
            var handle = _nonBlockingPopupHandle;
            _nonBlockingPopupHandle = null;
            if (handle == null) return;
            try { handle.Dispose(); }
            catch { /* non-fatal */ }
        }

        /// <summary>
        /// Tears down any non-blocking popup that the menubar still
        /// owns. Called from Lifecycle.Dispose so we never leak a popup
        /// past the menubar's own teardown.
        /// </summary>
        private void CloseAllPopups()
        {
            CloseNonBlockingPopup();
        }
    }
}
