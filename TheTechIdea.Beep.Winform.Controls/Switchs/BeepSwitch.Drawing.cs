using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSwitch - Drawing and painting
    /// </summary>
    public partial class BeepSwitch
    {
        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and other containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            if (_painter == null)
            {
                InitializePainter();
                if (_painter == null) return;
            }
            
            // Update metrics before painting
            UpdateMetrics();
            
            // Get current state
            SwitchState currentState = GetCurrentState();
            
            // Create track path based on orientation
            using (GraphicsPath trackPath = CreateTrackPath())
            {
                // Paint track (background) - uses BackgroundPainterFactory
                _painter.PaintTrack(g, this, trackPath, currentState);
                
                // Paint thumb (toggle knob) - uses BorderPainterFactory
                _painter.PaintThumb(g, this, _metrics.ThumbCurrentRect, currentState);
                
                // Paint labels
                _painter.PaintLabels(g, this, _metrics.OnLabelRect, _metrics.OffLabelRect);
            }
        }

        private GraphicsPath CreateTrackPath()
        {
            GraphicsPath path = new GraphicsPath();
            
            // Create pill/capsule shape
            if (Orientation == Switchs.Models.SwitchOrientation.Horizontal)
            {
                int radius = _metrics.TrackHeight / 2;
                path.AddArc(_metrics.TrackRect.X, _metrics.TrackRect.Y, _metrics.TrackHeight, _metrics.TrackHeight, 90, 180);
                path.AddArc(_metrics.TrackRect.Right - _metrics.TrackHeight, _metrics.TrackRect.Y, _metrics.TrackHeight, _metrics.TrackHeight, 270, 180);
            }
            else
            {
                int radius = _metrics.TrackWidth / 2;
                path.AddArc(_metrics.TrackRect.X, _metrics.TrackRect.Y, _metrics.TrackWidth, 2 * radius, 180, 180);
                path.AddArc(_metrics.TrackRect.X, _metrics.TrackRect.Bottom - 2 * radius, _metrics.TrackWidth, 2 * radius, 0, 180);
            }
            
            path.CloseFigure();
            return path;
        }

        private SwitchState GetCurrentState()
        {
            // Combine checked state with interaction state
            bool isOn = _checked || _animProgress > 0.5f;
            
            if (!Enabled)
                return isOn ? SwitchState.On_Disabled : SwitchState.Off_Disabled;
            
            if (IsPressed)
                return isOn ? SwitchState.On_Pressed : SwitchState.Off_Pressed;
            
            if (IsHovered)
                return isOn ? SwitchState.On_Hover : SwitchState.Off_Hover;
            
            if (Focused)
                return isOn ? SwitchState.On_Focused : SwitchState.Off_Focused;
            
            return isOn ? SwitchState.On_Normal : SwitchState.Off_Normal;
        }
    }
}

