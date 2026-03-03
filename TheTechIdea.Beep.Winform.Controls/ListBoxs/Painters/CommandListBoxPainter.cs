using System;
using System.Drawing;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Command-palette style list painter.
    /// Each row: [icon 20px] [command name] ... [keyboard shortcut right-aligned]
    /// Shortcut is stored in BeepListItem.SubText (e.g. "Ctrl+Shift+P").
    /// </summary>
    internal class CommandListBoxPainter : BaseListBoxPainter
    {
        public override int GetPreferredItemHeight()
            => DpiScalingHelper.ScaleValue(ListBoxTokens.ItemHeightCompact, _owner ?? new System.Windows.Forms.Control());

        public override void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            // Use base paint (background + items loop), drawing is delegated via DrawItem
            base.Paint(g, owner, drawingRect);
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            bool isDisabled = item is BeepListItem r && r.IsDisabled;
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int pad  = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, _owner);
            int iconSz = DpiScalingHelper.ScaleValue(ListBoxTokens.IconSize, _owner);

            int alpha = isDisabled ? ListBoxTokens.DisabledAlpha : 255;

            // Icon
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(itemRect.Left + pad,
                    itemRect.Top + (itemRect.Height - iconSz) / 2, iconSz, iconSz);
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            // Command name
            int textX = itemRect.Left + pad + iconSz + DpiScalingHelper.ScaleValue(ListBoxTokens.IconTextGap, _owner);
            Color nameColor = Color.FromArgb(alpha, isSelected
                ? _owner.GetSelectionTextColor()
                : (_theme?.ListItemForeColor ?? Color.Black));

            // Shortcut (from SubText)
            string? shortcut = (item as BeepListItem)?.SubText;
            int shortcutWidth = 0;
            Font? kbdFont = null;

            if (!string.IsNullOrEmpty(shortcut))
            {
                kbdFont = new Font(_owner.Font.FontFamily, Math.Max(7f, _owner.Font.Size - 1.5f));
                shortcutWidth = DpiScalingHelper.ScaleValue(
                    (int)g.MeasureString(shortcut, kbdFont).Width + 8, _owner);
            }

            int nameWidth = itemRect.Right - textX - shortcutWidth - pad;
            var nameRect  = new Rectangle(textX, itemRect.Top, nameWidth, itemRect.Height);
            DrawItemText(g, nameRect, item.Text ?? "", nameColor, TextFont ?? _owner.Font);

            // Shortcut chip
            if (!string.IsNullOrEmpty(shortcut) && kbdFont != null)
            {
                int chipH  = DpiScalingHelper.ScaleValue(20, _owner);
                var chipRect = new Rectangle(itemRect.Right - shortcutWidth - pad,
                    itemRect.Top + (itemRect.Height - chipH) / 2,
                    shortcutWidth, chipH);

                using var chipBrush = new SolidBrush(Color.FromArgb(alpha / 2,
                    _theme?.ButtonBackColor ?? Color.LightGray));
                using var path = RoundedRect(chipRect, DpiScalingHelper.ScaleValue(4, _owner));
                g.FillPath(chipBrush, path);

                using var kbdBrush = new SolidBrush(Color.FromArgb(alpha,
                    _theme?.ListForeColor ?? Color.Gray));
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(shortcut, kbdFont, kbdBrush, chipRect, sf);
                kbdFont.Dispose();
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        public override bool SupportsSearch() => true;
        public override bool SupportsCheckboxes() => false;
    }
}
