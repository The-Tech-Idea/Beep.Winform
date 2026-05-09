// BeepTabHeaderHost.HighContrast.cs
// High-contrast rendering helpers for BeepTabHeaderHost.
// When the system is running in a Windows high-contrast theme these methods
// replace theme-derived colours with their SystemColors counterparts so that
// the control meets WCAG 2.1 §1.4.11 and Windows high-contrast guidelines.
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        // ── Public query ──────────────────────────────────────────────────────

        /// <summary>
        /// Returns <see langword="true"/> when the system is running in a
        /// high-contrast accessibility theme.  Callers should prefer this property
        /// over <see cref="SystemInformation.HighContrast"/> so the decision is
        /// centralised and testable.
        /// </summary>
        public bool IsHighContrast => SystemInformation.HighContrast;

        // ── Color resolution ─────────────────────────────────────────────────

        /// <summary>
        /// Resolves the background fill colour for a tab body, honouring
        /// high-contrast system colours when required.
        /// </summary>
        public Color GetHighContrastTabBackground(BeepTabItem item)
        {
            if (!IsHighContrast)
            {
                return Color.Transparent;   // let the painter decide in normal mode
            }

            if (item.IsSelected)    return SystemColors.Highlight;
            if (item.IsHovered)     return SystemColors.HotTrack;
            if (item.IsPressed)     return SystemColors.ActiveCaption;

            return SystemColors.ButtonFace;
        }

        /// <summary>
        /// Resolves the foreground (text) colour for a tab body in
        /// high-contrast mode.
        /// </summary>
        public Color GetHighContrastTabForeground(BeepTabItem item)
        {
            if (!IsHighContrast) return SystemColors.ControlText;

            if (item.IsSelected) return SystemColors.HighlightText;
            if (item.IsHovered)  return SystemColors.HighlightText;

            return SystemColors.WindowText;
        }

        /// <summary>
        /// Resolves the border/outline colour for the header strip in
        /// high-contrast mode.
        /// </summary>
        public Color GetHighContrastBorderColor()
        {
            return IsHighContrast ? SystemColors.WindowFrame : SystemColors.ControlDark;
        }

        /// <summary>
        /// Resolves the colour used for the dirty-dot (unsaved) indicator.
        /// High contrast uses <see cref="SystemColors.ControlText"/> so the dot
        /// remains visible on any background.
        /// </summary>
        public Color GetHighContrastDirtyMarkerColor()
        {
            return IsHighContrast ? SystemColors.ControlText : Color.OrangeRed;
        }

        // ── High-contrast OnPaint override ───────────────────────────────────

        /// <summary>
        /// Performs a full high-contrast paint pass for all items in the current
        /// layout snapshot.  Called from <see cref="OnPaint"/> when
        /// <see cref="IsHighContrast"/> is <see langword="true"/>.
        /// This replaces the theme-painter pass; focus rings are still drawn by
        /// <see cref="BeepTabFocusVisualHelper"/> afterwards.
        /// </summary>
        private void PaintHighContrast(System.Drawing.Graphics graphics)
        {
            if (LayoutSnapshot == null || LayoutSnapshot.Items.Count == 0) return;

            // ── Header border ────────────────────────────────────────────────
            using (Pen borderPen = new Pen(GetHighContrastBorderColor(), 1f))
            {
                if (!LayoutSnapshot.HeaderBounds.IsEmpty)
                {
                    graphics.DrawRectangle(borderPen, LayoutSnapshot.HeaderBounds);
                }
            }

            foreach (BeepTabHeaderItemLayout itemLayout in LayoutSnapshot.Items)
            {
                PaintHighContrastTabItem(graphics, itemLayout);
            }
        }

        private void PaintHighContrastTabItem(System.Drawing.Graphics graphics, BeepTabHeaderItemLayout itemLayout)
        {
            BeepTabItem item = itemLayout.Item;

            // ── Tab background ────────────────────────────────────────────────
            Color bgColor = GetHighContrastTabBackground(item);
            if (bgColor != Color.Transparent)
            {
                using SolidBrush bg = new SolidBrush(bgColor);
                graphics.FillRectangle(bg, itemLayout.Bounds);
            }

            // ── Tab border ────────────────────────────────────────────────────
            using (Pen borderPen = new Pen(GetHighContrastBorderColor(), 1f))
            {
                graphics.DrawRectangle(borderPen, itemLayout.Bounds);
            }

            // ── Title text ────────────────────────────────────────────────────
            Color fgColor = GetHighContrastTabForeground(item);
            if (!itemLayout.TextBounds.IsEmpty && !string.IsNullOrEmpty(item.Title))
            {
                TextRenderer.DrawText(
                    graphics,
                    item.Title,
                    Font,
                    itemLayout.TextBounds,
                    fgColor,
                    TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }

            // ── Dirty marker ─────────────────────────────────────────────────
            if (item.IsDirty && !itemLayout.DirtyMarkerBounds.IsEmpty)
            {
                using SolidBrush dirtyBrush = new SolidBrush(GetHighContrastDirtyMarkerColor());
                graphics.FillEllipse(dirtyBrush, itemLayout.DirtyMarkerBounds);
            }

            // ── Close button ─────────────────────────────────────────────────
            if (itemLayout.HasCloseButton && !itemLayout.CloseButtonBounds.IsEmpty)
            {
                using Pen closePen = new Pen(fgColor, 1.5f);
                Rectangle r = itemLayout.CloseButtonBounds;
                // Draw × glyph.
                graphics.DrawLine(closePen, r.Left + 3, r.Top + 3, r.Right - 3, r.Bottom - 3);
                graphics.DrawLine(closePen, r.Right - 3, r.Top + 3, r.Left + 3, r.Bottom - 3);
            }
        }
    }
}
