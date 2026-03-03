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
            return new System.Windows.Forms.Padding(12, 6, 12, 6);
        }

        public override int GetPreferredItemHeight()
        {
            // Slightly taller for better checkbox targeting
            return 32;
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
                    _theme?.ListItemForeColor ?? System.Drawing.Color.FromArgb(26, 32, 44));
            else
                textColor = isSelected
                    ? (_theme?.OnPrimaryColor ?? System.Drawing.Color.White)
                    : (_theme?.ListItemForeColor ?? System.Drawing.Color.FromArgb(26, 32, 44));

            // Sub-text 2-line layout
            if (rich != null && !string.IsNullOrEmpty(rich.SubText))
            {
                int subH   = DpiScalingHelper.ScaleValue(16, _owner);
                int titleH = Math.Max(12, textRect.Height - subH - DpiScalingHelper.ScaleValue(ListBoxTokens.SubTextGap, _owner));
                var titleRect = new Rectangle(textRect.X, textRect.Y, textRect.Width, titleH);
                var subRect   = new Rectangle(textRect.X,
                    textRect.Y + titleH + DpiScalingHelper.ScaleValue(ListBoxTokens.SubTextGap, _owner),
                    textRect.Width, subH);
                DrawItemText(g, titleRect, item.Text, textColor, _owner.Font);
                DrawSubText(g, subRect, rich.SubText,
                    _owner.IsHighContrast ? _owner.HCItemForeground(isSelected) : (_theme?.ListItemForeColor ?? System.Drawing.Color.Gray),
                    _owner.Font);
            }
            else
            {
                DrawItemText(g, textRect, item.Text, textColor, _owner.Font);
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

            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, 3))
            {
                if (isSelected)
                {
                    var selColor = _theme?.PrimaryColor ?? Color.LightBlue;

                    // Selected background with subtle gradient
                    using (var brush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(30, selColor.R, selColor.G, selColor.B),
                        Color.FromArgb(10, selColor.R, selColor.G, selColor.B),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Selection border
                    using (var pen = new Pen(selColor, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    // Hover background
                    var hoverBg = _theme?.ListItemHoverBackColor ?? Color.FromArgb(248, 248, 248);
                    using (var brush = new SolidBrush(hoverBg))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new Pen(_theme?.AccentColor ?? Color.Gray, 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Normal state
                    var normalBg = _theme?.BackgroundColor ?? Color.White;
                    using (var brush = new SolidBrush(normalBg))
                    {
                        g.FillPath(brush, path);
                    }

                    // Normal border
                    using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(200, 200, 200), 0.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw subtle divider
            using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 0.5f))
            {
                g.DrawLine(pen, itemRect.Left + 8, itemRect.Bottom - 1, itemRect.Right - 8, itemRect.Bottom - 1);
            }
        }
    }
}
