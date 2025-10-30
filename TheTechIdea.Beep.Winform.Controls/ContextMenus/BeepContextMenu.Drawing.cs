using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

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
            
            // Validate content area rect - may be empty on first paint
            int contentWidth = _contentAreaRect.Width > 0 ? _contentAreaRect.Width : Width;
            int contentHeight = Height;
            
            // Ensure valid dimensions
            if (contentWidth <= 0 || contentHeight <= 0)
            {
                return; // Skip painting if invalid dimensions
            }
            
            var rect = new Rectangle(1, 1, Math.Max(1, contentWidth - 5), Math.Max(1, contentHeight - 5));
            
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
            
            // Use BeepStyling system
            // Create GraphicsPath for the context menu bounds
            var controlPath = BeepStyling.CreateControlStylePath(rect, effectiveStyle);
            
            // Paint using BeepStyling system
            var contentPath = BeepStyling.PaintControl(
                e.Graphics, 
                controlPath, 
                effectiveStyle, 
                theme, 
                _useThemeColors, 
                ControlState.Normal,
                false // Not transparent background
            );
            
            // Draw menu items in the content area
            if (contentPath != null)
            {
                DrawMenuItemsWithBeepStyling(e.Graphics, contentPath, effectiveStyle, theme);
                contentPath.Dispose();
            }
            
            controlPath.Dispose();
            
            // Reset transform before drawing border
            if (_needsScrolling)
            {
                e.Graphics.ResetTransform();
                e.Graphics.ResetClip();
            }
        }

        /// <summary>
        /// Draws menu items using BeepStyling system
        /// </summary>
        private void DrawMenuItemsWithBeepStyling(Graphics g, GraphicsPath contentPath, BeepControlStyle style, IBeepTheme theme)
        {
            if (_menuItems == null || _menuItems.Count == 0)
                return;

            var contentBounds = contentPath.GetBounds();
            var itemHeight = PreferredItemHeight;
            var yOffset = 0;

            for (int i = 0; i < _menuItems.Count; i++)
            {
                var item = _menuItems[i];
                var itemRect = new Rectangle(
                    (int)contentBounds.X + 4,
                    (int)contentBounds.Y + yOffset + 4,
                    (int)contentBounds.Width - 8,
                    itemHeight
                );

                // Skip if item is outside visible area (for scrolling)
                if (_needsScrolling && (yOffset < _scrollOffset - itemHeight || yOffset > _scrollOffset + Height))
                {
                    yOffset += itemHeight;
                    continue;
                }

                // Draw separator
                if (item.DisplayField == "-" || item.Tag == "separator")
                {
                    DrawSeparatorWithBeepStyling(g, itemRect, style, theme);
                }
                else
                {
                    // Draw menu item
                    DrawMenuItemWithBeepStyling(g, itemRect, item, i, style, theme);
                }

                yOffset += itemHeight;
            }
        }

        /// <summary>
        /// Draws a single menu item using BeepStyling
        /// </summary>
        private void DrawMenuItemWithBeepStyling(Graphics g, Rectangle itemRect, SimpleItem item, int index, BeepControlStyle style, IBeepTheme theme)
        {
            bool isHovered = (_hoveredIndex == index);
            bool isSelected = (_selectedIndex == index);

            // Create path for item
            var itemPath = BeepStyling.CreateControlStylePath(itemRect, style);

            // Determine item state
            var itemState = ControlState.Normal;
            if (isSelected) itemState = ControlState.Selected;
            else if (isHovered) itemState = ControlState.Hovered;

            // Paint item background using BeepStyling
            var itemContentPath = BeepStyling.PaintControl(
                g, 
                itemPath, 
                style, 
                theme, 
                _useThemeColors, 
                itemState,
                false
            );

            // Draw item content
            if (itemContentPath != null)
            {
                // Calculate layout areas
                int checkboxAreaWidth = _showCheckBox ? 20 : 0;
                int imageAreaWidth = _showImage && !string.IsNullOrEmpty(item.ImagePath) ? _imageSize + 8 : 0;
                int textStartX = itemRect.X + 8 + checkboxAreaWidth + imageAreaWidth;
                int textWidth = itemRect.Width - 16 - checkboxAreaWidth - imageAreaWidth;

                // Draw checkbox if enabled
                if (_showCheckBox)
                {
                    var checkboxRect = new Rectangle(
                        itemRect.X + 8,
                        itemRect.Y + (itemRect.Height - 16) / 2,
                        16,
                        16
                    );

                    // Create path for checkbox area
                    var checkboxPath = BeepStyling.CreateControlStylePath(checkboxRect, style);
                    
                    // Paint checkbox background
                    var checkboxContentPath = BeepStyling.PaintControl(
                        g, 
                        checkboxPath, 
                        style, 
                        theme, 
                        _useThemeColors, 
                        itemState,
                        false
                    );

                    // Draw checkmark if item is checked
                    if (item.IsChecked)
                    {
                        DrawCheckmark(g, checkboxRect, style, theme);
                    }

                    if (checkboxContentPath != null)
                        checkboxContentPath.Dispose();
                    checkboxPath.Dispose();
                }

                // Draw image if enabled and available
                if (_showImage && !string.IsNullOrEmpty(item.ImagePath))
                {
                    var imageRect = new Rectangle(
                        itemRect.X + 8 + checkboxAreaWidth,
                        itemRect.Y + (itemRect.Height - _imageSize) / 2,
                        _imageSize,
                        _imageSize
                    );

                    // Create path for image area
                    var imagePath = BeepStyling.CreateControlStylePath(imageRect, style);
                    
                    // Paint image using StyledImagePainter
                    BeepStyling.PaintStyleImage(g, imagePath, item.ImagePath, style);
                    
                    imagePath.Dispose();
                }

                // Draw text
                var textRect = new Rectangle(
                    textStartX,
                    itemRect.Y,
                    textWidth,
                    itemRect.Height
                );

                var textColor = _useThemeColors && theme != null ? theme.ForeColor : BeepStyling.GetForegroundColor(style);
                var brush = new SolidBrush(textColor);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                // Validate font before drawing
                Font useFont = _textFont;
                if (!IsValidFont(useFont))
                {
                    useFont = new Font("Segoe UI", 9f, FontStyle.Regular);
                }

                g.DrawString(item.DisplayField, useFont, brush, textRect, format);
                brush.Dispose();

                itemContentPath.Dispose();
            }

            itemPath.Dispose();
        }

        /// <summary>
        /// Draws a separator using BeepStyling
        /// </summary>
        private void DrawSeparatorWithBeepStyling(Graphics g, Rectangle itemRect, BeepControlStyle style, IBeepTheme theme)
        {
            var separatorColor = _useThemeColors && theme != null ? theme.BorderColor : BeepStyling.GetBorderColor(style);
            var pen = new Pen(separatorColor, 1);
            
            var y = itemRect.Y + itemRect.Height / 2;
            g.DrawLine(pen, itemRect.X + 8, y, itemRect.Right - 8, y);
            
            pen.Dispose();
        }

        /// <summary>
        /// Draws a checkmark in the checkbox
        /// </summary>
        private void DrawCheckmark(Graphics g, Rectangle checkboxRect, BeepControlStyle style, IBeepTheme theme)
        {
            var checkmarkColor = _useThemeColors && theme != null ? theme.ForeColor : BeepStyling.GetForegroundColor(style);
            var pen = new Pen(checkmarkColor, 2);
            
            // Draw checkmark using lines
            int x = checkboxRect.X + 3;
            int y = checkboxRect.Y + checkboxRect.Height / 2;
            int size = 8;
            
            // Draw checkmark lines
            g.DrawLine(pen, x, y, x + size / 3, y + size / 3);
            g.DrawLine(pen, x + size / 3, y + size / 3, x + size, y - size / 3);
            
            pen.Dispose();
        }
        
        #endregion
    }
}
