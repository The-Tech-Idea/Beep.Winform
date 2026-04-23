using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Tracks keyboard focus state for <see cref="BeepGridPro"/> and provides
    /// visual focus indication.
    /// </summary>
    public sealed class GridFocusManager
    {
        private readonly BeepGridPro _grid;
        private bool _hasFocus;

        public GridFocusManager(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _grid.GotFocus += OnGotFocus;
            _grid.LostFocus += OnLostFocus;
        }

        /// <summary>
        /// Whether the grid currently has keyboard focus.
        /// </summary>
        public bool HasFocus => _hasFocus;

        /// <summary>
        /// Color used for the focus border when the grid is focused.
        /// </summary>
        public Color FocusBorderColor { get; set; } = Color.DodgerBlue;

        /// <summary>
        /// Thickness of the focus border in pixels.
        /// </summary>
        public int FocusBorderThickness { get; set; } = 2;

        private void OnGotFocus(object? sender, EventArgs e)
        {
            _hasFocus = true;
            _grid.SafeInvalidate();
        }

        private void OnLostFocus(object? sender, EventArgs e)
        {
            _hasFocus = false;
            _grid.SafeInvalidate();
        }

        /// <summary>
        /// Draws a focus rectangle around the grid content area when focused.
        /// Call from the grid's paint handler.
        /// </summary>
        public void DrawFocusIndicator(Graphics g, Rectangle bounds)
        {
            if (!_hasFocus || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            using var pen = new Pen(FocusBorderColor, FocusBorderThickness);
            var rect = Rectangle.Inflate(bounds, -FocusBorderThickness / 2, -FocusBorderThickness / 2);
            g.DrawRectangle(pen, rect);
        }

        /// <summary>
        /// Detaches event handlers. Call during grid disposal.
        /// </summary>
        public void Dispose()
        {
            _grid.GotFocus -= OnGotFocus;
            _grid.LostFocus -= OnLostFocus;
        }
    }
}
