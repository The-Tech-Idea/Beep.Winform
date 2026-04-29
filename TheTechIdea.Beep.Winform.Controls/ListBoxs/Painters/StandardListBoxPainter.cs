using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Standard list box painter - default Windows-like Style with enhanced borders and backgrounds
    /// </summary>
    internal class StandardListBoxPainter : BaseListBoxPainter
    {
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Use precomputed rects for best consistency
            var info      = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;
            Rectangle iconRect  = info?.IconRect  ?? Rectangle.Empty;
            Rectangle textRect  = info?.TextRect  ?? itemRect;

            // Checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawCheckbox(g, checkRect, isChecked, isHovered);
            }

            // Icon
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
                DrawItemImage(g, iconRect, item.ImagePath);

            // ── Rich item extras ──────────────────────────────────────────────────
            var rich      = item as BeepListItem;
            bool disabled = rich?.IsDisabled == true;

            // Determine text colour (HC-aware, disabled-aware)
            Color textColor;
            if (_owner.IsHighContrast)
                textColor = _owner.HCItemForeground(isSelected);
            else if (disabled)
                textColor = Color.FromArgb(ListBoxTokens.DisabledAlpha, _helper.GetTextColor());
            else
                textColor = isSelected
                    ? (_theme?.OnPrimaryColor ?? Color.White)
                    : _helper.GetTextColor();

            if (textColor == Color.Empty) textColor = _helper.GetTextColor();

            // Badge pill — draw before text so it can shrink textRect
            if (rich != null && !string.IsNullOrEmpty(rich.BadgeText))
            {
                int badgePad = Scale(72);
                DrawBadgePill(g, itemRect, rich.BadgeText, rich.BadgeColor);
                textRect = new Rectangle(textRect.X, textRect.Y,
                    textRect.Width - badgePad, textRect.Height);
            }

            // Sub-text 2-line layout
            if (rich != null && !string.IsNullOrEmpty(rich.SubText))
            {
                int subH = Scale(16);
                int titleH = textRect.Height - subH - Scale(ListBoxTokens.SubTextGap);
                titleH = Math.Max(12, titleH);

                var titleRect = new Rectangle(textRect.X, textRect.Y, textRect.Width, titleH);
                var subRect   = new Rectangle(textRect.X,
                    textRect.Y + titleH + Scale(ListBoxTokens.SubTextGap),
                    textRect.Width, subH);

                DrawItemText(g, titleRect, item.Text, textColor, _owner.TextFont);
                DrawSubText(g, subRect, rich.SubText,
                    _owner.IsHighContrast ? _owner.HCItemForeground(isSelected) : _helper.GetTextColor(),
                    _owner.TextFont);
            }
            else
            {
                DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
            }

            // Focus ring (keyboard-navigated item)
            var visible = _helper?.GetVisibleItems();
            int fi = _owner.FocusedIndex;
            if (fi >= 0 && visible != null && fi < visible.Count && visible[fi] == item)
                DrawFocusRing(g, itemRect);
        }
        
        // Enhanced with border style and gradient background
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            int cornerRadius = Scale(ListBoxTokens.SelectionCornerRadius);
            float selBorder = Scale(2);
            float hoverBorder = Scale(1);
            float defaultBorder = 0.5f;

            // Create rounded rectangle path for modern look
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, cornerRadius))
            {
                // Draw background with gradient based on state
                if (isSelected)
                {
                    var selColor = _theme?.PrimaryColor ?? Color.LightBlue;
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(itemRect, 
                        Color.FromArgb(100, selColor.R, selColor.G, selColor.B),
                        Color.FromArgb(60, selColor.R, selColor.G, selColor.B),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Selection border
                    using (var pen = new System.Drawing.Pen(_theme?.PrimaryColor ?? Color.Blue, selBorder))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    var hoverColor = _theme?.ListItemHoverBackColor ?? Color.FromArgb(240, 240, 240);
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(itemRect,
                        Color.FromArgb(50, hoverColor.R, hoverColor.G, hoverColor.B),
                        Color.FromArgb(20, hoverColor.R, hoverColor.G, hoverColor.B),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new System.Drawing.Pen(_theme?.AccentColor ?? Color.LightGray, hoverBorder))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Default background
                    using (var brush = new System.Drawing.SolidBrush(_theme?.BackgroundColor ?? Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    // Default subtle border
                    using (var pen = new System.Drawing.Pen(Color.FromArgb(200, 200, 200), defaultBorder))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }
}
