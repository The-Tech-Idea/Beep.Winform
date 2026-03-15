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
    /// Chat/messaging list painter — 72 px rows with circular avatar,
    /// bold name, message preview, right-aligned timestamp, and unread-count badge.
    /// </summary>
    internal class ChatListBoxPainter : BaseListBoxPainter
    {
        public override int GetPreferredItemHeight()
            => Scale(ListBoxTokens.ChatRowHeight);

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                         bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int hm   = Scale(ListBoxTokens.RichListHMargin);
            int avSz = Scale(ListBoxTokens.ChatAvatarSize);
            int gap  = Scale(ListBoxTokens.AvatarTextGap);

            // ── Avatar (circular) ────────────────────────────────────────
            int avY = itemRect.Y + (itemRect.Height - avSz) / 2;
            var avRect = new Rectangle(itemRect.X + hm, avY, avSz, avSz);
            DrawCircularAvatar(g, avRect, item);

            // ── Trailing zone (time + badge) ─────────────────────────────
            int trailingW = Scale(ListBoxTokens.ChatTrailingWidth);
            int trailingX = itemRect.Right - trailingW - hm;

            // ── Content zone ─────────────────────────────────────────────
            int contentX = avRect.Right + gap;
            int contentW = trailingX - contentX - Scale(8);

            Color primaryFg = isSelected
                ? Color.White
                : (_theme?.ListItemForeColor ?? Color.Black);
            Color secondaryFg = Color.FromArgb(ListBoxTokens.SubTextAlpha,
                _theme?.ListItemForeColor ?? Color.Gray);

            // Name (bold)
            int nameY = itemRect.Y + Scale(14);
            var nameRect = new Rectangle(contentX, nameY, contentW, Scale(20));
            using var boldFont = BeepFontManager.GetFont(
                _owner.TextFont.Name, _owner.TextFont.Size, FontStyle.Bold);
            DrawItemText(g, nameRect, item.Text ?? item.DisplayField, primaryFg, boldFont);

            // Message preview (SubText)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var msgRect = new Rectangle(contentX, nameRect.Bottom + Scale(2), contentW, Scale(18));
                using var regFont = BeepFontManager.GetFont(
                    _owner.TextFont.Name, Math.Max(8f, _owner.TextFont.Size - 1.5f), FontStyle.Regular);
                DrawItemText(g, msgRect, item.SubText, secondaryFg, regFont);
            }

            // ── Trailing: Time (ShortcutText) ───────────────────────────
            if (!string.IsNullOrEmpty(item.ShortcutText))
            {
                using var timeFont = BeepFontManager.GetFont(
                    _owner.TextFont.Name, Math.Max(7f, _owner.TextFont.Size - 2f), FontStyle.Regular);
                var timeRect = new Rectangle(trailingX, nameY, trailingW, Scale(16));
                TextRenderer.DrawText(g, item.ShortcutText, timeFont, timeRect, secondaryFg,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            // ── Trailing: Badge pill (BadgeText) ─────────────────────────
            if (!string.IsNullOrEmpty(item.BadgeText))
            {
                DrawUnreadBadge(g, trailingX, nameRect.Bottom + Scale(4),
                    trailingW, item.BadgeText, item.BadgeBackColor);
            }
        }

        private void DrawUnreadBadge(Graphics g, int areaX, int y, int areaW,
            string text, Color badgeColor)
        {
            using var font = BeepFontManager.GetFont(
                _owner.TextFont.Name, Math.Max(7f, _owner.TextFont.Size - 3f), FontStyle.Bold);
            var textSize = TextRenderer.MeasureText(text, font);
            int pad  = Scale(6);
            int pillW = Math.Max(Scale(20), textSize.Width + pad);
            int pillH = Math.Max(Scale(16), textSize.Height + 2);
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
