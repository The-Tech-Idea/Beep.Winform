using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Profile card painter — 120 px rows with a large 64 px centered circular avatar,
    /// centered bold name, centered subtitle, and a 2-line centered bio/description.
    /// </summary>
    internal class ProfileCardListBoxPainter : BaseListBoxPainter
    {
        public override int GetPreferredItemHeight()
            => Scale(ListBoxTokens.ProfileCardRowHeight);

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                         bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int avSz = Scale(ListBoxTokens.ProfileCardAvatarSize);
            int hm   = Scale(ListBoxTokens.RichListHMargin);

            // ── Avatar (centered, circular) ──────────────────────────────
            int centerX = itemRect.X + (itemRect.Width - avSz) / 2;
            var avRect = new Rectangle(centerX, itemRect.Y + Scale(8), avSz, avSz);
            DrawCircularAvatar(g, avRect, item);

            // ── Text area (centered) ─────────────────────────────────────
            int textX = itemRect.X + hm;
            int textW = itemRect.Width - hm * 2;
            int curY  = avRect.Bottom + Scale(6);

            Color primaryFg = isSelected
                ? Color.White
                : (_theme?.ListItemForeColor ?? Color.Black);
            Color secondaryFg = Color.FromArgb(ListBoxTokens.SubTextAlpha,
                _theme?.ListItemForeColor ?? Color.Gray);
            Color tertiaryFg = Color.FromArgb((int)(ListBoxTokens.SubTextAlpha * 0.7f),
                _theme?.ListItemForeColor ?? Color.Gray);

            // Name (bold, centered)
            var boldFont = GetCachedFont(_owner.TextFont.Size + 1f, FontStyle.Bold);
            var nameRect = new Rectangle(textX, curY, textW, Scale(20));
            TextRenderer.DrawText(g, item.Text ?? item.DisplayField, boldFont, nameRect, primaryFg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            curY += Scale(20);

            // Title / Role (SubText, centered, secondary)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var regFont = GetCachedFont(_owner.TextFont.Size, FontStyle.Regular);
                var titleRect = new Rectangle(textX, curY, textW, Scale(16));
                TextRenderer.DrawText(g, item.SubText, regFont, titleRect, secondaryFg,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                curY += Scale(16);
            }

            // Bio / Description (centered, tertiary, max 2 lines, word-wrapped)
            if (!string.IsNullOrEmpty(item.Description))
            {
                var bioFont = GetCachedFont(Math.Max(7f, _owner.TextFont.Size - 1.5f), FontStyle.Regular);
                var bioRect = new Rectangle(textX, curY, textW, Scale(28));
                TextRenderer.DrawText(g, item.Description, bioFont, bioRect, tertiaryFg,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top |
                    TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        public override bool SupportsSearch() => false;
        public override bool SupportsCheckboxes() => false;
    }
}
