using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// High-contrast mode support for <see cref="BeepListBox"/> (Sprint 5 — WCAG 2.2 AA).
    /// Detects system high-contrast mode and provides corrected colours for all painters.
    /// Subscribes to <see cref="SystemEvents.UserPreferenceChanged"/> so the control
    /// reacts immediately when the user switches HC mode at runtime.
    /// </summary>
    public partial class BeepListBox
    {
        // ── State ─────────────────────────────────────────────────────────────

        private bool _highContrastSubscribed;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Returns <see langword="true"/> when Windows high-contrast mode is active.
        /// Painters should call this via <c>owner.IsHighContrast</c> before using theme colours.
        /// </summary>
        public bool IsHighContrast => SystemInformation.HighContrast;

        // ── HC-aware colour helpers (used by painters) ─────────────────────────

        /// <summary>Returns the correct background fill for a hovered item.</summary>
        public Color HCItemBackground(bool isHovered, bool isSelected)
        {
            if (!IsHighContrast)
                return Color.Empty;           // let the painter use its normal logic

            if (isSelected)  return SystemColors.Highlight;
            if (isHovered)   return SystemColors.HotTrack;
            return SystemColors.Window;
        }

        /// <summary>Returns the correct foreground colour for an item in HC mode.</summary>
        public Color HCItemForeground(bool isSelected)
        {
            if (!IsHighContrast) return Color.Empty;
            return isSelected ? SystemColors.HighlightText : SystemColors.WindowText;
        }

        /// <summary>Returns the correct border/frame colour for HC mode.</summary>
        public Color HCBorderColor =>
            IsHighContrast ? SystemColors.WindowFrame : Color.Empty;

        /// <summary>
        /// Returns the colour for the 3 px focus ring in HC mode,
        /// or <see langword="Color.Empty"/> when HC is inactive.
        /// </summary>
        public Color HCFocusRingColor =>
            IsHighContrast ? SystemColors.Highlight : Color.Empty;

        // ── Subscription lifecycle ─────────────────────────────────────────────

        /// <summary>
        /// Hooks into <see cref="SystemEvents.UserPreferenceChanged"/> so the
        /// control invalidates itself when the user toggles high-contrast mode.
        /// Called once from <see cref="BeepListBox"/> Core partial's <c>OnHandleCreated</c>.
        /// </summary>
        internal void SubscribeHighContrastEvents()
        {
            if (_highContrastSubscribed) return;
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            _highContrastSubscribed = true;
        }

        /// <summary>
        /// Unsubscribes from <see cref="SystemEvents.UserPreferenceChanged"/>.
        /// Called from <see cref="Dispose(bool)"/> in BaseControl or Core.
        /// </summary>
        internal void UnsubscribeHighContrastEvents()
        {
            if (!_highContrastSubscribed) return;
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            _highContrastSubscribed = false;
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Accessibility ||
                e.Category == UserPreferenceCategory.Color)
            {
                // Re-evaluate HC state and redraw
                if (IsHandleCreated)
                {
                    if (InvokeRequired)
                        BeginInvoke(new Action(Invalidate));
                    else
                        Invalidate();
                }
            }
        }

        // ── Drawing helpers for painters ──────────────────────────────────────

        /// <summary>
        /// Paints a 3 px high-contrast focus rectangle around the specified row.
        /// Only paints when both HC mode is active and the control has focus.
        /// Painters should call this at the very end of <c>DrawItem</c>.
        /// </summary>
        public void PaintFocusRectIfHC(Graphics g, Rectangle rowRect)
        {
            if (!IsHighContrast || !Focused) return;
            using var pen = new Pen(SystemColors.Highlight, 3f);
            g.DrawRectangle(pen,
                rowRect.X + 1, rowRect.Y + 1,
                rowRect.Width - 3, rowRect.Height - 3);
        }
    }
}
