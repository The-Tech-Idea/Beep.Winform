using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Borderless list box with bottom border on selection.
    /// Extends BaseListBoxPainter for full token / DPI / theme support.
    /// </summary>
    internal class BorderlessListBoxPainter : BaseListBoxPainter
    {
        public override bool SupportsSearch() => false;
        public override bool SupportsCheckboxes() => false;

        public override int GetPreferredItemHeight()
            => Scale(ListBoxTokens.ItemHeightCompact); // 40 logical px

        public override System.Windows.Forms.Padding GetPreferredPadding()
            => new System.Windows.Forms.Padding(Scale(ListBoxTokens.ItemPaddingH), Scale(ListBoxTokens.ItemPaddingV),
                                                 Scale(ListBoxTokens.ItemPaddingH), Scale(ListBoxTokens.ItemPaddingV));

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            var padding = GetPreferredPadding();
            var contentRect = Rectangle.Inflate(itemRect, -padding.Left, -padding.Top);

            Color textColor = isSelected
                ? (_theme?.OnPrimaryColor ?? Color.White)
                : (_theme?.ListItemForeColor ?? Color.Black);

            DrawItemText(g, contentRect, item.Text, textColor, _owner.TextFont);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            // Borderless: no outline, only a bottom accent line on selection
            if (isSelected)
            {
                int inset = Scale(8);
                using var pen = new Pen(_theme?.AccentColor ?? Color.Blue, Scale(2));
                g.DrawLine(pen, itemRect.Left + inset, itemRect.Bottom - Scale(2),
                                itemRect.Right - inset, itemRect.Bottom - Scale(2));
            }
        }
    }
}
