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
                // Skip dragged item (it will be painted at cursor position)
                if (state.IsDragging && _isDragging)
                    continue;

                // Paint item
                _dockPainter.PaintDockItem(g, state, _config, _currentTheme);
                
                // Paint selection/running indicator
                _dockPainter.PaintIndicator(g, state, _config, _currentTheme);

                // Paint badges and notifications
                PaintBadge(g, state, state.Bounds);

                // Paint progress indicators
                PaintProgressIndicator(g, state, state.Bounds);

                // Paint keyboard focus indicator
                if (_keyboardNavigationEnabled && _focusedIndex >= 0 && 
                    _itemStates.IndexOf(state) == _focusedIndex && Focused)
                {
                    PaintFocusIndicator(g, state.Bounds);
                }
            }

            // Paint drag feedback on top
            if (_isDragging)
            {
                PaintDragFeedback(g);
            }
        }

        /// <summary>
        /// Paints keyboard focus indicator
        /// </summary>
        private void PaintFocusIndicator(Graphics g, Rectangle bounds)
        {
            var focusRect = bounds;
            focusRect.Inflate(4, 4);

            var focusColor = _currentTheme?.AccentColor ?? Color.FromArgb(0, 122, 255);
            
            using (var pen = new Pen(focusColor, 2f) { DashStyle = DashStyle.Dot })
            {
                g.DrawRectangle(pen, focusRect);
            }
        }
        #endregion
    }
}
