using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Painting
        
        /// <summary>
        /// Handles tab transition animation rendering
        /// </summary>
        private void HandleTabTransition(Graphics g)
        {
            // Disabled to prevent crashes - simple immediate switching instead
        }

        /// <summary>
        /// Draws the content area background with proper theme colors
        /// </summary>
        private void DrawContentAreaBackground(Graphics g)
        {
            if (g == null || _contentArea.IsEmpty) return;
            
            // Skip if transparent
            if (IsTransparentBackground) return;
            
            // Get effective background color
            Color bgColor = GetEffectiveContentBackColor();
            
            // Draw content area background
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, _contentArea);
            }
        }
        
        /// <summary>
        /// Draws the tab area background
        /// </summary>
        private void DrawTabAreaBackground(Graphics g)
        {
            if (g == null || _tabArea.IsEmpty) return;
            
            // Skip if transparent
            if (IsTransparentBackground) return;
            
            // Use a slightly different shade for the tab strip area
            Color tabStripColor = _tabBackColor;
            
            using (var brush = new SolidBrush(tabStripColor))
            {
                g.FillRectangle(brush, _tabArea);
            }
        }

        /// <summary>
        /// Draws tabs directly in OnPaint with proper styling
        /// </summary>
        private void DrawTabsDirectlyInOnPaint(Graphics g)
        {
            if (g == null || _tabs == null || _tabs.Count == 0 || _tabArea.IsEmpty) return;
            
            // Ensure paint helper exists and is configured
            EnsurePaintHelper();
            
            // Draw tab strip background first
            DrawTabAreaBackground(g);
            
            // Draw each visible tab
            foreach (var tab in _tabs.Where(t => t.IsVisible && !t.Bounds.IsEmpty))
            {
                DrawTab(g, tab);
            }
            
            // Draw scroll buttons if needed
            if (_needsScrolling)
            {
                DrawModernButton(g, _scrollLeftButton, 
                    _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom ? ArrowDirection.Left : ArrowDirection.Up, 
                    ControlStyle);
                DrawModernButton(g, _scrollRightButton, 
                    _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom ? ArrowDirection.Right : ArrowDirection.Down, 
                    ControlStyle);
                DrawModernButton(g, _newTabButton, null, ControlStyle, isPlusButton: true);
            }
            
            // Draw separator line between tabs and content
            DrawTabContentSeparator(g);
        }
        
        /// <summary>
        /// Ensures the paint helper is initialized and configured
        /// </summary>
        private void EnsurePaintHelper()
        {
            if (_paintHelper == null)
            {
                _paintHelper = new TabPaintHelper(_currentTheme, ControlStyle, IsTransparentBackground);
            }
            
            // Always update to current settings
            _paintHelper.ControlStyle = ControlStyle;
            _paintHelper.IsTransparent = IsTransparentBackground;
            _paintHelper.TabStyle = TabStyle;
            _paintHelper.Theme = _currentTheme;
        }
      
        /// <summary>
        /// DrawContent is called by BaseControl.OnPaint - this is where we draw our tabs
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            // Skip during batch operations
            if (_batchMode) return;
            
            // Set up quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Draw content area background (if not transparent)
            DrawContentAreaBackground(g);
         
            // Draw tabs if in Tabbed mode
            if (_displayMode == ContainerDisplayMode.Tabbed && !_tabArea.IsEmpty && _tabs != null && _tabs.Count > 0)
            {
                var visibleTabs = _tabs.Where(t => t.IsVisible && !t.Bounds.IsEmpty).ToList();
                if (visibleTabs.Count > 0)
                {
                    DrawTabsDirectlyInOnPaint(g);
                }
            }
            
            // Handle tab transition animation
            HandleTabTransition(g);
        }

        private void DrawTabTransition(Graphics g)
        {
            if (_previousTab?.Addin == null || _activeTab?.Addin == null) return;

            var previousControl = _previousTab.Addin as Control;
            var activeControl = _activeTab.Addin as Control;

            if (previousControl == null || activeControl == null) return;

            // Calculate opacity values for cross-fade
            // transitionProgress: 0 = showing old, 1 = showing new
            float transitionProgress = _animationHelper.TransitionProgress;
            float oldOpacity = 1.0f - transitionProgress;
            float newOpacity = transitionProgress;

            // Draw previous control fading out (controls are hidden during transition, so we draw them)
            if (oldOpacity > 0.01f)
            {
                // Temporarily make control visible for DrawToBitmap, then hide again
                bool wasVisible = previousControl.Visible;
                previousControl.Visible = true;
                DrawControlWithOpacity(g, previousControl, _contentArea, oldOpacity);
                previousControl.Visible = wasVisible;
            }

            // Draw active control fading in
            if (newOpacity > 0.01f)
            {
                // Temporarily make control visible for DrawToBitmap, then hide again
                bool wasVisible = activeControl.Visible;
                activeControl.Visible = true;
                DrawControlWithOpacity(g, activeControl, _contentArea, newOpacity);
                activeControl.Visible = wasVisible;
            }
        }

        private void DrawControlWithOpacity(Graphics g, Control control, Rectangle bounds, float opacity)
        {
            if (control == null || !control.Visible || opacity <= 0) return;

            try
            {
                // Create a bitmap to capture the control's appearance
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var bitmapGraphics = Graphics.FromImage(bitmap))
                    {
                        bitmapGraphics.SmoothingMode = g.SmoothingMode;
                        bitmapGraphics.InterpolationMode = g.InterpolationMode;
                        bitmapGraphics.PixelOffsetMode = g.PixelOffsetMode;
                        bitmapGraphics.TextRenderingHint = g.TextRenderingHint;
                        
                        // Capture control rendering using DrawToBitmap
                        try
                        {
                            control.DrawToBitmap(bitmap, new Rectangle(0, 0, bounds.Width, bounds.Height));
                        }
                        catch
                        {
                            // If DrawToBitmap fails, fall back to simple fill
                            bitmapGraphics.Clear(control.BackColor);
                        }
                    }

                    // Apply opacity and draw
                    using (var imageAttributes = new System.Drawing.Imaging.ImageAttributes())
                    {
                        float[][] colorMatrixElements = {
                            new float[] {1, 0, 0, 0, 0},
                            new float[] {0, 1, 0, 0, 0},
                            new float[] {0, 0, 1, 0, 0},
                            new float[] {0, 0, 0, opacity, 0},
                            new float[] {0, 0, 0, 0, 1}
                        };

                        var colorMatrix = new System.Drawing.Imaging.ColorMatrix(colorMatrixElements);
                        imageAttributes.SetColorMatrix(colorMatrix, 
                            System.Drawing.Imaging.ColorMatrixFlag.Default, 
                            System.Drawing.Imaging.ColorAdjustType.Bitmap);

                        g.DrawImage(bitmap, bounds, 0, 0, bounds.Width, bounds.Height, 
                            GraphicsUnit.Pixel, imageAttributes);
                    }
                }
            }
            catch
            {
                // If rendering fails, just ensure the control is visible at the right opacity
                // Fall back to simple approach
            }
        }
        
        private void DrawTabContentSeparator(Graphics g)
        {
            if (_tabArea.IsEmpty || _contentArea.IsEmpty) return;
            
            // Use theme border color
            Color separatorColor = _borderColor;
            
            using (var pen = new Pen(separatorColor, 1f))
            {
                switch (_tabPosition)
                {
                    case TabPosition.Top:
                        g.DrawLine(pen, _tabArea.Left, _tabArea.Bottom - 1, _tabArea.Right, _tabArea.Bottom - 1);
                        break;
                    case TabPosition.Bottom:
                        g.DrawLine(pen, _tabArea.Left, _tabArea.Top, _tabArea.Right, _tabArea.Top);
                        break;
                    case TabPosition.Left:
                        g.DrawLine(pen, _tabArea.Right - 1, _tabArea.Top, _tabArea.Right - 1, _tabArea.Bottom);
                        break;
                    case TabPosition.Right:
                        g.DrawLine(pen, _tabArea.Left, _tabArea.Top, _tabArea.Left, _tabArea.Bottom);
                        break;
                }
            }
        }

        private void DrawTab(Graphics g, AddinTab tab)
        {
            if (tab == null || tab.Bounds.IsEmpty || string.IsNullOrEmpty(tab.Title)) return;
            
            // Ensure paint helper is ready
            EnsurePaintHelper();
            
            var isActive = tab == _activeTab;
            var isHovered = tab == _hoveredTab;
            
            _paintHelper.DrawProfessionalTab(g, tab.Bounds, tab.Title, BeepThemesManager.ToFont(_currentTheme.TabFont),
                isActive, isHovered, _showCloseButtons, 
                tab.IsCloseHovered, tab.AnimationProgress);
        }

        private void DrawScrollButtons(Graphics g)
        {
            if (!_needsScrolling) return;

            // Get control style for modern button rendering
            var controlStyle = ControlStyle;
            
            // Draw scroll buttons with modern styling
            DrawModernButton(g, _scrollLeftButton, _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom ? ArrowDirection.Left : ArrowDirection.Up, controlStyle);
            DrawModernButton(g, _scrollRightButton, _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom ? ArrowDirection.Right : ArrowDirection.Down, controlStyle);
            DrawModernButton(g, _newTabButton, null, controlStyle, isPlusButton: true);
        }
        
        private void DrawModernButton(Graphics g, Rectangle bounds, ArrowDirection? arrowDirection, BeepControlStyle style, bool isPlusButton = false)
        {
            if(bounds.Width <= 0 || bounds.Height <= 0) return;
            
            // Create rounded path for button
            var buttonPath = BeepStyling.CreateControlStylePath(bounds, style);
            if (buttonPath == null) return;
            
            try
            {
                // Draw button background with subtle gradient
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    bounds,
                    ControlPaint.Light(_tabBackColor, 0.1f),
                    ControlPaint.Dark(_tabBackColor, 0.05f),
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, buttonPath);
                }
                
                // Draw button border
                using (var pen = new Pen(_borderColor, 1f))
                {
                    g.DrawPath(pen, buttonPath);
                }
                
                // Draw icon (arrow or plus)
                var iconColor = _tabForeColor;
                using (var pen = new Pen(iconColor, 2f))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    
                    if (isPlusButton)
                    {
                        // Draw plus sign
                        var center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
                        var size = Math.Min(bounds.Width, bounds.Height) / 4;
                        g.DrawLine(pen, center.X - size, center.Y, center.X + size, center.Y);
                        g.DrawLine(pen, center.X, center.Y - size, center.X, center.Y + size);
                    }
                    else if (arrowDirection.HasValue)
                    {
                        // Draw arrow
                        DrawArrow(g, bounds, arrowDirection.Value);
                    }
                }
            }
            finally
            {
                buttonPath.Dispose();
            }
        }

        private void DrawArrow(Graphics g, Rectangle bounds, ArrowDirection direction)
        {
            var center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            var size = Math.Min(bounds.Width, bounds.Height) / 3;
            var points = new PointF[3];

            switch (direction)
            {
                case ArrowDirection.Left:
                    points[0] = new PointF(center.X - size / 2, center.Y);
                    points[1] = new PointF(center.X + size / 2, center.Y - size);
                    points[2] = new PointF(center.X + size / 2, center.Y + size);
                    break;
                case ArrowDirection.Right:
                    points[0] = new PointF(center.X + size / 2, center.Y);
                    points[1] = new PointF(center.X - size / 2, center.Y - size);
                    points[2] = new PointF(center.X - size / 2, center.Y + size);
                    break;
                case ArrowDirection.Up:
                    points[0] = new PointF(center.X, center.Y - size / 2);
                    points[1] = new PointF(center.X - size, center.Y + size / 2);
                    points[2] = new PointF(center.X + size, center.Y + size / 2);
                    break;
                case ArrowDirection.Down:
                    points[0] = new PointF(center.X, center.Y + size / 2);
                    points[1] = new PointF(center.X - size, center.Y - size / 2);
                    points[2] = new PointF(center.X + size, center.Y - size / 2);
                    break;
            }

            using (var brush = new SolidBrush(_tabForeColor))
            {
                g.FillPolygon(brush, points);
            }
        }

        #endregion
    }
}

