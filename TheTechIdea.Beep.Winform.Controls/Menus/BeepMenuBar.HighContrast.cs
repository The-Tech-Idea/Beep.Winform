// BeepMenuBar.HighContrast.cs
// Phase 07 — Accessibility, Keyboard, Mnemonics.
//
// Mirrors BeepListBox.HighContrast.cs exactly. When Windows High Contrast
// mode is on, painters must replace theme colours with SystemColors so
// the user's accessibility palette wins. Subscribes to
// SystemEvents.UserPreferenceChanged so a mid-session HC toggle re-paints
// the bar without needing a theme switch.
//
// Drawing.cs consults IsHighContrast at the top of its per-item paint and
// short-circuits to a HC-specific render path that uses HCItemBackground /
// HCItemForeground / HCBorderColor. The keyboard focus indicator in
// PaintFocusRectIfHC paints a 3px Highlight-coloured ring on the
// currently keyboard-focused top-level item (Phase 07-C).
//
// See .plans/Menus-Phase-07-AccessibilityKeyboard.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        // ── State ─────────────────────────────────────────────────────────────

        private bool _highContrastSubscribed;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Returns <see langword="true"/> when Windows high-contrast mode is active.
        /// Painters read this via <c>this.IsHighContrast</c> before deciding which
        /// colour helpers to use.
        /// </summary>
        public bool IsHighContrast => SystemInformation.HighContrast;

        // ── HC-aware colour helpers (used by Drawing.cs) ─────────────────────

        /// <summary>
        /// Returns the correct background fill for a menubar top-level item.
        /// Returns <see cref="Color.Empty"/> when HC is inactive so the caller
        /// can fall back to the normal chrome pipeline.
        /// </summary>
        public Color HCItemBackground(bool isHovered, bool isSelected)
        {
            if (!IsHighContrast) return Color.Empty;
            if (isSelected) return SystemColors.Highlight;
            if (isHovered)  return SystemColors.HotTrack;
            return SystemColors.Menu;
        }

        /// <summary>
        /// Returns the correct foreground colour for an item label in HC mode.
        /// </summary>
        public Color HCItemForeground(bool isHovered, bool isSelected)
        {
            if (!IsHighContrast) return Color.Empty;
            if (isSelected || isHovered) return SystemColors.HighlightText;
            return SystemColors.MenuText;
        }

        /// <summary>Border/frame colour for HC mode. Empty when HC inactive.</summary>
        public Color HCBorderColor =>
            IsHighContrast ? SystemColors.WindowFrame : Color.Empty;

        /// <summary>
        /// Colour for the 3 px focus ring drawn around the keyboard-focused
        /// top-level item when HC is active.
        /// </summary>
        public Color HCFocusRingColor =>
            IsHighContrast ? SystemColors.Highlight : Color.Empty;

        // ── Subscription lifecycle ─────────────────────────────────────────────

        /// <summary>
        /// Subscribes to <see cref="SystemEvents.UserPreferenceChanged"/> so the
        /// bar re-paints when the user toggles HC mid-session. Called once from
        /// the Popup partial's <see cref="BeepMenuBar.OnHandleCreated(EventArgs)"/>
        /// override (the canonical handle-create hook).
        /// </summary>
        internal void SubscribeHighContrastEvents()
        {
            if (_highContrastSubscribed) return;
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            _highContrastSubscribed = true;
        }

        /// <summary>
        /// Unsubscribes from <see cref="SystemEvents.UserPreferenceChanged"/>.
        /// Called from the Popup partial's
        /// <see cref="BeepMenuBar.OnHandleDestroyed(EventArgs)"/> override.
        /// </summary>
        internal void UnsubscribeHighContrastEvents()
        {
            if (!_highContrastSubscribed) return;
            try { SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged; }
            catch { /* non-fatal */ }
            _highContrastSubscribed = false;
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category != UserPreferenceCategory.Accessibility &&
                e.Category != UserPreferenceCategory.Color) return;

            if (!IsHandleCreated || IsDisposed) return;

            if (InvokeRequired)
            {
                try { BeginInvoke(new Action(Invalidate)); } catch { }
            }
            else
            {
                Invalidate();
            }
        }

        // ── Drawing helpers ──────────────────────────────────────────────────

        /// <summary>
        /// Paints a 3 px focus ring on <paramref name="itemRect"/> using the HC
        /// Highlight colour. No-op when HC is inactive — Drawing.cs uses its
        /// own non-HC focus rectangle path then.
        /// </summary>
        public void PaintFocusRectIfHC(Graphics g, Rectangle itemRect)
        {
            if (!IsHighContrast) return;
            using var pen = new Pen(SystemColors.Highlight, 3f);
            g.DrawRectangle(pen,
                itemRect.X + 1, itemRect.Y + 1,
                itemRect.Width - 3, itemRect.Height - 3);
        }

        /// <summary>
        /// HC-only per-item renderer. Replaces the normal BeepStyling chrome
        /// pipeline with a flat fill + 1 px frame + text using SystemColors.
        /// Called by Drawing.cs when <see cref="IsHighContrast"/> is true.
        /// </summary>
        internal void DrawMenuItemHighContrast(
            Graphics g,
            Models.SimpleItem item,
            Rectangle rect,
            bool isHovered,
            bool isSelected)
        {
            if (item == null) return;

            var back = HCItemBackground(isHovered, isSelected);
            using (var fill = new SolidBrush(back))
            {
                g.FillRectangle(fill, rect);
            }
            using (var pen = new Pen(HCBorderColor, 1f))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            int horizontalPadding = ScaleUi(8);
            int scaledImageSize   = ScaledImageSize;
            int imageAreaWidth    = !string.IsNullOrEmpty(item.ImagePath) ? scaledImageSize + horizontalPadding : 0;
            int textStartX        = rect.X + horizontalPadding + imageAreaWidth;
            int textWidth         = rect.Width - (horizontalPadding * 2) - imageAreaWidth;

            // Note: per Microsoft's HC guidance we skip the image in HC mode
            // unless it's an SVG that can be re-tinted. Beep's images come
            // from arbitrary file paths so we omit them and let the text
            // carry full contrast against the HC background.

            var fore = HCItemForeground(isHovered, isSelected);
            var textRect = new Rectangle(textStartX, rect.Y, textWidth, rect.Height);

            var baseFlags = TextFormatFlags.VerticalCenter
                          | TextFormatFlags.Left
                          | TextFormatFlags.EndEllipsis;

            var flags = _drawMnemonics
                ? baseFlags
                : baseFlags | TextFormatFlags.HidePrefix;

            TextRenderer.DrawText(g, item.Text ?? "", _textFont, textRect, fore, flags);
        }
    }
}
