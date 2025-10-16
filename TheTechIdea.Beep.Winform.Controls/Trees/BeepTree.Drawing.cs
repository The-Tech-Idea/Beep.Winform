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
          //  base.ApplyTheme();

            if (_currentTheme != null)
            {
                // Validate if Colors are set, else fallback to defaults
                if (_currentTheme.TreeBackColor == Color.Empty)
                    _currentTheme.TreeBackColor = Color.White;
                if (_currentTheme.TreeForeColor == Color.Empty)
                    _currentTheme.TreeForeColor = Color.Black;
                if (_currentTheme.TreeNodeSelectedFont == null)
                    _currentTheme.TreeNodeSelectedFont = new TypographyStyle
                    {
                        FontFamily = "Arial",
                        FontSize = 10,
                        FontWeight = FontWeight.Bold,
                        TextColor = Color.White
                    };
                if (_currentTheme.TreeNodeUnSelectedFont == null)
                    _currentTheme.TreeNodeUnSelectedFont = new TypographyStyle
                    {
                        FontFamily = "Arial",
                        FontSize = 10,
                        FontWeight = FontWeight.Regular,
                        TextColor = Color.Black
                    };
                if (_currentTheme.LabelFont == null)
                    _currentTheme.LabelFont = new TypographyStyle
                    {
                        FontFamily = "Arial",
                        FontSize = 10,
                        FontWeight = FontWeight.Regular,
                        TextColor = Color.Black
                    };
                if (_currentTheme.TreeNodeSelectedBackColor == Color.Empty)
                    _currentTheme.TreeNodeSelectedBackColor = Color.Blue;
                if (_currentTheme.TreeNodeSelectedForeColor == Color.Empty)
                    _currentTheme.TreeNodeSelectedForeColor = Color.White;
                if (_currentTheme.TreeNodeHoverBackColor == Color.Empty)
                    _currentTheme.TreeNodeHoverBackColor = Color.LightGray;
                if (_currentTheme.TreeNodeHoverForeColor == Color.Empty)
                    _currentTheme.TreeNodeHoverForeColor = Color.Black;
                if (_currentTheme.TreeNodeCheckedBoxBackColor == Color.Empty)
                    _currentTheme.TreeNodeCheckedBoxBackColor = Color.Blue;
                if (_currentTheme.TreeNodeCheckedBoxForeColor == Color.Empty)
                    _currentTheme.TreeNodeCheckedBoxForeColor = Color.White;
                if (_currentTheme.TreeBorderColor == Color.Empty)
                    _currentTheme.TreeBorderColor = Color.Gray;
                if (_currentTheme.TreeNodeForeColor == Color.Empty)
                    _currentTheme.TreeNodeForeColor = Color.Black;
                if (_currentTheme.AccentColor == Color.Empty)
                    _currentTheme.AccentColor = Color.DodgerBlue;
               

                    // Apply theme colors
                    BackColor = TreeBackColor;
                ForeColor = TreeForeColor;

                // Update font if using theme font
                if (UseThemeFont)
                {
                    try
                    {
                        var themeFont = ThemeManagement.BeepThemesManager.ToFont(_currentTheme.LabelFont);
                        // Framework handles DPI scaling automatically
                        TextFont = themeFont;
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
