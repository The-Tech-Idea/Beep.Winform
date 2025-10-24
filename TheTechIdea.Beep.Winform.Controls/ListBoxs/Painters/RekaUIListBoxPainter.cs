using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

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
                    itemBounds.X + 4,
                    itemBounds.Y + 2,
                    itemBounds.Width - 8,
                    itemBounds.Height - 4
                );

                // STEP 1: Draw item background
                DrawItemBackground(g, itemBounds, isHovered, isSelected);

                // STEP 2: Draw left accent bar on selected items
                if (isSelected)
                {
                    var accentRect = new Rectangle(contentBounds.X, contentBounds.Y, 3, contentBounds.Height);
                    using (var accentBrush = new SolidBrush(_theme.AccentColor))
                    {
                        g.FillRectangle(accentBrush, accentRect);
                    }
                }

                // STEP 3: Draw keyboard focus indicator
                // Focus state: consider owner focus on selected item (approximation)
                bool isFocused = _owner.Focused && isSelected;
                if (isFocused)
                {
                    // Dotted outline for keyboard navigation
                    using (var focusPen = new Pen(_theme.AccentColor, 2))
                    {
                        focusPen.DashStyle = DashStyle.Dot;
                        var focusRect = contentBounds;
                        focusRect.Inflate(-1, -1);
                        g.DrawRectangle(focusPen, focusRect);
                    }
                }

                // Calculate content areas
                int leftOffset = contentBounds.X + (isSelected ? 10 : 8);
                int iconSize = 18;
                int spacing = 8;

                // STEP 4: Draw checkmark indicator for selected items
                if (isSelected)
                {
                    var checkRect = new Rectangle(
                        contentBounds.Right - 24,
                        contentBounds.Y + (contentBounds.Height - iconSize) / 2,
                        iconSize,
                        iconSize
                    );

                    // Draw checkmark using path
                    using (var checkPen = new Pen(_theme.AccentColor, 2))
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
                    using (var iconBrush = new SolidBrush(Color.FromArgb(50, _theme.AccentColor)))
                    {
                        g.FillEllipse(iconBrush, iconRect);
                    }

                    // TODO: Draw actual icon
                    leftOffset += iconSize + spacing;
                }

                // STEP 6: Draw text content
                var textRect = new Rectangle(
                    leftOffset,
                    contentBounds.Y,
                    contentBounds.Right - leftOffset - (isSelected ? 32 : 12),
                    contentBounds.Height
                );

                Color textColor = _theme.LabelForeColor;
                
                using (var textBrush = new SolidBrush(textColor))
                using (var font = new Font(_owner.Font.FontFamily, _owner.Font.Size, FontStyle.Regular))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap
                    };

                    g.DrawString(item.Text ?? string.Empty, font, textBrush, textRect, sf);
                }

                // STEP 7: Draw description/subtitle if present
                if (!string.IsNullOrEmpty(item.Description))
                {
                    var descRect = new Rectangle(
                        leftOffset,
                        contentBounds.Y + contentBounds.Height / 2 + 2,
                        textRect.Width,
                        contentBounds.Height / 2 - 2
                    );

                    Color descColor = Color.FromArgb(120, textColor);
                    using (var descBrush = new SolidBrush(descColor))
                    using (var descFont = new Font(_owner.Font.FontFamily, _owner.Font.Size - 1.5f, FontStyle.Regular))
                    {
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Near,
                            Trimming = StringTrimming.EllipsisCharacter
                        };

                        g.DrawString(item.Description, descFont, descBrush, descRect, sf);
                    }
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
            return 36; // Reka UI default compact height
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for RekaUI background, border, and shadow
           
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);
            }
        }
    }
}
