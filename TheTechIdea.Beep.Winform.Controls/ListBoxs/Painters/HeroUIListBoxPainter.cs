using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// HeroUI-inspired listbox painter with modern, clean design
    /// Features: Rounded items, subtle shadows, smooth hover states, focus rings
    /// Based on: https://www.heroui.com/docs/components/listbox
    /// </summary>
    internal class HeroUIListBoxPainter : BaseListBoxPainter
    {
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
                // Add padding to item bounds
                var contentBounds = new Rectangle(
                    itemRect.X + Scale(8),
                    itemRect.Y + Scale(4),
                    itemRect.Width - Scale(16),
                    itemRect.Height - Scale(8)
                );

                // STEP 1: Draw item background
                DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

                // Calculate content areas
                int leftOffset = contentBounds.X + Scale(12);
                int iconSize = Scale(20);
                int spacing = Scale(8);

                // STEP 4: Draw start content (icon/avatar)
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    var iconRect = new Rectangle(leftOffset, contentBounds.Y + (contentBounds.Height - iconSize) / 2, iconSize, iconSize);
                    
                    // Draw rounded icon background
                    using (var iconPath = GraphicsExtensions.CreateRoundedRectanglePath(iconRect, Scale(4)))
                    using (var iconBgBrush = new SolidBrush(Color.FromArgb(30, _theme.AccentColor)))
                    {
                        g.FillPath(iconBgBrush, iconPath);
                    }

                    // Draw icon using StyledImagePainter
                    DrawItemImage(g, iconRect, item.ImagePath);
                    leftOffset += iconSize + spacing;
                }

                // STEP 5: Draw text content
                var textRect = new Rectangle(
                    leftOffset,
                    contentBounds.Y,
                    contentBounds.Right - leftOffset - Scale(12),
                    contentBounds.Height
                );

                Color textColor = isSelected ? Color.White : _theme.LabelForeColor;
                
                // Main text
                using (var textBrush = new SolidBrush(textColor))
                using (var font = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size, FontStyle.Regular))
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

                // STEP 6: Draw end content (shortcut/badge)
                if (!string.IsNullOrEmpty(item.Description))
                {
                    var badgeText = item.Description;
                    using (var badgeFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size - 1, FontStyle.Regular))
                    {
                        var badgeSize = TextUtils.MeasureText(g, badgeText, badgeFont);
                        var badgeRect = new Rectangle(
                            contentBounds.Right - (int)badgeSize.Width - Scale(20),
                            contentBounds.Y + (contentBounds.Height - (int)badgeSize.Height) / 2,
                            (int)badgeSize.Width + Scale(12),
                            (int)badgeSize.Height + Scale(4)
                        );

                        // Badge background
                        using (var badgePath = GraphicsExtensions.CreateRoundedRectanglePath(badgeRect, Scale(4)))
                        using (var badgeBrush = new SolidBrush(Color.FromArgb(50, _theme.AccentColor)))
                        {
                            g.FillPath(badgeBrush, badgePath);
                        }

                        // Badge text
                        Color badgeColor = Color.FromArgb(180, textColor);
                        using (var badgeTextBrush = new SolidBrush(badgeColor))
                        {
                            var sf = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };
                            g.DrawString(badgeText, badgeFont, badgeTextBrush, badgeRect, sf);
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
            return Scale(44); // HeroUI default item height
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for HeroUI background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);
                if (isHovered)
                {
                    using (var hoverBrush = new SolidBrush(Color.FromArgb(50, Color.Gray)))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }

    }
}
