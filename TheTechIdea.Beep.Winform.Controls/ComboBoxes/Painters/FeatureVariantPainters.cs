using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

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
                SizeF szF = TextUtils.MeasureText(more, _owner.TextFont, int.MaxValue);
                var sz = new Size((int)szF.Width, (int)szF.Height);
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
            SizeF textSizeF = TextUtils.MeasureText(text, _owner.TextFont, int.MaxValue);
            var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
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
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;
            
            // Draw search icon instead of dropdown arrow using SVG icon
            Color iconColor = _theme?.SecondaryColor ?? Color.Gray;
            
            try
            {
                // Use StyledImagePainter to render search icon from IconsManagement
                // Apply theme color tinting for better integration
                StyledImagePainter.PaintWithTint(g, buttonRect, SvgsUI.Search, iconColor, 1.0f, 0);
            }
            catch
            {
                // Fallback to simple magnifying glass if icon fails to load
                var centerX = buttonRect.Left + buttonRect.Width / 2;
                var centerY = buttonRect.Top + buttonRect.Height / 2;
                var radius = Math.Min(buttonRect.Width, buttonRect.Height) / 4;
                
                var pen = PaintersFactory.GetPen(iconColor, 1.5f);
                g.DrawEllipse(pen, centerX - radius, centerY - radius - 2, radius * 2, radius * 2);
                g.DrawLine(pen, centerX + radius - 2, centerY + radius - 2, 
                          centerX + radius + 2, centerY + radius + 2);
            }
        }
    }
    
    /// <summary>
    /// Dropdown with icons displayed next to items
    /// </summary>
    internal class WithIconsComboBoxPainter : OutlinedComboBoxPainter
    {
        /// <summary>
        /// Returns padding that reserves space for item icons (24px default icon width + 16px spacing)
        /// Icons are rendered by BeepContextMenu in the dropdown items using SimpleItem.ImagePath
        /// </summary>
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(40, 6, 8, 6); // Extra left padding for icons (24px icon + 16px spacing)
        }
        
        /// <summary>
        /// Gets the preferred icon width for item icons in dropdown
        /// </summary>
        public virtual int GetPreferredIconWidth()
        {
            return 24; // Default icon size for items
        }
    }
    
    /// <summary>
    /// Menu-Style dropdown with categories/sections
    /// </summary>
    internal class MenuComboBoxPainter : OutlinedComboBoxPainter
    {
        /// <summary>
        /// Menu-style combo box with section dividers support.
        /// Section dividers are rendered by BeepContextMenu in the dropdown.
        /// Items can be grouped using SimpleItem.Category or SimpleItem.Children for hierarchical grouping.
        /// BeepContextMenu should detect groups and render dividers between sections.
        /// </summary>
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(8, 6, 8, 6);
        }
    }
    
    /// <summary>
    /// Country selector dropdown with flags
    /// </summary>
    internal class CountrySelectorPainter : WithIconsComboBoxPainter
    {
        /// <summary>
        /// Country selector with flag rendering support.
        /// Flags are rendered by BeepContextMenu in dropdown items using SimpleItem.ImagePath.
        /// Assign flag image/SVG paths to SimpleItem.ImagePath when creating country items.
        /// Inherits icon layout space from WithIconsComboBoxPainter (40px left padding).
        /// </summary>
        public override int GetPreferredIconWidth()
        {
            return 20; // Slightly smaller for flags (20x20 or 16x16 recommended)
        }
    }
}
