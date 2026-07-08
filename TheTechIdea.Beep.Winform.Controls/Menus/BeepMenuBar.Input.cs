// BeepMenuBar.Input.cs
// Phase 02 — Partial-Class Split.
//
// Owns mouse handling, hit testing, hover tracking, and the
// HandleMenuItemClick dispatcher. Cool-down + toggle semantics live in
// BeepMenuBar.Popup.cs from Phase 01 and are consulted here.
//
// See .plans/Menus-Phase-02-PartialClassSplit.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ContextMenus;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        // Owned here because hover state is purely an input concern.
        private string _hoveredMenuItemName;

        // Phase 04B — hover-swap throttle.
        // Crossing the seam between adjacent top-level items can fire
        // OnMouseMove multiple times in <1 ms; throttling to one swap
        // per 50 ms eliminates jitter without harming responsiveness
        // (50 ms is well below the perceptual lag threshold).
        private const int kHoverSwapThrottleMs = 50;
        private DateTime  _lastHoverSwapUtc = DateTime.MinValue;

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (DesignMode) return;

            Debug.WriteLine($"BeepMenuBar OnMouseClick at {e.Location}");

            Point mousePoint = e.Location;
            var menuRects = CalculateMenuItemRects();

            // Phase 01 — Dismissal/Re-open Hot-Fix:
            // The WM_LBUTTONDOWN that dismissed a popup is also delivered
            // here. If the click lands on a menubar item inside the
            // cool-down window, swallow it — otherwise we'd re-open the
            // popup the user just dismissed.
            bool inCoolDown = IsInDismissalCoolDown();

            for (int i = 0; i < items.Count && i < menuRects.Count; i++)
            {
                var item = items[i];
                var rect = menuRects[i];

                if (!rect.Contains(mousePoint)) continue;

                if (inCoolDown)
                {
                    NoteSuppressedReopen();
                    return;
                }

                // Phase 01 — Toggle semantics: same-item click closes the
                // popup instead of re-opening it (matches VS, Office,
                // DevExpress, WPF Menu).
                // Phase 04A — toggle-close also returns activation to
                // ActiveNoPopup so the next Alt-letter keystroke still
                // targets the menubar without a redundant Alt press.
                if (_openTopLevelIndex == i)
                {
                    Debug.WriteLine($"BeepMenuBar: toggle-close on {item?.Text} (index {i})");
                    try { ContextMenuManager.CloseAllMenus(); }
                    catch { /* non-fatal */ }
                    _openTopLevelIndex = -1;
                    if (Activation == MenubarActivation.ActiveWithPopup)
                    {
                        SetActivation(MenubarActivation.ActiveNoPopup);
                    }
                    return;
                }

                Debug.WriteLine($"Menu item {i} clicked: {item.Text}");

                int previousSelected = _selectedIndex;
                _selectedIndex = i;

                if (previousSelected >= 0 && previousSelected < menuRects.Count && previousSelected != i)
                {
                    InvalidateRegion(menuRects[previousSelected]);
                }
                InvalidateRegion(menuRects[i]);

                HandleMenuItemClick(item, i);
                return;
            }
        }

        private void InvalidateRegion(Rectangle region)
        {
            if (!region.IsEmpty)
            {
                this.Invalidate(region);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode) return;

            string previousHovered = _hoveredMenuItemName;
            _hoveredMenuItemName = null;

            int hoveredIndex = -1;
            var menuRects = CalculateMenuItemRects();
            for (int i = 0; i < menuRects.Count; i++)
            {
                if (!menuRects[i].Contains(e.Location)) continue;

                hoveredIndex = i;
                _hoveredMenuItemName = $"MenuItem_{i}";
                Cursor = Cursors.Hand;

                if (previousHovered != _hoveredMenuItemName)
                {
                    // Invalidate both regions so the hover transition stays smooth.
                    if (!string.IsNullOrEmpty(previousHovered))
                    {
                        int previousIndex = int.Parse(previousHovered.Replace("MenuItem_", ""));
                        if (previousIndex < menuRects.Count)
                            InvalidateRegion(menuRects[previousIndex]);
                    }
                    InvalidateRegion(menuRects[i]);
                }
                break;
            }

            if (_hoveredMenuItemName == null)
            {
                Cursor = Cursors.Default;
                if (!string.IsNullOrEmpty(previousHovered))
                {
                    int previousIndex = int.Parse(previousHovered.Replace("MenuItem_", ""));
                    if (previousIndex < menuRects.Count)
                    {
                        InvalidateRegion(menuRects[previousIndex]);
                    }
                }
            }

            // Phase 04B — hover-swap.
            // When a non-blocking popup is up and the cursor drifts
            // onto a different top-level item that has children, close
            // the current popup and open the new one. No click required
            // — matches VS / Office / WPF Menu behaviour.
            //
            // Hover-swap is gated by:
            //   * Activation == ActiveWithPopup (otherwise there's no
            //     popup to swap)
            //   * HasNonBlockingPopup (the blocking Show path can't be
            //     swapped without re-entering the message pump)
            //   * Hover crossed onto a DIFFERENT item with children
            //   * Throttle window has elapsed (avoids jitter at the seam)
            if (hoveredIndex >= 0
                && Activation == MenubarActivation.ActiveWithPopup
                && HasNonBlockingPopup
                && hoveredIndex != _openTopLevelIndex)
            {
                MaybeHoverSwap(hoveredIndex);
            }
        }

        /// <summary>
        /// Phase 04B — swaps the active non-blocking popup to a different
        /// top-level item. Throttled so seam-crossings don't open three
        /// popups in two frames.
        /// </summary>
        private void MaybeHoverSwap(int targetIndex)
        {
            if (targetIndex < 0 || targetIndex >= items.Count) return;
            var target = items[targetIndex];
            if (target == null) return;
            if (target.Children == null || target.Children.Count == 0) return;

            var nowUtc   = DateTime.UtcNow;
            var elapsed  = (nowUtc - _lastHoverSwapUtc).TotalMilliseconds;
            if (elapsed >= 0 && elapsed < kHoverSwapThrottleMs) return;
            _lastHoverSwapUtc = nowUtc;

            Debug.WriteLine($"BeepMenuBar.HoverSwap: {_openTopLevelIndex} -> {targetIndex} ({target.Text})");
            _selectedIndex = targetIndex;
            try
            {
                ShowMenuItemPopupNonBlocking(target, targetIndex);
            }
            catch
            {
                // Defensive — don't let a swap failure poison the menubar.
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredMenuItemName == null) return;

            string previousHovered = _hoveredMenuItemName;
            _hoveredMenuItemName = null;
            Cursor = Cursors.Default;

            if (!string.IsNullOrEmpty(previousHovered))
            {
                int previousIndex = int.Parse(previousHovered.Replace("MenuItem_", ""));
                var menuRects = CalculateMenuItemRects();
                if (previousIndex < menuRects.Count)
                {
                    InvalidateRegion(menuRects[previousIndex]);
                }
            }
        }

        /// <summary>
        /// Routes a click on a top-level menubar item: open popup when the
        /// item has children; fire its method-binding otherwise.
        ///
        /// Hot-fix 2026-05-17: reverted Phase 04B's switch to the
        /// non-blocking popup adapter. That path opens the popup while
        /// this OnMouseClick handler is still on the call stack — focus
        /// transfer to the popup is unreliable in that window and the
        /// popup vanishes before the user sees it.
        ///
        /// The blocking path (ShowMenuItemPopup → base.ShowContextMenu →
        /// ContextMenuManager.Show + PumpUntilClosed) keeps the popup
        /// pumped and focused for the entire user interaction and is the
        /// always-worked path we shipped pre-Phase-04B.
        ///
        /// Hover-swap (the reason Phase 04B existed) is deferred — it
        /// needs a redesign that opens the popup AFTER the current
        /// message handler returns (BeginInvoke / posted message) rather
        /// than during it. Tracked separately.
        /// </summary>
        private void HandleMenuItemClick(SimpleItem item, int index)
        {
            Debug.WriteLine($"HandleMenuItemClick: Item: {item?.Text}, Index: {index}");
            if (item == null) return;

            if (item.Children.Count > 0)
            {
                Debug.WriteLine($"items.childs {item.Children.Count}");
                ShowMenuItemPopup(item, index);
            }
            else
            {
                currentMenu = items;
                SelectedItem = item;
              

                if (SelectedItem.MethodName != null)
                {
                    RunMethodFromGlobalFunctions(SelectedItem, SelectedItem.MethodName);
                }
            }
        }
    }
}
