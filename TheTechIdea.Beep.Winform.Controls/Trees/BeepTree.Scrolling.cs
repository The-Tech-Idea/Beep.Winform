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

            // Always base layout on the BaseControl-provided DrawingRect
            // so we match other controls (ComboBox, Chart, etc.).
            var inner = DrawingRect; // content area from BaseControl painter

            // Guard: if DrawingRect is not ready yet, defer until next paint/resize
            if (inner.Width <= 0 || inner.Height <= 0)
            {
                _verticalScrollBar.Visible = false;
                _horizontalScrollBar.Visible = false;
                return;
            }

            int availW = inner.Width;
            int availH = inner.Height;
            int vBarW = _verticalScrollBar.Width;
            int hBarH = _horizontalScrollBar.Height;

            // Determine necessity with interplay: adding one scrollbar may force the other
            bool needsV = ShowVerticalScrollBar && _virtualSize.Height > availH;
            bool needsH = ShowHorizontalScrollBar && _virtualSize.Width > availW;

            // First pass adjustments
            if (needsV)
                availW -= vBarW;
            if (needsH)
                availH -= hBarH;

            // Re-evaluate after adjustments
            if (ShowVerticalScrollBar && !needsV && _virtualSize.Height > availH)
            {
                needsV = true;
                availW -= vBarW;
            }
            if (ShowHorizontalScrollBar && !needsH && _virtualSize.Width > availW)
            {
                needsH = true;
                availH -= hBarH;
            }

            // Apply visibility
            _verticalScrollBar.Visible = needsV;
            _horizontalScrollBar.Visible = needsH;

            // Compute final client area after visibility is known
            Rectangle clientArea = GetClientArea();

            // Configure vertical scrollbar relative to DrawingRect
            if (needsV)
            {
                int vHeight = inner.Height - (needsH ? hBarH : 0);
                _verticalScrollBar.Bounds = new Rectangle(
                    inner.Right - vBarW,
                    inner.Top,
                    vBarW,
                    Math.Max(0, vHeight)
                );
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = Math.Max(0, _virtualSize.Height);
                _verticalScrollBar.LargeChange = Math.Max(1, clientArea.Height);
                _verticalScrollBar.SmallChange = Math.Max(1, GetScaledMinRowHeight());
                int vMaxVal = Math.Max(0, _verticalScrollBar.Maximum - _verticalScrollBar.LargeChange);
                _verticalScrollBar.Value = Math.Min(Math.Max(0, _yOffset), vMaxVal);
            }
            else
            {
                _yOffset = 0;
            }

            // Configure horizontal scrollbar relative to DrawingRect
            if (needsH)
            {
                int hWidth = inner.Width - (needsV ? vBarW : 0);
                _horizontalScrollBar.Bounds = new Rectangle(
                    inner.Left,
                    inner.Bottom - hBarH,
                    Math.Max(0, hWidth),
                    hBarH
                );
                _horizontalScrollBar.Minimum = 0;
                _horizontalScrollBar.Maximum = Math.Max(0, _virtualSize.Width);
                _horizontalScrollBar.LargeChange = Math.Max(1, clientArea.Width);
                _horizontalScrollBar.SmallChange = Math.Max(1, GetScaledIndentWidth());
                int hMaxVal = Math.Max(0, _horizontalScrollBar.Maximum - _horizontalScrollBar.LargeChange);
                _horizontalScrollBar.Value = Math.Min(Math.Max(0, _xOffset), hMaxVal);
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
            // Base the client area on BaseControl.DrawingRect (inner content area),
            // subtracting any visible scrollbars. This matches other controls.
            var inner = DrawingRect;
            if (inner.Width <= 0 || inner.Height <= 0)
                return Rectangle.Empty;

            int vBarW = (_verticalScrollBar?.Visible == true) ? _verticalScrollBar.Width : 0;
            int hBarH = (_horizontalScrollBar?.Visible == true) ? _horizontalScrollBar.Height : 0;

            return new Rectangle(
                inner.Left,
                inner.Top,
                Math.Max(0, inner.Width - vBarW),
                Math.Max(0, inner.Height - hBarH)
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
            try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
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
            try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
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
                    try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
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
