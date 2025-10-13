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
            
            // Get metrics from painter
            var metrics = _contextMenuPainter.GetMetrics(theme, false);
            
            // Let painter draw everything
            _contextMenuPainter.DrawBackground(e.Graphics, this, _contentAreaRect, metrics, theme);
            _contextMenuPainter.DrawItems(e.Graphics, this, _menuItems, _selectedItem, _hoveredItem, metrics, theme);
            _contextMenuPainter.DrawBorder(e.Graphics, this, ClientRectangle, metrics, theme);
        }
        
        #endregion
    }
}
