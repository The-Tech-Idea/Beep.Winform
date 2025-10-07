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
            if (g == null) return;

            // Get the painter
            var painter = GetCurrentPainter();
            if (painter == null) return;

            // Get client area (excluding scrollbars)
            Rectangle clientArea = GetClientArea();

            // Set clip region to client area
            g.SetClip(clientArea);

            // Let the painter draw the tree background
            painter.Paint(g, this, clientArea);

            // Draw visible nodes
            DrawVisibleNodes(g, clientArea, painter);

            // Reset clip
            g.ResetClip();
        }

        #endregion

        #region Node Drawing

        /// <summary>
        /// Draws all visible nodes within the viewport using the painter.
        /// Uses layout helper for viewport queries and coordinate transformation.
        /// Delegates all actual rendering to the painter.
        /// </summary>
        private void DrawVisibleNodes(Graphics g, Rectangle clientArea, ITreePainter painter)
        {
            if (_layoutHelper == null) return;

            // Get cached layout from helper
            var layout = _layoutHelper.GetCachedLayout();
            if (layout == null || layout.Count == 0) return;

            // Get font to use (convert theme TypographyStyle to Font)
            Font font = UseThemeFont && _currentTheme != null
                ? ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont)
                : TextFont;
            if (font == null) font = TextFont;

            foreach (var nodeInfo in layout)
            {
                // Use layout helper to check if node is in viewport (with virtualization support)
                if (VirtualizeLayout && !_layoutHelper.IsNodeInViewport(nodeInfo))
                    continue;

                // Transform all rectangles to viewport space using layout helper
                Rectangle rowRect = _layoutHelper.TransformToViewport(nodeInfo.RowRectContent);
                Rectangle toggleRect = _layoutHelper.TransformToViewport(nodeInfo.ToggleRectContent);
                Rectangle checkRect = _layoutHelper.TransformToViewport(nodeInfo.CheckRectContent);
                Rectangle iconRect = _layoutHelper.TransformToViewport(nodeInfo.IconRectContent);
                Rectangle textRect = _layoutHelper.TransformToViewport(nodeInfo.TextRectContent);

                // Determine node state
                bool isSelected = nodeInfo.Item.IsSelected;
                bool isHovered = (_lastHoveredItem == nodeInfo.Item);
                bool hasChildren = nodeInfo.Item.Children?.Count > 0;

                // Paint node background (selection/hover effects) - Painter decides how
                painter.PaintNodeBackground(g, rowRect, isHovered, isSelected);

                // Paint toggle button if has children - Painter decides appearance
                if (hasChildren && !toggleRect.IsEmpty)
                {
                    painter.PaintToggle(g, toggleRect, nodeInfo.Item.IsExpanded, hasChildren, isHovered);
                }

                // Paint checkbox if enabled - Painter decides style
                if (ShowCheckBox && !checkRect.IsEmpty)
                {
                    painter.PaintCheckbox(g, checkRect, nodeInfo.Item.IsChecked, isHovered);
                }

                // Paint icon if available - Painter decides rendering
                if (!string.IsNullOrEmpty(nodeInfo.Item.ImagePath) && !iconRect.IsEmpty)
                {
                    painter.PaintIcon(g, iconRect, nodeInfo.Item.ImagePath);
                }

                // Paint text - Painter decides font color, effects, etc.
                if (!textRect.IsEmpty)
                {
                    painter.PaintText(g, textRect, nodeInfo.Item.Text ?? "", font, isSelected, isHovered);
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
                // Apply theme colors
                BackColor = TreeBackColor;
                ForeColor = TreeForeColor;

                // Update font if using theme font
                if (UseThemeFont)
                {
                    try
                    {
                        var themeFont = ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont);
                        TextFont = UseScaledFont ? ScaleFont(themeFont) : themeFont;
                    }
                    catch { /* keep existing TextFont on failure */ }
                }

                // Apply theme to renderers
                if (_toggleRenderer != null)
                {
                    _toggleRenderer.Theme = Theme;
                    _toggleRenderer.ApplyTheme();
                }

                if (_checkRenderer != null)
                {
                    _checkRenderer.Theme = Theme;
                    _checkRenderer.ApplyTheme();
                }

                if (_iconRenderer != null)
                {
                    _iconRenderer.Theme = Theme;
                    _iconRenderer.ApplyTheme();
                }

                if (_buttonRenderer != null)
                {
                    _buttonRenderer.Theme = Theme;
                    _buttonRenderer.ApplyTheme();
                }

                if (_verticalScrollBar != null)
                {
                    _verticalScrollBar.Theme = Theme;
                    _verticalScrollBar.ApplyTheme();
                }

                if (_horizontalScrollBar != null)
                {
                    _horizontalScrollBar.Theme = Theme;
                    _horizontalScrollBar.ApplyTheme();
                }

                // Reinitialize painter with new theme
                InitializePainter();

                // Recalculate layout (fonts may have changed)
                RecalculateLayoutCache();
            }

            Invalidate();
        }

        #endregion
    }
}
