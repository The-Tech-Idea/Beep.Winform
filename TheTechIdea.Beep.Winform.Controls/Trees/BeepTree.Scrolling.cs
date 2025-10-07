using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepTree - Scrolling partial class.
    /// Handles scrollbar initialization, updates, and scroll events.
    /// </summary>
    public partial class BeepTree
    {
        #region Scrollbar Initialization

        /// <summary>
        /// Initializes the vertical and horizontal scrollbars.
        /// </summary>
        private void InitializeScrollbars()
        {
            // Vertical scrollbar
            _verticalScrollBar = new BeepScrollBar
            {
                IsChild = true,
                ScrollOrientation = Orientation.Vertical,
                Dock = DockStyle.None,
                Width = 16
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            Controls.Add(_verticalScrollBar);

            // Horizontal scrollbar
            _horizontalScrollBar = new BeepScrollBar
            {
                IsChild = true,
                ScrollOrientation =  Orientation.Horizontal,
                Dock = DockStyle.None,
                Height = 16
            };
            _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            Controls.Add(_horizontalScrollBar);

            UpdateScrollBars();
        }

        #endregion

        #region Scrollbar Update

        /// <summary>
        /// Updates the scrollbar positions, visibility, and ranges based on content size.
        /// </summary>
        internal void UpdateScrollBars()
        {
            if (_verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            // Get client area (area available for content)
            Rectangle clientArea = GetClientArea();

            // Vertical scrollbar
            bool needsVerticalScroll = _virtualSize.Height > clientArea.Height && ShowVerticalScrollBar;
            _verticalScrollBar.Visible = needsVerticalScroll;

            if (needsVerticalScroll)
            {
                _verticalScrollBar.Bounds = new Rectangle(
                    Width - _verticalScrollBar.Width,
                    0,
                    _verticalScrollBar.Width,
                    Height - (needsVerticalScroll && _horizontalScrollBar.Visible ? _horizontalScrollBar.Height : 0)
                );

                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = _virtualSize.Height;
                _verticalScrollBar.LargeChange = clientArea.Height;
                _verticalScrollBar.SmallChange = GetScaledMinRowHeight();
                _verticalScrollBar.Value = Math.Min(_yOffset, Math.Max(0, _virtualSize.Height - clientArea.Height));
            }
            else
            {
                _yOffset = 0;
            }

            // Horizontal scrollbar
            bool needsHorizontalScroll = _virtualSize.Width > clientArea.Width && ShowHorizontalScrollBar;
            _horizontalScrollBar.Visible = needsHorizontalScroll;

            if (needsHorizontalScroll)
            {
                _horizontalScrollBar.Bounds = new Rectangle(
                    0,
                    Height - _horizontalScrollBar.Height,
                    Width - (needsHorizontalScroll && _verticalScrollBar.Visible ? _verticalScrollBar.Width : 0),
                    _horizontalScrollBar.Height
                );

                _horizontalScrollBar.Minimum = 0;
                _horizontalScrollBar.Maximum = _virtualSize.Width;
                _horizontalScrollBar.LargeChange = clientArea.Width;
                _horizontalScrollBar.SmallChange = GetScaledIndentWidth();
                _horizontalScrollBar.Value = Math.Min(_xOffset, Math.Max(0, _virtualSize.Width - clientArea.Width));
            }
            else
            {
                _xOffset = 0;
            }
        }

        #endregion

        #region Client Area Calculation

        /// <summary>
        /// Gets the client area available for drawing content (excluding scrollbars).
        /// </summary>
        private Rectangle GetClientArea()
        {
            int scrollBarWidth = (_verticalScrollBar?.Visible == true) ? _verticalScrollBar.Width : 0;
            int scrollBarHeight = (_horizontalScrollBar?.Visible == true) ? _horizontalScrollBar.Height : 0;

            return new Rectangle(
                0,
                0,
                Width - scrollBarWidth,
                Height - scrollBarHeight
            );
        }

        #endregion

        #region Scroll Event Handlers

        /// <summary>
        /// Handles vertical scrollbar scroll events.
        /// </summary>
        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            if (sender is BeepScrollBar sb)
            {
                _yOffset = sb.Value;
            }
            Invalidate();
        }

        /// <summary>
        /// Handles horizontal scrollbar scroll events.
        /// </summary>
        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            if (sender is BeepScrollBar sb)
            {
                _xOffset = sb.Value;
            }
            Invalidate();
        }

        #endregion

        #region Mouse Wheel Support

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (_verticalScrollBar?.Visible == true)
            {
                int delta = -e.Delta / 120 * _verticalScrollBar.SmallChange;
                int newValue = Math.Max(_verticalScrollBar.Minimum, 
                                       Math.Min(_verticalScrollBar.Maximum - _verticalScrollBar.LargeChange, 
                                               _yOffset + delta));
                
                if (newValue != _yOffset)
                {
                    _yOffset = newValue;
                    _verticalScrollBar.Value = newValue;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Resize Handling

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _resizeTimer?.Stop();
            _resizeTimer?.Start();
        }

        #endregion

        #region Scroll To Node

        /// <summary>
        /// Scrolls the tree to make the specified node visible.
        /// </summary>
        public void ScrollToNode(SimpleItem node)
        {
            if (node == null)
                return;

            // Find the node in visible nodes
            var nodeInfo = _visibleNodes.Find(n => n.Item == node);
            if (nodeInfo.Item == null)
                return;

            Rectangle clientArea = GetClientArea();

            // Check if node is already visible
            int nodeTop = nodeInfo.Y - _yOffset;
            int nodeBottom = nodeTop + nodeInfo.RowHeight;

            if (nodeTop < 0)
            {
                // Node is above viewport - scroll up
                _yOffset = nodeInfo.Y;
            }
            else if (nodeBottom > clientArea.Height)
            {
                // Node is below viewport - scroll down
                _yOffset = nodeInfo.Y - clientArea.Height + nodeInfo.RowHeight;
            }

            // Ensure offset is within bounds
            _yOffset = Math.Max(0, Math.Min(_yOffset, _virtualSize.Height - clientArea.Height));

            if (_verticalScrollBar?.Visible == true)
            {
                _verticalScrollBar.Value = _yOffset;
            }

            Invalidate();
        }

        #endregion
    }
}
