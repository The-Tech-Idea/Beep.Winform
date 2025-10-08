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
            System.Diagnostics.Debug.WriteLine("[BeepTree] DrawContent() start");
            base.DrawContent(g);
            
            // Ensure DrawingRect is up-to-date for viewport transforms
            UpdateDrawingRect();
            
            // Keep scrollbars in sync
            try { UpdateScrollBars(); } catch { }
            
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
            System.Diagnostics.Debug.WriteLine($"BeepTree.DrawContent: Using painter {painter.GetType().Name}");

            // Use DrawingRect from BaseControl (already accounts for borders, padding, etc.)
            Rectangle drawingArea = DrawingRect;
            System.Diagnostics.Debug.WriteLine($"BeepTree.DrawContent: Drawing area = {drawingArea}");
            if (drawingArea.Width <= 0 || drawingArea.Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"BeepTree.DrawContent: Invalid drawing area: {drawingArea}");
                return;
            }

            // Set clip region to drawing area
            try
            {
                g.SetClip(drawingArea);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepTree: SetClip failed: {ex.Message}");
            }

            // Let the painter draw the ENTIRE TREE (all nodes)
            try
            {
                System.Diagnostics.Debug.WriteLine($"[BeepTree] Calling painter.Paint() to draw entire tree");
                painter.Paint(g, this, drawingArea);
                // After painting, refresh hit areas for BaseControl hit-test infra
                try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BeepTree] Painter.Paint failed: {ex.Message}\n{ex.StackTrace}");
            }

          
            System.Diagnostics.Debug.WriteLine("[BeepTree] DrawContent() end");
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
                // Theme/font can change metrics - rebuild hit areas
                try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
            }

            Invalidate();
        }

        #endregion
    }
}
