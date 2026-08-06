using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Auto-hide: the dock retracts to a thin reveal strip and comes back when the pointer
    /// approaches the edge it is docked to.
    /// </summary>
    /// <remarks>
    /// <c>AutoHide</c> and <c>AutoHideDelay</c> were published properties with no mechanism at all -
    /// setting them did nothing, in any style. They are implemented rather than deleted because
    /// removing published properties breaks consumers, and auto-hide is a real dock behaviour rather
    /// than a name someone liked.
    ///
    /// The retracted dock keeps a few pixels on screen. A dock that hides completely cannot be
    /// brought back with a mouse, which is a trap rather than a feature.
    /// </remarks>
    public partial class BeepDock
    {
        private const int RevealStripThickness = 4;

        private Timer _autoHideTimer;
        private bool _isRetracted;
        private int _expandedExtent;

        /// <summary>True while the dock is retracted to its reveal strip.</summary>
        [System.ComponentModel.Browsable(false)]
        public bool IsAutoHidden => _isRetracted;

        private void InitializeAutoHide()
        {
            _autoHideTimer = new Timer { Interval = Math.Max(50, _config.AutoHideDelay) };
            _autoHideTimer.Tick += AutoHideTimer_Tick;
        }

        /// <summary>Starts or stops the retract timer to match the current configuration.</summary>
        private void UpdateAutoHideState()
        {
            if (_autoHideTimer == null)
                return;

            if (!_config.AutoHide)
            {
                _autoHideTimer.Stop();
                if (_isRetracted)
                    Expand();
                return;
            }

            _autoHideTimer.Interval = Math.Max(50, _config.AutoHideDelay);
            RestartAutoHideCountdown();
        }

        /// <summary>The pointer is away; begin counting down to retract.</summary>
        private void RestartAutoHideCountdown()
        {
            if (!_config.AutoHide || _autoHideTimer == null)
                return;

            _autoHideTimer.Stop();
            _autoHideTimer.Start();
        }

        private void AutoHideTimer_Tick(object sender, EventArgs e)
        {
            _autoHideTimer.Stop();

            if (!_config.AutoHide || _isRetracted || IsDisposed)
                return;

            // Never retract out from under the pointer or while the user is mid-drag.
            if (ClientRectangle.Contains(PointToClient(Cursor.Position)) || _isDragging)
            {
                RestartAutoHideCountdown();
                return;
            }

            Retract();
        }

        private void Retract()
        {
            if (_isRetracted)
                return;

            bool horizontal = _config.Orientation == Docks.DockOrientation.Horizontal;
            _expandedExtent = horizontal ? Height : Width;
            _isRetracted = true;

            // MinimumSize is set by UpdateDockSize to keep the dock at its natural extent; it has to
            // be released before the control can shrink below it.
            MinimumSize = horizontal
                ? new Size(MinimumSize.Width, RevealStripThickness)
                : new Size(RevealStripThickness, MinimumSize.Height);

            if (horizontal)
                Height = RevealStripThickness;
            else
                Width = RevealStripThickness;

            Invalidate();
        }

        private void Expand()
        {
            if (!_isRetracted)
                return;

            _isRetracted = false;

            if (_config.Orientation == Docks.DockOrientation.Horizontal)
                Height = _expandedExtent > 0 ? _expandedExtent : _config.DockHeight;
            else
                Width = _expandedExtent > 0 ? _expandedExtent : _config.DockHeight;

            UpdateDockSize();
            UpdateItemBounds();
            Invalidate();
        }

        /// <summary>
        /// Reveals the dock when the pointer reaches it, and restarts the countdown when it leaves.
        /// </summary>
        private void HandleAutoHidePointer(bool pointerInside)
        {
            if (!_config.AutoHide)
                return;

            if (pointerInside)
            {
                _autoHideTimer?.Stop();
                Expand();
            }
            else
            {
                RestartAutoHideCountdown();
            }
        }

        private void DisposeAutoHide()
        {
            if (_autoHideTimer == null)
                return;

            _autoHideTimer.Stop();
            _autoHideTimer.Tick -= AutoHideTimer_Tick;
            _autoHideTimer.Dispose();
            _autoHideTimer = null;
        }
    }
}
