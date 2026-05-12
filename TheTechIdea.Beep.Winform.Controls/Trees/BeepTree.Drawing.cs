using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepTree - Drawing partial class.
    /// Handles all rendering logic using painters for clean separation of concerns.
    /// </summary>
    public partial class BeepTree
    {
        #region Main Drawing Method

        /// <summary>
        /// Main drawing method - coordinates rendering using the active painter.
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
         //   System.Diagnostics.Debug.WriteLine("[BeepTree] DrawContent() start");
            base.DrawContent(g);
            
            // Ensure DrawingRect is up-to-date for viewport transforms
            UpdateDrawingRect();
            
            if (g == null)
            {
                System.Diagnostics.Debug.WriteLine("BeepTree.DrawContent: Graphics is null!");
                return;
            }

            // Get the painter
            var painter = GetCurrentPainter();
            if (painter == null)
            {
                System.Diagnostics.Debug.WriteLine("BeepTree.DrawContent: No painter available!");
                return;
            }
         //   System.Diagnostics.Debug.WriteLine($"BeepTree.DrawContent: Using painter {painter.GetType().Name}");

            // Use client area (excludes scrollbars) for drawing and clipping
            Rectangle drawingArea = GetClientArea();
         //   System.Diagnostics.Debug.WriteLine($"BeepTree.DrawContent: Drawing area = {drawingArea}");
            if (drawingArea.Width <= 0 || drawingArea.Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"BeepTree.DrawContent: Invalid drawing area: {drawingArea}");
                return;
            }

            // Set clip region to client area (excludes scrollbars)
            try
            {
                g.SetClip(drawingArea);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepTree: SetClip failed: {ex.Message}");
            }

            // Draw breadcrumb navigation if enabled
            if (ShowBreadcrumb && SelectedNode != null)
            {
                DrawBreadcrumb(g, drawingArea);
                drawingArea.Y += _breadcrumbHeight;
                drawingArea.Height -= _breadcrumbHeight;
            }

            // FIG-03: Empty-state rendering — show a centred message when there are no nodes
            if (_nodes.Count == 0 || IsLoading)
            {
                Font emptyFont = _textFont;
                bool disposeEmptyFont = false;
                if (_useThemeFont && _currentTheme?.LabelFont != null)
                {
                    var themeFont = ThemeManagement.BeepThemesManager.ToFontForControl(_currentTheme.LabelFont, this);
                    if (themeFont != null && themeFont != _textFont)
                    {
                        emptyFont = themeFont;
                        disposeEmptyFont = true;
                    }
                }
                var emptyColor = _currentTheme?.TreeForeColor ?? ForeColor;

                string displayText = IsLoading ? LoadingText : _emptyStateText;
                TextRenderer.DrawText(g, displayText, emptyFont, drawingArea, emptyColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

                if (disposeEmptyFont)
                {
                    emptyFont?.Dispose();
                }

                if (IsLoading)
                {
                    // Draw a simple loading indicator (animated dots could be added later)
                    int dotSize = 8;
                    int dotSpacing = 12;
                    int totalWidth = dotSize * 3 + dotSpacing * 2;
                    int startX = drawingArea.Left + (drawingArea.Width - totalWidth) / 2;
                    int dotY = drawingArea.Top + drawingArea.Height / 2 + 20;

                    for (int i = 0; i < 3; i++)
                    {
                        var dotRect = new Rectangle(startX + i * (dotSize + dotSpacing), dotY, dotSize, dotSize);
                        using (var brush = new System.Drawing.SolidBrush(emptyColor))
                        {
                            g.FillEllipse(brush, dotRect);
                        }
                    }
                }

                return;
            }

            // Let the painter draw the ENTIRE TREE (all nodes)
            try
            {
           //     System.Diagnostics.Debug.WriteLine($"[BeepTree] Calling painter.Paint() to draw entire tree");
                painter.Paint(g, this, drawingArea);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BeepTree] Painter.Paint failed: {ex.Message}\n{ex.StackTrace}");
            }

            // FIG-04: Focus ring — draw a dotted accent-colour rect around the selected node row
            if (Focused && SelectedNode != null && _visibleNodes.Count > 0)
            {
                NodeInfo ni = default;
                for (int i = 0; i < _visibleNodes.Count; i++)
                {
                    if (_visibleNodes[i].Item == SelectedNode) { ni = _visibleNodes[i]; break; }
                }
                if (ni.Item != null && ni.RowRectContent.Width > 0)
                {
                    var vp = _layoutHelper.TransformToViewport(ni.RowRectContent);
                    vp.Intersect(drawingArea);
                    if (vp.Width > 4 && vp.Height > 4)
                    {
                        var focusColor = FocusIndicatorColor != Color.Empty ? FocusIndicatorColor : (_currentTheme?.AccentColor ?? System.Drawing.Color.DodgerBlue);
                        using var focusPen = new System.Drawing.Pen(focusColor, 1.5f)
                        {
                            DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
                        };
                        g.DrawRectangle(focusPen, System.Drawing.Rectangle.Inflate(vp, -1, -1));
                    }
                }
            }

          
         //   System.Diagnostics.Debug.WriteLine("[BeepTree] DrawContent() end");
        }

        /// <summary>
        /// Draws the breadcrumb navigation bar at the top of the tree.
        /// </summary>
        private void DrawBreadcrumb(Graphics g, Rectangle bounds)
        {
            if (SelectedNode == null)
                return;

            var path = BreadcrumbPath;
            if (path.Count == 0)
                return;

            // Clear previous hit-test data
            _breadcrumbItemRects.Clear();
            _breadcrumbItems.Clear();

            var bgBrush = PaintersFactory.GetSolidBrush(_currentTheme?.TreeBackColor ?? SystemColors.Control);
            var breadcrumbBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, _breadcrumbHeight);
            g.FillRectangle(bgBrush, breadcrumbBounds);

            // Draw separator line
            var separatorPen = PaintersFactory.GetPen(_currentTheme?.BorderColor ?? Color.Gray, 1f);
            g.DrawLine(separatorPen, breadcrumbBounds.Left, breadcrumbBounds.Bottom - 1, breadcrumbBounds.Right, breadcrumbBounds.Bottom - 1);

            // Draw breadcrumb items
            var font = _textFont ?? SystemFonts.DefaultFont;
            var textColor = _currentTheme?.TreeForeColor ?? Color.Black;
            var accentColor = _currentTheme?.AccentColor ?? Color.Blue;

            int x = breadcrumbBounds.X + 8;
            int y = breadcrumbBounds.Y + (breadcrumbBounds.Height - font.Height) / 2;

            for (int i = 0; i < path.Count; i++)
            {
                var node = path[i];
                string text = node.Text ?? "Root";

                // Measure text
                var textSize = TextRenderer.MeasureText(g, text, font);
                var textRect = new Rectangle(x, y, textSize.Width, textSize.Height);

                // Store for hit testing
                _breadcrumbItemRects.Add(textRect);
                _breadcrumbItems.Add(node);

                // Draw text
                bool isLast = i == path.Count - 1;
                var color = isLast ? accentColor : textColor;
                TextRenderer.DrawText(g, text, font, textRect, color, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                x += textSize.Width;

                // Draw separator
                if (i < path.Count - 1)
                {
                    var sepText = " > ";
                    var sepSize = TextRenderer.MeasureText(g, sepText, font);
                    var sepRect = new Rectangle(x, y, sepSize.Width, sepSize.Height);
                    TextRenderer.DrawText(g, sepText, font, sepRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    x += sepSize.Width;
                }
            }
        }

        #endregion

        #region Theme Application

        /// <summary>
        /// Applies the current theme to the tree and its painters.
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme != null)
            {
                // Use local fallback values instead of mutating the shared theme
                // This prevents corruption of theme colors across multiple controls
                var treeBackColor = _currentTheme.TreeBackColor == Color.Empty ? Color.White : _currentTheme.TreeBackColor;
                var treeForeColor = _currentTheme.TreeForeColor == Color.Empty ? Color.Black : _currentTheme.TreeForeColor;
                var selectedBackColor = _currentTheme.TreeNodeSelectedBackColor == Color.Empty ? Color.Blue : _currentTheme.TreeNodeSelectedBackColor;
                var selectedForeColor = _currentTheme.TreeNodeSelectedForeColor == Color.Empty ? Color.White : _currentTheme.TreeNodeSelectedForeColor;
                var hoverBackColor = _currentTheme.TreeNodeHoverBackColor == Color.Empty ? Color.LightGray : _currentTheme.TreeNodeHoverBackColor;
                var hoverForeColor = _currentTheme.TreeNodeHoverForeColor == Color.Empty ? Color.Black : _currentTheme.TreeNodeHoverForeColor;
                var accentColor = _currentTheme.AccentColor == Color.Empty ? Color.DodgerBlue : _currentTheme.AccentColor;

                // Apply theme colors
                BackColor = treeBackColor;
                ForeColor = treeForeColor;

                // Update font if using theme font
                if (UseThemeFont)
                {
                    try
                    {
                        var themeFont = ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont);
                        // Set backing field directly to avoid triggering UseThemeFont = false
                        _textFont = themeFont;
                    }
                    catch { /* keep existing TextFont on failure */ }
                }

                //// Apply theme to renderers
                //if (_toggleRenderer != null)
                //{
                //    _toggleRenderer.MenuStyle = MenuStyle;
                //    _toggleRenderer.ApplyTheme();
                //}

                //if (_checkRenderer != null)
                //{
                //    _checkRenderer.MenuStyle = MenuStyle;
                //    _checkRenderer.ApplyTheme();
                //}

                //if (_iconRenderer != null)
                //{
                //    _iconRenderer.MenuStyle = MenuStyle;
                //    _iconRenderer.ApplyTheme();
                //}

                //if (_buttonRenderer != null)
                //{
                //    _buttonRenderer.MenuStyle = MenuStyle;
                //    _buttonRenderer.ApplyTheme();
                //}

                //if (_verticalScrollBar != null)
                //{
                //    _verticalScrollBar.MenuStyle = MenuStyle;
                //    _verticalScrollBar.ApplyTheme();
                //}

                //if (_horizontalScrollBar != null)
                //{
                //    _horizontalScrollBar.MenuStyle = MenuStyle;
                //    _horizontalScrollBar.ApplyTheme();
                //}

                // Reinitialize painter with new theme
                InitializePainter();

                // Recalculate layout (fonts may have changed)
                RecalculateLayoutCache();
                // MenuStyle/font can change metrics - rebuild hit areas
                try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
            }

            Invalidate();
        }

        #endregion
    }
}
