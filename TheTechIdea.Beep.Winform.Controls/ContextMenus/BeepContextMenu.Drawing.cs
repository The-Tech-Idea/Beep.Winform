using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    public partial class BeepContextMenu
    {
        #region Paint Override
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (_contextMenuPainter == null || _menuItems == null)
            {
                return;
            }
            
            // Enable anti-aliasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Use current theme (kept in sync with BeepThemesManager)
            var theme = _currentTheme ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();
            var rect = new Rectangle(1 ,1, _contentAreaRect.Width - 5, Height - 5);
            // Get metrics from painter
            var metrics = _contextMenuPainter.GetMetrics(theme, false);
            
            // Apply clipping if scrolling is enabled
            if (_needsScrolling)
            {
                var clipRect = new Rectangle(1, 1, rect.Width-5, rect.Height - 5);
                e.Graphics.SetClip(clipRect);
                
                // Translate graphics context by scroll offset
                e.Graphics.TranslateTransform(0, -_scrollOffset);
            }
            
            // Let painter draw everything
            _contextMenuPainter.DrawBackground(e.Graphics, this, rect, metrics, theme);
            _contextMenuPainter.DrawItems(e.Graphics, this, _menuItems, _selectedItem, _hoveredItem, metrics, theme);
            
            // Reset transform before drawing border
            if (_needsScrolling)
            {
                e.Graphics.ResetTransform();
                e.Graphics.ResetClip();
            }
            
          //  _contextMenuPainter.DrawBorder(e.Graphics, this, ClientRectangle, metrics, theme);
        }
        
        #endregion
    }
}
