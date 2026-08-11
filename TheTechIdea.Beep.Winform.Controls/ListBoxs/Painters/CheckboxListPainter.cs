using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// List with checkboxes for multi-select with distinct styling
    /// </summary>
    internal class CheckboxListPainter : OutlinedListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(
                Scale(ListBoxTokens.ItemPaddingH),
                Scale(ListBoxTokens.ItemPaddingV),
                Scale(ListBoxTokens.ItemPaddingH),
                Scale(ListBoxTokens.ItemPaddingV));
        }

        public override int GetPreferredItemHeight()
        {
            // Slightly taller for better checkbox targeting
            return Math.Max(Scale(ListBoxTokens.ItemHeightDense), Scale(ListBoxTokens.MinTouchTargetPx));
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || item == null || itemRect.IsEmpty) return;

            // Draw item background
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            var rich     = item as BeepListItem;
            bool disabled = rich?.IsDisabled == true;

            // Calculate checkbox rectangle
            var padding     = GetPreferredPadding();
            int cbSize      = DpiScalingHelper.ScaleValue(ListBoxTokens.CheckboxSize, _owner);
            var checkboxRect = new Rectangle(
                itemRect.X + padding.Left,
                itemRect.Y + (itemRect.Height - cbSize) / 2,
                cbSize, cbSize);

            // Draw checkbox (disabled items show greyed-out box)
            DrawCheckbox(g, checkboxRect, item.IsChecked, isHovered && !disabled);

            // Content area after checkbox
            int gap      = DpiScalingHelper.ScaleValue(8, _owner);
            var textRect = new Rectangle(
                checkboxRect.Right + gap,
                itemRect.Y,
                itemRect.Width - checkboxRect.Right - gap - padding.Right,
                itemRect.Height);

            // Badge — shrink textRect
            if (rich != null && !string.IsNullOrEmpty(rich.BadgeText))
            {
                int badgePad = DpiScalingHelper.ScaleValue(72, _owner);
                DrawBadgePill(g, itemRect, rich.BadgeText, rich.BadgeColor);
                textRect.Width -= badgePad;
            }

            // Determine text colour
            Color textColor;
            if (_owner.IsHighContrast)
                textColor = _owner.HCItemForeground(isSelected);
            else if (disabled)
                textColor = System.Drawing.Color.FromArgb(ListBoxTokens.DisabledAlpha,
                    Theme.ListItemForeColor);
            else
                textColor = isSelected
                    ? Theme.OnPrimaryColor
                    : Theme.ListItemForeColor;

            // Sub-text 2-line layout
            if (rich != null && !string.IsNullOrEmpty(rich.SubText))
            {
                int subH   = DpiScalingHelper.ScaleValue(16, _owner);
                int titleH = Math.Max(12, textRect.Height - subH - DpiScalingHelper.ScaleValue(ListBoxTokens.SubTextGap, _owner));
                var titleRect = new Rectangle(textRect.X, textRect.Y, textRect.Width, titleH);
                var subRect   = new Rectangle(textRect.X,
                    textRect.Y + titleH + DpiScalingHelper.ScaleValue(ListBoxTokens.SubTextGap, _owner),
                    textRect.Width, subH);
                DrawItemText(g, titleRect, item.Text, textColor, _owner.TextFont);
                DrawSubText(g, subRect, rich.SubText,
                    _owner.IsHighContrast ? _owner.HCItemForeground(isSelected) : Theme.ListItemForeColor,
                    _owner.TextFont);
            }
            else
            {
                DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
            }

            // Focus ring
            var visible = _helper?.GetVisibleItems();
            int fi = _owner.FocusedIndex;
            if (fi >= 0 && visible != null && fi < visible.Count && visible[fi] == item)
                DrawFocusRing(g, itemRect);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, Scale(ListBoxTokens.CornerRadiusSmall)))
            {
                if (isSelected)
                {
                    var selColor = Theme.PrimaryColor;

                    // Selected background with subtle gradient
                    using (var brush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(30, selColor.R, selColor.G, selColor.B),
                        Color.FromArgb(10, selColor.R, selColor.G, selColor.B),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Selection border
                    g.DrawPath(GetPen(selColor, 2f), path);
                }
                else if (isHovered)
                {
                    // Hover background
                    var hoverBg = Theme.ListItemHoverBackColor;
                    g.FillPath(GetBrush(hoverBg), path);

                    // Hover border
                    g.DrawPath(GetPen(Theme.AccentColor, 1.5f), path);
                }
                else
                {
                    // Normal state
                    var normalBg = Theme.BackgroundColor;
                    g.FillPath(GetBrush(normalBg), path);

                    // Normal border
                    g.DrawPath(GetPen(Theme.BorderColor, 1f), path);
                }
            }

            // Draw subtle divider
            {
                int inset = Scale(ListBoxTokens.IconTextGap - 2);
                g.DrawLine(GetPen(Theme.BorderColor, 1f), itemRect.Left + inset, itemRect.Bottom - 1, itemRect.Right - inset, itemRect.Bottom - 1);
            }
        }
    }
}
