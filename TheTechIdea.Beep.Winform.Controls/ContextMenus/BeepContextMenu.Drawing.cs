using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    public partial class BeepContextMenu
    {
        #region Paint Override
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (_menuItems == null)
            {
                return;
            }
            
            // Enable anti-aliasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Use current theme (kept in sync with BeepThemesManager)
            var theme = _currentTheme ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();
            var rect = new Rectangle(1, 1, _contentAreaRect.Width - 5, Height - 5);
            
            // Get effective control style
            var effectiveStyle = GetEffectiveControlStyle();
            
            // Apply clipping if scrolling is enabled
            if (_needsScrolling)
            {
                var clipRect = new Rectangle(1, 1, rect.Width - 5, rect.Height - 5);
                e.Graphics.SetClip(clipRect);
                
                // Translate graphics context by scroll offset
                e.Graphics.TranslateTransform(0, -_scrollOffset);
            }
            
            // Draw menu background+border with BeepStyling (outer frame/skin ONLY)
            var controlPath = BeepStyling.CreateControlStylePath(rect, effectiveStyle);
            BeepStyling.PaintControl(
                e.Graphics,
                controlPath,
                effectiveStyle,
                theme,
                _useThemeColors,
                ControlState.Normal,
                false
            )?.Dispose();
            controlPath.Dispose();

            // Draw each menu item simply
            DrawMenuItemsSimple(e.Graphics);

            // Reset transform before drawing border
            if (_needsScrolling)
            {
                e.Graphics.ResetTransform();
                e.Graphics.ResetClip();
            }
        }

        private void DrawMenuItemsSimple(Graphics g)
        {
            if (_menuItems == null || _menuItems.Count == 0)
                return;

            var contentBounds = _contentAreaRect;
            var itemHeight = PreferredItemHeight;
            var yOffset = 0;

            for (int i = 0; i < _menuItems.Count; i++)
            {
                var item = _menuItems[i];
                var itemRect = new Rectangle(
                    contentBounds.X + 4,
                    contentBounds.Y + yOffset + 4,
                    contentBounds.Width - 8,
                    itemHeight
                );

                if (_needsScrolling && (yOffset < _scrollOffset - itemHeight || yOffset > _scrollOffset + Height))
                {
                    yOffset += itemHeight;
                    continue;
                }

                if (item.DisplayField == "-" || item.Tag == "separator")
                {
                    DrawSeparatorSimple(g, itemRect);
                }
                else
                {
                    DrawMenuItemSimple(g, itemRect, item, i);
                }

                yOffset += itemHeight;
            }
        }

        private void DrawMenuItemSimple(Graphics g, Rectangle itemRect, SimpleItem item, int index)
        {
            bool isHovered = (_hoveredIndex == index);
            bool isSelected = (_selectedIndex == index);

            // Draw hover/selected highlight
            if (isHovered || isSelected)
            {
                var hoverColor = isSelected ? Color.FromArgb(90, 120, 230) : Color.FromArgb(220, 235, 255);
                using (var fillBrush = new SolidBrush(hoverColor))
                    g.FillRectangle(fillBrush, itemRect);
            }
            else
            {
                using (var fillBrush = new SolidBrush(Color.White))
                    g.FillRectangle(fillBrush, itemRect);
            }

            int iconPadding = 8;
            int textPadding = 8;
            int imageX = itemRect.X + iconPadding;

            // Draw image if enabled and available
            if (_showImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                try
                {
                    using (var img = Image.FromFile(item.ImagePath))
                    {
                        g.DrawImage(img, imageX, itemRect.Y + (itemRect.Height - img.Height) / 2, _imageSize, _imageSize);
                    }
                }
                catch { }
            }

            int textX = imageX + (_showImage && !string.IsNullOrEmpty(item.ImagePath) ? _imageSize + textPadding : 0);
            int textWidth = itemRect.Right - textX - textPadding;

            // Draw text
            using (var brush = new SolidBrush(Color.Black))
            using (var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
            {
                var safeFont = _textFont;
                g.DrawString(item.DisplayField ?? "", safeFont, brush, new Rectangle(textX, itemRect.Y, textWidth, itemRect.Height), format);
            }
        }

        private void DrawSeparatorSimple(Graphics g, Rectangle itemRect)
        {
            int sepY = itemRect.Top + itemRect.Height / 2;
            using (var pen = new Pen(Color.LightGray, 1))
                g.DrawLine(pen, itemRect.Left + 10, sepY, itemRect.Right - 10, sepY);
        }
        
        #endregion
    }
}
