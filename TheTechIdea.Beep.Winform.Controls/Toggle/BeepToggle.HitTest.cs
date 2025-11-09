using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Toggle
{
    /// <summary>
    /// Hit testing partial class for BeepToggle
    /// Manages mouse interaction detection using painter-specific regions
    /// </summary>
    public partial class BeepToggle
    {
        private ToggleHitRegion _currentHitRegion = ToggleHitRegion.None;
        private ToggleHitRegion _hoveredRegion = ToggleHitRegion.None;

        /// <summary>
        /// Gets the currently hovered region
        /// </summary>
        public ToggleHitRegion HoveredRegion => _hoveredRegion;

        /// <summary>
        /// Perform hit test using painter-specific regions
        /// </summary>
        public new bool HitTest(Point location)
        {
            if (_painter == null)
                return false;

            // Delegate hit testing to the painter
            bool isHit = _painter.HitTest(location);
            
            // Get detailed region for hover effects
            _currentHitRegion = _painter.GetHitRegion(location);
            
            // Update hover state if region changed
            if (_currentHitRegion != _hoveredRegion)
            {
                _hoveredRegion = _currentHitRegion;
                Invalidate();
            }

            return isHit;
        }

        /// <summary>
        /// Check if point is within the track region
        /// </summary>
        public bool IsPointOnTrack(Point location)
        {
            return _painter?.HitTestTrack(location) ?? false;
        }

        /// <summary>
        /// Check if point is within the thumb region
        /// </summary>
        public bool IsPointOnThumb(Point location)
        {
            return _painter?.HitTestThumb(location) ?? false;
        }

        /// <summary>
        /// Get the specific region at the point
        /// </summary>
        public ToggleHitRegion GetRegionAtPoint(Point location)
        {
            return _painter?.GetHitRegion(location) ?? ToggleHitRegion.None;
        }

        /// <summary>
        /// Override mouse move to update hover region
        /// </summary>
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (_painter != null)
            {
                var newRegion = _painter.GetHitRegion(e.Location);
                if (newRegion != _hoveredRegion)
                {
                    _hoveredRegion = newRegion;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Override mouse leave to clear hover region
        /// </summary>
        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            
            if (_hoveredRegion != ToggleHitRegion.None)
            {
                _hoveredRegion = ToggleHitRegion.None;
                Invalidate();
            }
        }

        /// <summary>
        /// Override mouse down to add ripple effect for Material styles
        /// </summary>
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Add ripple effect for Material Design painters
            if (_painter is Painters.MaterialPillTogglePainter materialPainter)
            {
                materialPainter.AddRipple(e.Location);
                Invalidate();
            }
        }
    }
}
