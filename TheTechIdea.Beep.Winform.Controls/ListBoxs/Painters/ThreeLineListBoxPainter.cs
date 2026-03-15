using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Three-line list painter — 88 px rows with a 48 px rounded-square thumbnail
    /// and up to four text lines (title + SubText + SubText2 + SubText3).
    /// Follows the MD3 canonical three-line list spec.
    /// </summary>
    internal class ThreeLineListBoxPainter : BaseListBoxPainter
    {
        public override int GetPreferredItemHeight()
            => Scale(ListBoxTokens.ThreeLineRowHeight);

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                         bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int hm    = Scale(ListBoxTokens.RichListHMargin);
            int imgSz = Scale(ListBoxTokens.ThreeLineImageSize);
            int gap   = Scale(ListBoxTokens.AvatarTextGap);
            int radius = Scale(ListBoxTokens.ThreeLineImageRadius);

            // ── Image (rounded square, optional) ─────────────────────────
            int textX;
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int imgY = itemRect.Y + (itemRect.Height - imgSz) / 2;
                var imgRect = new Rectangle(itemRect.X + hm, imgY, imgSz, imgSz);

                // Clip to rounded rectangle
                var state = g.Save();
                using (var clipPath = GraphicsExtensions.CreateRoundedRectanglePath(
                    new RectangleF(imgRect.X, imgRect.Y, imgRect.Width, imgRect.Height), radius))
                {
                    g.SetClip(clipPath);
                    DrawItemImage(g, imgRect, item.ImagePath);
                }
                g.Restore(state);

                // Subtle border
                using var borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1f);
                using var borderPath = GraphicsExtensions.CreateRoundedRectanglePath(
                    new RectangleF(imgRect.X, imgRect.Y, imgRect.Width, imgRect.Height), radius);
                g.DrawPath(borderPen, borderPath);

                textX = imgRect.Right + gap;
            }
            else
            {
                textX = itemRect.X + hm;
            }

            // ── Text lines ───────────────────────────────────────────────
            int textW = itemRect.Right - textX - hm;
            int lineH = Scale(18);

            Color primaryFg = isSelected
                ? Color.White
                : (_theme?.ListItemForeColor ?? Color.Black);
            Color secondaryFg = Color.FromArgb(ListBoxTokens.SubTextAlpha,
                _theme?.ListItemForeColor ?? Color.Gray);
            Color tertiaryFg = Color.FromArgb((int)(ListBoxTokens.SubTextAlpha * 0.65f),
                _theme?.ListItemForeColor ?? Color.Gray);

            // Count visible lines for centering
            int linesCount = 1
                + (string.IsNullOrEmpty(item.SubText) ? 0 : 1)
                + (string.IsNullOrEmpty(item.SubText2) ? 0 : 1)
                + (string.IsNullOrEmpty(item.SubText3) ? 0 : 1);
            int totalTextH = linesCount * lineH;
            int startY = itemRect.Y + (itemRect.Height - totalTextH) / 2;
            int curY = startY;

            // Line 1: Title (bold)
            using var boldFont = BeepFontManager.GetFont(
                _owner.TextFont.Name, _owner.TextFont.Size, FontStyle.Bold);
            DrawItemText(g, new Rectangle(textX, curY, textW, lineH),
                item.Text ?? item.DisplayField, primaryFg, boldFont);
            curY += lineH;

            // Line 2: SubText (secondary)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                DrawSubText(g, new Rectangle(textX, curY, textW, lineH),
                    item.SubText, primaryFg, _owner.TextFont);
                curY += lineH;
            }

            // Line 3: SubText2 (secondary)
            if (!string.IsNullOrEmpty(item.SubText2))
            {
                DrawSubText(g, new Rectangle(textX, curY, textW, lineH),
                    item.SubText2, primaryFg, _owner.TextFont);
                curY += lineH;
            }

            // Line 4: SubText3 (tertiary, smaller)
            if (!string.IsNullOrEmpty(item.SubText3))
            {
                using var smallFont = BeepFontManager.GetFont(
                    _owner.TextFont.Name, Math.Max(7f, _owner.TextFont.Size - 2f), FontStyle.Regular);
                DrawItemText(g, new Rectangle(textX, curY, textW, lineH),
                    item.SubText3, tertiaryFg, smallFont);
            }
        }

        public override bool SupportsCheckboxes() => true;
    }
}
