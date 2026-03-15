using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Contact list painter — 72 px rows with circular avatar,
    /// bold name, role/title, and email/phone on three text lines.
    /// </summary>
    internal class ContactListBoxPainter : BaseListBoxPainter
    {
        public override int GetPreferredItemHeight()
            => Scale(ListBoxTokens.ContactRowHeight);

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item,
                                         bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int hm   = Scale(ListBoxTokens.RichListHMargin);
            int avSz = Scale(ListBoxTokens.ContactAvatarSize);
            int gap  = Scale(12);

            // ── Avatar (circular) ────────────────────────────────────────
            int avY = itemRect.Y + (itemRect.Height - avSz) / 2;
            var avRect = new Rectangle(itemRect.X + hm, avY, avSz, avSz);
            DrawCircularAvatar(g, avRect, item);

            // ── Text lines ───────────────────────────────────────────────
            int textX = avRect.Right + gap;
            int textW = itemRect.Right - textX - hm;
            int lineH = Scale(18);

            Color primaryFg = isSelected
                ? Color.White
                : (_theme?.ListItemForeColor ?? Color.Black);
            Color secondaryFg = Color.FromArgb(ListBoxTokens.SubTextAlpha,
                _theme?.ListItemForeColor ?? Color.Gray);
            Color tertiaryFg = Color.FromArgb((int)(ListBoxTokens.SubTextAlpha * 0.7f),
                _theme?.ListItemForeColor ?? Color.Gray);

            // Calculate vertical start to center the text block
            int linesCount = 1
                + (string.IsNullOrEmpty(item.SubText) ? 0 : 1)
                + (string.IsNullOrEmpty(item.SubText2) ? 0 : 1);
            int totalTextH = linesCount * lineH;
            int startY = itemRect.Y + (itemRect.Height - totalTextH) / 2;

            // Line 1: Name (bold)
            var nameRect = new Rectangle(textX, startY, textW, lineH);
            using var boldFont = BeepFontManager.GetFont(
                _owner.TextFont.Name, _owner.TextFont.Size, FontStyle.Bold);
            DrawItemText(g, nameRect, item.Text ?? item.DisplayField, primaryFg, boldFont);

            // Line 2: Role / Title (SubText, secondary)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var roleRect = new Rectangle(textX, nameRect.Bottom, textW, lineH);
                DrawSubText(g, roleRect, item.SubText, primaryFg, _owner.TextFont);
            }

            // Line 3: Email / Phone (SubText2, tertiary, smaller)
            if (!string.IsNullOrEmpty(item.SubText2))
            {
                int line3Y = nameRect.Bottom + (string.IsNullOrEmpty(item.SubText) ? 0 : lineH);
                var emailRect = new Rectangle(textX, line3Y, textW, lineH);
                using var smallFont = BeepFontManager.GetFont(
                    _owner.TextFont.Name, Math.Max(7f, _owner.TextFont.Size - 2f), FontStyle.Regular);
                DrawItemText(g, emailRect, item.SubText2, tertiaryFg, smallFont);
            }
        }

        public override bool SupportsCheckboxes() => true;
    }
}
