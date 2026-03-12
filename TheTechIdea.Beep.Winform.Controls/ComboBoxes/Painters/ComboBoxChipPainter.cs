using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Standard implementation of <see cref="IComboBoxChipPainter"/>.
    /// Draws multi-select chips with background tints, borders, text labels,
    /// and close (×) buttons. Writes chip-close hit rects back to
    /// <see cref="BeepComboBox.ChipCloseRects"/> for mouse interaction.
    /// </summary>
    internal sealed class ComboBoxChipPainter : IComboBoxChipPainter
    {
        public void PaintChips(Graphics g, BeepComboBox owner, ComboBoxRenderState state, ComboBoxLayoutSnapshot layout)
        {
            if (layout.Chips == null || layout.Chips.Count == 0)
                return;

            var theme = state.ThemeTokens;
            Font font = theme?.LabelFont ?? SystemFonts.DefaultFont;

            // Resolve chip colors from theme tokens
            Color chipBack = theme?.SelectedBackColor != Color.Empty
                ? theme.SelectedBackColor
                : Color.FromArgb(227, 242, 253);
            Color chipFore = theme?.SelectedForeColor != Color.Empty
                ? theme.SelectedForeColor
                : Color.FromArgb(25, 118, 210);
            Color chipBorder = theme?.SelectedBorderColor != Color.Empty
                ? theme.SelectedBorderColor
                : Color.FromArgb(144, 202, 249);

            // Contrast check
            if (ThemeContrastHelper.ContrastRatio(chipFore, chipBack) < 2.5)
            {
                chipFore = ThemeContrastHelper.AdjustForegroundToContrast(chipFore, chipBack, 2.8);
            }

            int radius = layout.CornerRadius > 0 ? Math.Max(layout.CornerRadius - 2, 2) : 4;

            // Clear and rebuild hit-test rects for chip close buttons
            owner.ChipCloseRects.Clear();

            foreach (var chip in layout.Chips)
            {
                var brush = PaintersFactory.GetSolidBrush(chipBack);
                var pen = PaintersFactory.GetPen(chipBorder, 1f);

                using (var path = GetRoundedRectPath(chip.ChipRect, radius))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }

                if (chip.IsOverflow)
                {
                    // Draw "+N" badge centered
                    string text = $"+{chip.OverflowCount}";
                    TextRenderer.DrawText(g, text, font, chip.ChipRect, chipFore,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    continue;
                }

                // Normal chip — compute content area with padding
                Rectangle contentRect = chip.ChipRect;
                int hPad = owner.ScaleLogicalX(6);
                contentRect = new Rectangle(
                    contentRect.X + hPad,
                    contentRect.Y,
                    Math.Max(1, contentRect.Width - hPad * 2),
                    contentRect.Height);

                // Draw close button (×) and register hit rect
                if (!chip.CloseButtonRect.IsEmpty)
                {
                    // Shrink content rect so text doesn't overlap close button
                    contentRect.Width = Math.Max(1, contentRect.Width - chip.CloseButtonRect.Width - owner.ScaleLogicalX(2));

                    using (var iconPath = new GraphicsPath())
                    {
                        iconPath.AddRectangle(chip.CloseButtonRect);
                        StyledImagePainter.PaintWithTint(g, iconPath, SvgsUI.X, chipFore, 0.75f);
                    }

                    // Register hit rect for mouse interaction
                    string key = chip.Item?.GuidId ?? chip.Item?.Text ?? string.Empty;
                    if (!string.IsNullOrEmpty(key))
                    {
                        owner.ChipCloseRects[key] = chip.CloseButtonRect;
                    }
                }

                // Draw chip text
                string label = chip.Item?.Text ?? string.Empty;
                TextRenderer.DrawText(g, label, font, contentRect, chipFore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
