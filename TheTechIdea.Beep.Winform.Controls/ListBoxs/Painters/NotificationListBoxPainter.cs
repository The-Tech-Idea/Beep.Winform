using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Notification list painter — 80 px rows with a 40 px circular icon,
    /// bold title, multi-line description body, trailing time, and type badge.
    /// </summary>
    internal class NotificationListBoxPainter : BaseListBoxPainter
    {
        public override int GetPreferredItemHeight()
            => Scale(ListBoxTokens.NotificationRowHeight);

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                         bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int hm   = Scale(ListBoxTokens.RichListHMargin);
            int icSz = Scale(ListBoxTokens.NotificationIconSize);
            int gap  = Scale(12);

            // ── Icon (circular with tinted background) ───────────────────
            int icY = itemRect.Y + (itemRect.Height - icSz) / 2;
            var icRect = new Rectangle(itemRect.X + hm, icY, icSz, icSz);

            // Draw tinted circle background
            var tint = _theme?.PrimaryColor ?? Color.DodgerBlue;
            using (var bgBrush = new SolidBrush(Color.FromArgb(30, tint)))
            {
                g.FillEllipse(bgBrush, icRect);
            }
            DrawCircularAvatar(g, icRect, item);

            // ── Trailing zone ────────────────────────────────────────────
            int trailingW = Scale(ListBoxTokens.NotificationTrailingWidth);
            int trailingX = itemRect.Right - trailingW - hm;

            Color secondaryFg = Color.FromArgb(ListBoxTokens.SubTextAlpha,
                _theme?.ListItemForeColor ?? Color.Gray);

            // Time (SubText) — top-right
            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var timeFont = BeepFontManager.GetFont(
                    _owner.TextFont.Name, Math.Max(7f, _owner.TextFont.Size - 2f), FontStyle.Regular);
                var timeRect = new Rectangle(trailingX, itemRect.Y + Scale(10), trailingW, Scale(16));
                TextRenderer.DrawText(g, item.SubText, timeFont, timeRect, secondaryFg,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            // Badge (BadgeText) — below time
            if (!string.IsNullOrEmpty(item.BadgeText))
            {
                DrawTypeBadge(g, trailingX, itemRect.Y + Scale(30),
                    trailingW, item.BadgeText, item.BadgeBackColor);
            }

            // ── Content zone ─────────────────────────────────────────────
            int textX = icRect.Right + gap;
            int textW = trailingX - textX - Scale(8);

            Color primaryFg = isSelected
                ? Color.White
                : (_theme?.ListItemForeColor ?? Color.Black);

            // Title (bold)
            var titleRect = new Rectangle(textX, itemRect.Y + Scale(10), textW, Scale(18));
            using var boldFont = BeepFontManager.GetFont(
                _owner.TextFont.Name, _owner.TextFont.Size, FontStyle.Bold);
            DrawItemText(g, titleRect, item.Text ?? item.DisplayField, primaryFg, boldFont);

            // Description body (2 lines max, word-wrapped)
            if (!string.IsNullOrEmpty(item.Description))
            {
                var descRect = new Rectangle(textX, titleRect.Bottom + Scale(2), textW, Scale(34));
                using var descFont = BeepFontManager.GetFont(
                    _owner.TextFont.Name, Math.Max(8f, _owner.TextFont.Size - 1.5f), FontStyle.Regular);
                TextRenderer.DrawText(g, item.Description, descFont, descRect, secondaryFg,
                    TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        private void DrawTypeBadge(Graphics g, int areaX, int y, int areaW,
            string text, Color badgeColor)
        {
            using var font = BeepFontManager.GetFont(
                _owner.TextFont.Name, Math.Max(6f, _owner.TextFont.Size - 3.5f), FontStyle.Bold);
            var textSize = TextRenderer.MeasureText(text, font);
            int pad  = Scale(4);
            int pillW = Math.Max(Scale(18), textSize.Width + pad);
            int pillH = Math.Max(Scale(14), textSize.Height + 2);
            int pillX = areaX + areaW - pillW;
            var pillRect = new RectangleF(pillX, y, pillW, pillH);

            var fill = badgeColor == Color.Empty
                ? (_theme?.PrimaryColor ?? Color.DodgerBlue)
                : badgeColor;
            int r = pillH / 2;
            using var path = GraphicsExtensions.CreateRoundedRectanglePath(pillRect, r);
            using var fillBrush = new SolidBrush(fill);
            g.FillPath(fillBrush, path);

            Color textColor = fill.GetBrightness() > 0.55f ? Color.Black : Color.White;
            using var textBrush = new SolidBrush(textColor);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(text, font, textBrush, pillRect, sf);
        }

        public override bool SupportsCheckboxes() => false;
    }
}
