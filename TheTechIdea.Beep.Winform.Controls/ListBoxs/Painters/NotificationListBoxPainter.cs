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
            g.FillEllipse(GetBrush(Color.FromArgb(30, tint)), icRect);
            DrawCircularAvatar(g, icRect, item);

            // ── Trailing zone ────────────────────────────────────────────
            int trailingW = Scale(ListBoxTokens.NotificationTrailingWidth);
            int trailingX = itemRect.Right - trailingW - hm;

            Color secondaryFg = Color.FromArgb(ListBoxTokens.SubTextAlpha,
                _theme?.ListItemForeColor ?? Color.Gray);

            // Time (SubText) — top-right
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var timeFont = GetCachedFont(Math.Max(7f, _owner.TextFont.Size - 2f), FontStyle.Regular);
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
            var boldFont = GetCachedFont(_owner.TextFont.Size, FontStyle.Bold);
            DrawItemText(g, titleRect, item.Text ?? item.DisplayField, primaryFg, boldFont);

            // Description body (2 lines max, word-wrapped)
            if (!string.IsNullOrEmpty(item.Description))
            {
                var descRect = new Rectangle(textX, titleRect.Bottom + Scale(2), textW, Scale(34));
                var descFont = GetCachedFont(Math.Max(8f, _owner.TextFont.Size - 1.5f), FontStyle.Regular);
                TextRenderer.DrawText(g, item.Description, descFont, descRect, secondaryFg,
                    TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        private void DrawTypeBadge(Graphics g, int areaX, int y, int areaW,
            string text, Color badgeColor)
        {
            var font = GetCachedFont(Math.Max(6f, _owner.TextFont.Size - 3.5f), FontStyle.Bold);
            var textSize = TextRenderer.MeasureText(text, font);
            int pad  = Scale(4);
            int pillW = Math.Max(Scale(ListBoxTokens.BadgeMinWidth), textSize.Width + pad);
            int pillH = Math.Max(Scale(ListBoxTokens.BadgePillRadius * 2), textSize.Height + 2);
            int pillX = areaX + areaW - pillW;
            var pillRect = new RectangleF(pillX, y, pillW, pillH);

            var fill = badgeColor == Color.Empty
                ? (_theme?.PrimaryColor ?? Color.DodgerBlue)
                : badgeColor;
            int r = Scale(ListBoxTokens.BadgePillRadius);
            using var path = GraphicsExtensions.CreateRoundedRectanglePath(pillRect, r);
            g.FillPath(GetBrush(fill), path);

            Color textColor = fill.GetBrightness() > 0.55f ? Color.Black : Color.White;
            TextRenderer.DrawText(g, text, font, Rectangle.Round(pillRect), textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        public override bool SupportsCheckboxes() => false;
    }
}
