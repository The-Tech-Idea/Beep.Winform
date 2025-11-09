using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Drawing
    /// </summary>
    public partial class BeepDock
    {
        #region Drawing
        /// <summary>
        /// Draws the dock content
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (_dockPainter == null || _items.Count == 0)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Paint dock background
            _dockPainter.PaintDockBackground(g, ClientRectangle, _config, _currentTheme);

            // Paint each dock item
            foreach (var state in _itemStates)
            {
                _dockPainter.PaintDockItem(g, state, _config, _currentTheme);
                _dockPainter.PaintIndicator(g, state, _config, _currentTheme);
            }
        }
        #endregion
    }
}
