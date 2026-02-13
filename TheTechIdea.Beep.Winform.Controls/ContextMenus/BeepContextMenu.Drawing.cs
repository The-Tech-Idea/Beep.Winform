using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    public partial class BeepContextMenu
    {
        #region Paint Override
        
        /// <summary>
        /// Override OnPaintBackground to handle custom background painting.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Skip base class painting - we handle everything in OnPaint using BeepStyling
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;
            // Don't call base.OnPaintBackground - we paint our own styled background in OnPaint
        }
        
        /// <summary>
        /// Override OnResize to recalculate region when form size changes
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Recalculate region when size changes to maintain rounded corners
            if (ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                Invalidate(); // This will trigger OnPaint which will update the region
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            // BeepContextMenu handles all painting itself using BeepStyling
            
            if (_menuItems == null || ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }
            
            // Enable anti-aliasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Use current theme (kept in sync with BeepThemesManager)
            var theme = _currentTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;
            var effectiveStyle = ControlStyle;
            // Get effective control style - use _contextMenuType (our local FormStyle field)
            if (_contextMenuType != ThemeManagement.BeepThemesManager.CurrentStyle)
            {
                effectiveStyle = BeepStyling.GetControlStyle(ThemeManagement.BeepThemesManager.CurrentStyle);
            }
            ControlStyle = effectiveStyle;
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
            
            // Draw menu background+border+shadow with BeepStyling (outer frame/skin ONLY)
            var controlPath = BeepStyling.CreateControlStylePath(outerRect, effectiveStyle);
            
            // Set form region to clip to rounded path (hides sharp black corners)
            UpdateFormRegion(controlPath, e.Graphics);
            
            BeepStyling.PaintControl(
                e.Graphics,
                controlPath,
                effectiveStyle,
                theme,
                true,
                ControlState.Normal,
                false
            )?.Dispose();
            controlPath.Dispose();

            // Clip and paint the content area independently so scrolling does not move the frame
            var contentClip = new Rectangle(
                beepInsets,
                beepInsets,
                Width - (beepInsets * 2) - (_scrollBar.Visible ? SCROLL_BAR_WIDTH : 0),
                Height - (beepInsets * 2));

            var itemState = e.Graphics.Save();
            e.Graphics.SetClip(contentClip);

            // Always clear the content area to the menu background to avoid ghosting while scrolling
            var menuBgColor = theme?.MenuBackColor ?? Color.White;
            using (var bgBrush = new SolidBrush(menuBgColor))
            {
                e.Graphics.FillRectangle(bgBrush, contentClip);
            }

            // Draw each menu item
            DrawMenuItemsSimple(e.Graphics, beepInsets);

            // Restore graphics state (clip + transform) before exiting paint
            e.Graphics.Restore(itemState);
        }

        private void DrawMenuItemsSimple(Graphics g, int beepInsets)
        {
            if (_menuItems == null || _menuItems.Count == 0)
                return;
            int internalPadding = GetInternalPadding();
            int searchSpacing = GetSearchSpacing();
            int contentStartX = beepInsets + internalPadding;
            int contentStartY = beepInsets + internalPadding;
            int contentWidth = Width - (beepInsets * 2) - (internalPadding * 2) - (_scrollBar.Visible ? SCROLL_BAR_WIDTH : 0);
            int searchAreaHeight = _showSearchBox ? (_searchTextBox != null ? _searchTextBox.Height : ScaleLogical(DefaultSearchBoxHeightLogical)) : 0;
            if (searchAreaHeight > 0)
            {
                // Draw or position the actual search textbox control
                if (_searchTextBox != null)
                {
                    try
                    {
                        _searchTextBox.Left = contentStartX;
                        _searchTextBox.Top = contentStartY + internalPadding;
                        _searchTextBox.Width = Math.Max(100, Width - (beepInsets * 2) - (internalPadding * 2) - (_scrollBar.Visible ? SCROLL_BAR_WIDTH : 0));
                        _searchTextBox.Visible = true;
                        _searchTextBox.BringToFront();
                    }
                    catch { }
                }
                contentStartY += searchAreaHeight + searchSpacing;
            }
            var yOffset = 0;
            int scroll = _needsScrolling ? _scrollOffset : 0;
            for (int i = 0; i < _menuItems.Count; i++)
            {
                var item = _menuItems[i];
                var itemHeight = (int)PreferredItemHeight; // default
                if (item != null) itemHeight = GetItemHeight(item);
                int itemY = contentStartY + yOffset - scroll;
                var itemRect = new Rectangle(contentStartX, itemY, contentWidth, itemHeight);

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

            // Get theme colors directly from current theme
            var theme = _currentTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;
            
            // Always use theme colors if available, fallback to defaults
            var menuBgColor = theme?.MenuBackColor ?? Color.White;
            var selectedBgColor = theme?.MenuItemSelectedBackColor ?? Color.FromArgb(90, 120, 230);
            var hoverBgColor = theme?.MenuItemHoverBackColor ?? Color.FromArgb(220, 235, 255);

            // Determine if background is dark or light to adjust text contrast
            bool isDarkBackground = IsDarkColor(menuBgColor);
            var textColor = theme?.MenuItemForeColor ?? Color.Black;
            
            // If text and background don't have enough contrast, flip the text color
            if (isDarkBackground && IsDarkColor(textColor))
            {
                // Dark background with dark text = poor contrast, use light text
                textColor = Color.White;
            }
            else if (!isDarkBackground && !IsDarkColor(textColor))
            {
                // Light background with light text = poor contrast, use dark text
                textColor = Color.Black;
            }

            // Draw hover/selected highlight
            if (isHovered || isSelected)
            {
                var highlightColor = isSelected ? selectedBgColor : hoverBgColor;
                using (var fillBrush = new SolidBrush(highlightColor))
                    g.FillRectangle(fillBrush, itemRect);
            }
            else
            {
                using (var fillBrush = new SolidBrush(menuBgColor))
                    g.FillRectangle(fillBrush, itemRect);
            }

            // Get effective control style for image painting
            var effectiveStyle = ControlStyle;
            
            int iconPadding = ScaleLogical(8);
            int textPadding = ScaleLogical(8);
            int imageX = itemRect.X + iconPadding;
            
            // Reserve space for arrow if item has children
            int arrowWidth = hasChildren ? ScaleLogical(20) : 0;
            
            // Reserve space for shortcut text if present
            int shortcutWidth = 0;
            if (hasShortcut)
            {
                SizeF shortcutSizeF = TextUtils.MeasureText(item.ShortcutText, _textFont, int.MaxValue);
                var shortcutSize = new Size((int)shortcutSizeF.Width, (int)shortcutSizeF.Height);
                shortcutWidth = shortcutSize.Width + ScaleLogical(16);
            }

            // Calculate layout areas for image and text
            int imageAreaWidth = (_showImage && !string.IsNullOrEmpty(item.ImagePath)) ? _imageSize + ScaleLogical(8) : 0;
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

            // Draw text and optional subtext
            var textRect = new Rectangle(textStartX, itemRect.Y, textWidth, itemRect.Height);
            var safeFont = _textFont;
            if (!string.IsNullOrEmpty(item.SubText))
            {
                // Two-line layout: size from actual font metrics to avoid overlap at high DPI.
                int titleHeight = MeasureFontHeight(safeFont);
                int subTextHeight = MeasureFontHeight(_shortcutFont ?? safeFont);
                int lineGap = ScaleLogical(2);
                int textBlockHeight = titleHeight + subTextHeight + lineGap;
                int textStartY = itemRect.Y + Math.Max(0, (itemRect.Height - textBlockHeight) / 2);

                var titleRect = new Rectangle(textStartX, textStartY, textWidth, titleHeight);
                var subRect = new Rectangle(textStartX, textStartY + titleHeight + lineGap, textWidth, subTextHeight);
                TextRenderer.DrawText(g, item.DisplayField ?? "", safeFont, titleRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                
                // Muted subtext color from theme (50% opacity)
                Color subColor = Color.Gray;
                if (theme?.MenuItemForeColor != null)
                {
                    subColor = Color.FromArgb(160, theme.MenuItemForeColor);
                }
                
                TextRenderer.DrawText(g, item.SubText ?? "", _shortcutFont, subRect, subColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
            else
            {
                TextRenderer.DrawText(g, item.DisplayField ?? "", safeFont, textRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
            
            // Draw shortcut text if present (right-aligned, before arrow)
            if (hasShortcut)
            {
                var shortcutColor = Color.Gray;
                if (theme?.MenuItemForeColor != null)
                {
                    // 50% opacity of forecolor for shortcut hint
                    shortcutColor = Color.FromArgb(128, theme.MenuItemForeColor);
                }
                    
                var shortcutRect = new Rectangle(
                    itemRect.Right - arrowWidth - shortcutWidth,
                    itemRect.Y,
                    shortcutWidth - ScaleLogical(8),
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
            int arrowSize = ScaleLogical(8);
            int arrowX = itemRect.Right - ScaleLogical(12);
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
            var theme = _currentTheme ?? ThemeManagement.BeepThemesManager.CurrentTheme;
            var separatorColor = theme?.BorderColor ?? Color.LightGray;
            
            int sepY = itemRect.Top + itemRect.Height / 2;
            int sidePadding = ScaleLogical(10);
            using (var pen = new Pen(separatorColor, Math.Max(1f, _scaleFactor)))
                g.DrawLine(pen, itemRect.Left + sidePadding, sepY, itemRect.Right - sidePadding, sepY);
        }

        /// <summary>
        /// Determines if a color is considered "dark" (luminance less than 128)
        /// Used for contrast checking to ensure text is readable
        /// </summary>
        private bool IsDarkColor(Color color)
        {
            // Standard luminance formula: (0.299 * R + 0.587 * G + 0.114 * B)
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
            return luminance < 128; // Threshold: 0-255 scale
        }
        
        /// <summary>
        /// Updates the form's Region to clip to the rounded path, hiding sharp corners
        /// </summary>
        private void UpdateFormRegion(GraphicsPath controlPath, Graphics g)
        {
            if (controlPath == null || g == null || ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;
            
            try
            {
                // Dispose old region if it exists (properly clean up before creating new one)
                if (Region != null)
                {
                    var oldRegion = Region;
                    Region = null; // Clear reference first to prevent issues
                    oldRegion.Dispose();
                }
                
                // Create region from the rounded path to clip the form
                // This ensures sharp corners are hidden when rounded borders are used
                Region = new Region(controlPath);
            }
            catch
            {
                // If region creation fails, continue without clipping
                // This can happen if the path is invalid or too complex
                // Reset to null to ensure no invalid region is set
                if (Region != null)
                {
                    try
                    {
                        var oldRegion = Region;
                        Region = null;
                        oldRegion.Dispose();
                    }
                    catch { }
                }
            }
        }
        
        #endregion
    }
}
