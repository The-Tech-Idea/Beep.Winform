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
            
            // Get effective control style
            var effectiveStyle = ControlStyle;
            
            // CRITICAL: Calculate BeepStyling insets (padding + border + shadow) to match RecalculateSize()
            int beepPadding = BeepStyling.GetPadding(effectiveStyle);
            float beepBorderWidth = BeepStyling.GetBorderThickness(effectiveStyle);
            int beepShadow = Styling.Shadows.StyleShadows.HasShadow(effectiveStyle) 
                ? Math.Max(2, Styling.Shadows.StyleShadows.GetShadowBlur(effectiveStyle) / 2) 
                : 0;
            
            // Total insets on each side (this is what RecalculateSize uses)
            int beepInsets = beepPadding + (int)Math.Ceiling(beepBorderWidth) + beepShadow;
            
            // Create the outer rectangle for the control (accounting for shadow offset)
            // Shadow is drawn OUTSIDE the control, so we need to leave space for it
            var outerRect = new Rectangle(
                beepShadow, 
                beepShadow, 
                Width - (beepShadow * 2), 
                Height - (beepShadow * 2)
            );
            
            // Apply clipping if scrolling is enabled
            // Clip to content area (inside padding and border)
            if (_needsScrolling)
            {
                var clipRect = new Rectangle(beepInsets, beepInsets,
     Width - (beepInsets * 2) - (_scrollBar.Visible ? SCROLL_BAR_WIDTH : 0),
     Height - (beepInsets * 2));
                e.Graphics.SetClip(clipRect);
                
                // Translate graphics context by scroll offset
                e.Graphics.TranslateTransform(0, -_scrollOffset);
            }
            
            // Draw menu background+border+shadow with BeepStyling (outer frame/skin ONLY)
            var controlPath = BeepStyling.CreateControlStylePath(outerRect, effectiveStyle);
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
            DrawMenuItemsSimple(e.Graphics, beepInsets);

            // Reset transform before drawing border
            if (_needsScrolling)
            {
                e.Graphics.ResetTransform();
                e.Graphics.ResetClip();
            }
        }

        private void DrawMenuItemsSimple(Graphics g, int beepInsets)
        {
            if (_menuItems == null || _menuItems.Count == 0)
                return;

            var itemHeight = PreferredItemHeight;

            int contentStartX = beepInsets + 4;
            int contentStartY = beepInsets + 4;
            int contentWidth = Width - (beepInsets * 2) - 8 - (_scrollBar.Visible ? SCROLL_BAR_WIDTH : 0);
            var yOffset = 0;

            for (int i = 0; i < _menuItems.Count; i++)
            {
                var item = _menuItems[i];
                var itemRect = new Rectangle(
                    contentStartX,
                    contentStartY + yOffset,
                    contentWidth,
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
            bool isHovered = (_hoveredItem ==  item);
            bool isSelected = (_selectedIndex == index);
            bool hasChildren = ContextMenuManager.HasChildren(item);
            bool hasShortcut = _showShortcuts && !string.IsNullOrEmpty(item.ShortcutText);

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

            // Get effective control style for image painting
            var effectiveStyle = ControlStyle;
            
            int iconPadding = 8;
            int textPadding = 8;
            int imageX = itemRect.X + iconPadding;
            
            // Reserve space for arrow if item has children
            int arrowWidth = hasChildren ? 20 : 0;
            
            // Reserve space for shortcut text if present
            int shortcutWidth = 0;
            if (hasShortcut)
            {
                var shortcutSize = TextRenderer.MeasureText(item.ShortcutText, _textFont);
                shortcutWidth = shortcutSize.Width + 16; // 16px padding (8px each side)
            }

            // Calculate layout areas for image and text
            int imageAreaWidth = (_showImage && !string.IsNullOrEmpty(item.ImagePath)) ? _imageSize + 8 : 0;
            int textStartX = itemRect.X + iconPadding + imageAreaWidth;
            int textWidth = itemRect.Width - iconPadding - imageAreaWidth - textPadding - arrowWidth - shortcutWidth;

            // Draw image if enabled and available using BeepStyling (same as BeepMenuBar)
            if (_showImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                try
                {
                    var imageRect = new Rectangle(
                        imageX,
                        itemRect.Y + (itemRect.Height - _imageSize) / 2,
                        _imageSize,
                        _imageSize
                    );

                    // Create path for image area (rounded rectangle matching style)
                    var imagePath = BeepStyling.CreateControlStylePath(imageRect, effectiveStyle);
                    
                    // Paint image using StyledImagePainter (same as BeepMenuBar)
                    BeepStyling.PaintStyleImage(g, imagePath, item.ImagePath, effectiveStyle);
                    
                    imagePath.Dispose();
                }
                catch { }
            }

            // Draw text (same approach as BeepMenuBar - use TextRenderer for safety)
            var textColor = UseThemeColors && _currentTheme != null ? _currentTheme.MenuItemForeColor : Color.Black;
            var textRect = new Rectangle(textStartX, itemRect.Y, textWidth, itemRect.Height);
            var safeFont = _textFont;
            
            TextRenderer.DrawText(g, item.DisplayField ?? "", safeFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            
            // Draw shortcut text if present (right-aligned, before arrow)
            if (hasShortcut)
            {
                var shortcutColor = UseThemeColors && _currentTheme != null 
                    ? Color.FromArgb(128, _currentTheme.MenuItemForeColor) // 50% opacity
                    : Color.Gray;
                    
                var shortcutRect = new Rectangle(
                    itemRect.Right - arrowWidth - shortcutWidth,
                    itemRect.Y,
                    shortcutWidth - 8, // Remove padding for text
                    itemRect.Height
                );
                
                TextRenderer.DrawText(g, item.ShortcutText, safeFont, shortcutRect, shortcutColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
            
            // Draw arrow indicator if item has children (drill-down)
            if (hasChildren)
            {
                DrawSubMenuArrow(g, itemRect, textColor);
            }
        }
        
        /// <summary>
        /// Draws a right-pointing arrow to indicate sub-menu availability
        /// </summary>
        private void DrawSubMenuArrow(Graphics g, Rectangle itemRect, Color arrowColor)
        {
            // Calculate arrow position (right side of item)
            int arrowSize = 8;
            int arrowX = itemRect.Right - 12;
            int arrowY = itemRect.Y + (itemRect.Height - arrowSize) / 2;
            
            // Draw a simple right-pointing triangle
            var arrowPoints = new Point[]
            {
                new Point(arrowX, arrowY),                          // Top
                new Point(arrowX, arrowY + arrowSize),              // Bottom
                new Point(arrowX + arrowSize / 2, arrowY + arrowSize / 2)  // Right point
            };
            
            using (var arrowBrush = new SolidBrush(arrowColor))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillPolygon(arrowBrush, arrowPoints);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
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
