using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Multi-select dropdown with chips/pills for selected items
    /// </summary>
    internal class MultiSelectChipsPainter : OutlinedComboBoxPainter
    {
        protected override void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;

            var selectedItems = _owner.SelectedItems;
            var animating = _owner.GetAnimatingChips();
            var combined = new System.Collections.Generic.List<SimpleItem>();
            if (selectedItems != null) combined.AddRange(selectedItems);
            if (animating != null)
            {
                foreach (var a in animating)
                {
                    if (!combined.Contains(a)) combined.Add(a);
                }
            }
            if (combined == null || combined.Count == 0)
            {
                base.DrawText(g, textAreaRect);
                return;
            }

            int chipX = textAreaRect.X + 4;
            int chipY = textAreaRect.Y + (textAreaRect.Height - 24) / 2;
            int maxDisplay = Math.Max(1, _owner.MaxDisplayChips);
            int shown = 0;

            foreach (var item in combined)
            {
                if (shown >= maxDisplay) break;
                var progress = _owner.GetChipAnimationProgress(item);
                // Apply theme-defined easing if any
                var themeEasing = _owner._currentTheme?.AnimationEasingFunction;
                progress = TheTechIdea.Beep.Winform.Controls.Helpers.AnimationEasingHelper.Evaluate(themeEasing, progress);
                var chipSize = DrawChip(g, item.Text, chipX, chipY, progress);
                chipX += chipSize.Width + 6;
                shown++;
            }

            // Draw extra counter if more items selected
            if ((selectedItems?.Count ?? 0) > maxDisplay)
            {
                string more = $"+{selectedItems.Count - maxDisplay}";
                var sz = System.Windows.Forms.TextRenderer.MeasureText(more, _owner.TextFont);
                var rect = new Rectangle(chipX, chipY, sz.Width + 16, 24);
                // Draw rounded background
                using (var path = GraphicsExtensions.CreateRoundedRectanglePath(rect, 12))
                using (var brush = new System.Drawing.SolidBrush(_theme?.PrimaryColor ?? Color.LightGray))
                {
                    g.FillPath(brush, path);
                }
                TextRenderer.DrawText(g, more, _owner.TextFont, rect, _theme?.OnPrimaryColor ?? Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private Size DrawChip(Graphics g, string text, int x, int y, float progress)
        {
            var textSize = System.Windows.Forms.TextRenderer.MeasureText(text, _owner.TextFont);
            int chipWidth = textSize.Width + 28;
            int chipHeight = 24;

            Rectangle chipRect = new Rectangle(x, y, chipWidth, chipHeight);
            // Apply simple scale + alpha animation based on progress (0..1)
            float scale = 0.85f + 0.15f * Math.Max(0f, Math.Min(1f, progress));
            int scaledW = (int)(chipRect.Width * scale);
            int scaledH = (int)(chipRect.Height * scale);
            var scaledRect = new Rectangle(chipRect.X, chipRect.Y + (chipRect.Height - scaledH) / 2, scaledW, scaledH);
            var bgColor = _theme?.PrimaryColor ?? Color.Empty;
            int alpha = (int)(255 * Math.Max(0.35f, Math.Min(1f, progress)));
            var colorWithAlpha = Color.FromArgb(alpha, bgColor.R, bgColor.G, bgColor.B);
            // Draw background
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(scaledRect, 12))
            using (var brush = new System.Drawing.SolidBrush(colorWithAlpha))
            {
                g.FillPath(brush, path);
            }

            // Draw text
            var textRect = new Rectangle(scaledRect.X + 10, scaledRect.Y, scaledRect.Width - 20, scaledRect.Height);
            var textColor = _theme?.OnPrimaryColor ?? Color.White;
            var textAlpha = (int)(alpha); // same alpha
            var textColorAlpha = Color.FromArgb(textAlpha, textColor.R, textColor.G, textColor.B);
            TextRenderer.DrawText(g, text, _owner.TextFont, textRect, textColorAlpha, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            return new Size(scaledW, scaledH);
        }
    }
    
    /// <summary>
    /// Dropdown with integrated search functionality
    /// </summary>
    internal class SearchableDropdownPainter : OutlinedComboBoxPainter
    {
        // TODO: Add search icon indicator
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // Draw search icon instead of dropdown arrow
            Color iconColor = _theme?.SecondaryColor ?? Color.Gray;
            
            // Simple magnifying glass representation
            var centerX = buttonRect.Left + buttonRect.Width / 2;
            var centerY = buttonRect.Top + buttonRect.Height / 2;
            var radius = Math.Min(buttonRect.Width, buttonRect.Height) / 4;
            
            var pen = PaintersFactory.GetPen(iconColor, 1.5f);
            g.DrawEllipse(pen, centerX - radius, centerY - radius - 2, radius * 2, radius * 2);
            g.DrawLine(pen, centerX + radius - 2, centerY + radius - 2, 
                      centerX + radius + 2, centerY + radius + 2);
        }
    }
    
    /// <summary>
    /// Dropdown with icons displayed next to items
    /// </summary>
    internal class WithIconsComboBoxPainter : OutlinedComboBoxPainter
    {
        // TODO: Reserve space for icons in layout
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(40, 6, 8, 6); // Extra left padding for icons
        }
    }
    
    /// <summary>
    /// Menu-Style dropdown with categories/sections
    /// </summary>
    internal class MenuComboBoxPainter : OutlinedComboBoxPainter
    {
        // TODO: Add section dividers in dropdown
        // For now, uses outlined Style
    }
    
    /// <summary>
    /// Country selector dropdown with flags
    /// </summary>
    internal class CountrySelectorPainter : WithIconsComboBoxPainter
    {
        // Inherits icon space from WithIconsComboBoxPainter
        // TODO: Add flag rendering support
    }
}
