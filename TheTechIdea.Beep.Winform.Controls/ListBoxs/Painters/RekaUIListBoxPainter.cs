using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Reka UI-inspired listbox painter with accessible, minimal design
    /// Features: Clear focus states, keyboard navigation indicators, ARIA-compliant styling
    /// Based on: https://reka-ui.com/docs/components/listbox
    /// </summary>
    internal class RekaUIListBoxPainter : BaseListBoxPainter
    {
        protected override void DrawItem(Graphics g, Rectangle itemBounds, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || item == null || itemBounds.Width <= 0 || itemBounds.Height <= 0)
                return;

            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // Minimal padding
                var contentBounds = new Rectangle(
                    itemBounds.X + Scale(4),
                    itemBounds.Y + Scale(2),
                    itemBounds.Width - Scale(8),
                    itemBounds.Height - Scale(4)
                );

                // STEP 1: Draw item background
                DrawItemBackgroundEx(g, itemBounds, item, isHovered, isSelected);

                // STEP 2: Draw left accent bar on selected items
                if (isSelected)
                {
                    var accentRect = new Rectangle(contentBounds.X, contentBounds.Y, Scale(3), contentBounds.Height);
                    g.FillRectangle(GetBrush(_theme.AccentColor), accentRect);
                }

                // STEP 3: Draw keyboard focus indicator
                // Focus state: consider owner focus on selected item (approximation)
                bool isFocused = _owner.Focused && isSelected;
                if (isFocused)
                {
                    // Dotted outline for keyboard navigation
                    using (var focusPen = new Pen(_theme.AccentColor, Scale(2)))
                    {
                        focusPen.DashStyle = DashStyle.Dot;
                        var focusRect = contentBounds;
                        focusRect.Inflate(-Scale(1), -Scale(1));
                        g.DrawRectangle(focusPen, focusRect);
                    }
                }

                // Calculate content areas
                int leftOffset = contentBounds.X + (isSelected ? Scale(10) : Scale(8));
                int iconSize = Scale(18);
                int spacing = Scale(8);

                // STEP 4: Draw checkmark indicator for selected items
                if (isSelected)
                {
                    var checkRect = new Rectangle(
                        contentBounds.Right - Scale(24),
                        contentBounds.Y + (contentBounds.Height - iconSize) / 2,
                        iconSize,
                        iconSize
                    );

                    // Draw checkmark using path
                    using (var checkPen = new Pen(_theme.AccentColor, Scale(2)))
                    {
                        checkPen.StartCap = LineCap.Round;
                        checkPen.EndCap = LineCap.Round;

                        var centerX = checkRect.X + checkRect.Width / 2;
                        var centerY = checkRect.Y + checkRect.Height / 2;
                        var size = checkRect.Width / 3;

                        g.DrawLine(checkPen,
                            centerX - size, centerY,
                            centerX - size / 3, centerY + size);
                        g.DrawLine(checkPen,
                            centerX - size / 3, centerY + size,
                            centerX + size, centerY - size);
                    }
                }

                // STEP 5: Draw icon (if present)
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    var iconRect = new Rectangle(
                        leftOffset,
                        contentBounds.Y + (contentBounds.Height - iconSize) / 2,
                        iconSize,
                        iconSize
                    );

                    // Simple circular icon background
                    g.FillEllipse(GetBrush(Color.FromArgb(50, _theme.AccentColor)), iconRect);

                    // Draw actual icon via StyledImagePainter (clipped to ellipse)
                    using (var clipPath = new GraphicsPath())
                    {
                        clipPath.AddEllipse(iconRect);
                        var state = g.Save();
                        g.SetClip(clipPath, CombineMode.Intersect);
                        try
                        {
                            StyledImagePainter.Paint(g, iconRect, item.ImagePath, Style);
                        }
                        finally
                        {
                            g.Restore(state);
                        }
                    }
                    leftOffset += iconSize + spacing;
                }

                // STEP 6: Draw text content
                var textRect = new Rectangle(
                    leftOffset,
                    contentBounds.Y,
                    contentBounds.Right - leftOffset - (isSelected ? Scale(32) : Scale(12)),
                    contentBounds.Height
                );

                Color textColor = _theme.LabelForeColor;
                
                var font = GetCachedFont(_owner.TextFont.Size, FontStyle.Regular);
                TextRenderer.DrawText(g, item.Text ?? string.Empty, font, textRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

                // STEP 7: Draw description/subtitle if present
                if (!string.IsNullOrEmpty(item.Description))
                {
                    var descRect = new Rectangle(
                        leftOffset,
                        contentBounds.Y + contentBounds.Height / 2 + Scale(2),
                        textRect.Width,
                        contentBounds.Height / 2 - Scale(2)
                    );

                    Color descColor = Color.FromArgb(120, textColor);
                    var descFont = GetCachedFont(_owner.TextFont.Size - 1.5f, FontStyle.Regular);
                    TextRenderer.DrawText(g, item.Description, descFont, descRect, descColor,
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

        public override int GetPreferredItemHeight()
        {
            return Scale(36); // Reka UI default compact height
        }

        // Enhanced hover effects and selection indicators
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for RekaUI background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);

                // Add hover effect with subtle shadow
                if (isHovered && !isSelected)
                {
                    g.FillPath(GetBrush(Color.FromArgb(30, _theme.AccentColor)), path);
                }
            }
        }
    }
}
