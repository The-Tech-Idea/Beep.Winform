using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Chakra UI-inspired listbox painter with accessible, colorful design
    /// Features: Double focus rings, accessible colors, smooth animations feel, rounded corners
    /// Based on: https://chakra-ui.com/docs/components/listbox
    /// </summary>
    internal class ChakraUIListBoxPainter : BaseListBoxPainter
    {
        /// <summary>
        /// Draws the item background (hover/selected) with Chakra-Style rounded container.
        /// </summary>
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.Width <= 0 || itemRect.Height <= 0)
                return;

            // Use BeepStyling for ChakraUI background, border, and shadow
           
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);
            }
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || item == null || itemRect.Width <= 0 || itemRect.Height <= 0)
                return;

            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Background via base contract
                DrawItemBackground(g, itemRect, isHovered, isSelected);

                // Chakra UI spacing (content rect inside the rounded background)
                var contentBounds = new Rectangle(
                    itemRect.X + 6,
                    itemRect.Y + 3,
                    itemRect.Width - 12,
                    itemRect.Height - 6
                );

                // STEP 2: Draw Chakra's signature double focus ring
                // Owner handles focus; optional ring here disabled to keep API parity
                bool isFocused = false;
                if (isFocused)
                {
                    // Outer ring (shadow ring)
                    var outerRect = contentBounds;
                    outerRect.Inflate(3, 3);
                    using (var outerPath = CreateRoundedRectangle(outerRect, 9))
                    using (var outerPen = new Pen(Color.FromArgb(60, _theme.AccentColor), 4))
                    {
                        g.DrawPath(outerPen, outerPath);
                    }

                    // Inner ring (focus ring)
                    var innerRect = contentBounds;
                    innerRect.Inflate(1, 1);
                    using (var innerPath = CreateRoundedRectangle(innerRect, 7))
                    using (var innerPen = new Pen(_theme.AccentColor, 2))
                    {
                        g.DrawPath(innerPen, innerPath);
                    }
                }

                // Calculate content areas
                int leftOffset = contentBounds.X + 12;
                int iconSize = 20;
                int spacing = 10;

                // STEP 3: Draw icon with Chakra styling
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    var iconRect = new Rectangle(
                        leftOffset,
                        contentBounds.Y + (contentBounds.Height - iconSize) / 2,
                        iconSize,
                        iconSize
                    );

                    // Rounded icon background with Chakra colors
                    using (var iconPath = CreateRoundedRectangle(iconRect, 4))
                    {
                        Color iconBgColor = isSelected
                            ? Color.FromArgb(60, _theme.AccentColor)
                            : Color.FromArgb(237, 242, 247); // gray.100

                        using (var iconBgBrush = new SolidBrush(iconBgColor))
                        {
                            g.FillPath(iconBgBrush, iconPath);
                        }
                    }

                    // TODO: Draw actual icon
                    leftOffset += iconSize + spacing;
                }

                // STEP 4: Draw selected indicator (checkmark icon)
                if (isSelected)
                {
                    var checkSize = 18;
                    var checkRect = new Rectangle(
                        contentBounds.Right - checkSize - 12,
                        contentBounds.Y + (contentBounds.Height - checkSize) / 2,
                        checkSize,
                        checkSize
                    );

                    // Rounded background for checkmark
                    using (var checkBgPath = CreateRoundedRectangle(checkRect, 4))
                    using (var checkBgBrush = new SolidBrush(_theme.AccentColor))
                    {
                        g.FillPath(checkBgBrush, checkBgPath);
                    }

                    // Draw checkmark
                    using (var checkPen = new Pen(Color.White, 2))
                    {
                        checkPen.StartCap = LineCap.Round;
                        checkPen.EndCap = LineCap.Round;

                        var centerX = checkRect.X + checkRect.Width / 2;
                        var centerY = checkRect.Y + checkRect.Height / 2;
                        var size = 4;

                        g.DrawLine(checkPen,
                            centerX - size, centerY,
                            centerX - 1, centerY + size);
                        g.DrawLine(checkPen,
                            centerX - 1, centerY + size,
                            centerX + size + 1, centerY - size);
                    }
                }

                // STEP 5: Draw main text
                var textRect = new Rectangle(
                    leftOffset,
                    contentBounds.Y,
                    contentBounds.Right - leftOffset - (isSelected ? 36 : 12),
                    contentBounds.Height
                );

                // Chakra uses specific text colors
                Color textColor = isSelected 
                    ? Color.FromArgb(44, 82, 130) // blue.700
                    : Color.FromArgb(26, 32, 44); // gray.800
                
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

                // STEP 6: Draw description with Chakra's muted text
                if (!string.IsNullOrEmpty(item.Description))
                {
                    var descY = contentBounds.Y + contentBounds.Height / 2 + 4;
                    var descRect = new Rectangle(
                        leftOffset,
                        descY,
                        textRect.Width,
                        contentBounds.Bottom - descY - 4
                    );

                    Color descColor = Color.FromArgb(74, 85, 104); // gray.600
                    using (var descBrush = new SolidBrush(descColor))
                    using (var descFont = new Font(_owner.Font.FontFamily, _owner.Font.Size - 1, FontStyle.Regular))
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

                // STEP 7: Draw keyboard shortcut/badge (right aligned)
                if (item.Tag != null && item.Tag is string shortcut && !string.IsNullOrEmpty(shortcut))
                {
                    using (var badgeFont = new Font(_owner.Font.FontFamily, _owner.Font.Size - 1.5f, FontStyle.Regular))
                    {
                        var badgeSize = TextUtils.MeasureText(g, shortcut, badgeFont);
                        var badgeRect = new Rectangle(
                            contentBounds.Right - (int)badgeSize.Width - 24,
                            contentBounds.Y + (contentBounds.Height - (int)badgeSize.Height - 4) / 2,
                            (int)badgeSize.Width + 12,
                            (int)badgeSize.Height + 4
                        );

                        // Chakra badge styling
                        using (var badgePath = CreateRoundedRectangle(badgeRect, 3))
                        {
                            Color badgeBgColor = Color.FromArgb(237, 242, 247); // gray.100
                            using (var badgeBgBrush = new SolidBrush(badgeBgColor))
                            {
                                g.FillPath(badgeBgBrush, badgePath);
                            }
                        }

                        // Badge text
                        Color badgeTextColor = Color.FromArgb(74, 85, 104); // gray.600
                        using (var badgeTextBrush = new SolidBrush(badgeTextColor))
                        {
                            var sf = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };
                            g.DrawString(shortcut, badgeFont, badgeTextBrush, badgeRect, sf);
                        }
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
            return 40; // Chakra UI default item height
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
