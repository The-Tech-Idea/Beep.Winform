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
            for (int i = 0; i < _itemStates.Count; i++)
            {
                var state = _itemStates[i];
                if (_overflowStartIndex >= 0 && i >= _overflowStartIndex)
                {
                    break;
                }

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
                if (_keyboardNavigationEnabled && state.IsFocused && Focused)
                {
                    PaintFocusIndicator(g, state.Bounds);
                }

                // Paint separators between visible items where enabled.
                if (_config.SeparatorStyle != Docks.DockSeparatorStyle.None)
                {
                    var isLastVisible = i == _itemStates.Count - 1 ||
                        (_overflowStartIndex > 0 && i == _overflowStartIndex - 1);
                    if (!isLastVisible)
                    {
                        var separatorPoint = _config.Orientation == Docks.DockOrientation.Horizontal
                            ? new Point(state.Bounds.Right + (_config.Spacing / 2), state.Bounds.Top)
                            : new Point(state.Bounds.Left, state.Bounds.Bottom + (_config.Spacing / 2));
                        _dockPainter.PaintSeparator(g, separatorPoint, _config, _currentTheme);
                    }
                }
            }

            if (_overflowStartIndex >= 0 && !_overflowBounds.IsEmpty)
            {
                PaintOverflowAffordance(g);
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
            var focusPad = Helpers.DpiScalingHelper.ScaleValue(4, this);
            focusRect.Inflate(focusPad, focusPad);

            var focusColor = _currentTheme?.AccentColor ?? Color.FromArgb(0, 122, 255);

            var focusThickness = Helpers.DpiScalingHelper.ScaleValue(2, this);
            using (var pen = new Pen(focusColor, focusThickness) { DashStyle = DashStyle.Dot })
            {
                g.DrawRectangle(pen, focusRect);
            }
        }

        private void PaintOverflowAffordance(Graphics g)
        {
            if (_overflowBounds.IsEmpty)
            {
                return;
            }

            using (var bg = new SolidBrush(Color.FromArgb(50, _currentTheme?.AccentColor ?? Color.DodgerBlue)))
            {
                g.FillEllipse(bg, _overflowBounds);
            }

            using (var pen = new Pen(_currentTheme?.AccentColor ?? Color.DodgerBlue, Helpers.DpiScalingHelper.ScaleValue(2, this)))
            {
                var cy = _overflowBounds.Top + (_overflowBounds.Height / 2);
                var startX = _overflowBounds.Left + (_overflowBounds.Width / 4);
                var endX = _overflowBounds.Right - (_overflowBounds.Width / 4);
                g.DrawLine(pen, startX, cy, endX, cy);
            }
        }
        #endregion
    }
}
