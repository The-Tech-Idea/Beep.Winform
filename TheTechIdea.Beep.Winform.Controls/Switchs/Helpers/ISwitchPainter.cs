using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers
{
    /// <summary>
    /// Interface for BeepSwitch painters
    /// Each painter provides a unique visual style for the switch control
    /// </summary>
    public interface ISwitchPainter
    {
        /// <summary>
        /// Calculate layout metrics and register hit areas
        /// This method determines positions for track, thumb, and labels
        /// </summary>
        /// <param name="owner">The BeepSwitch control</param>
        /// <param name="metrics">Metrics object to populate</param>
        void CalculateLayout(BeepSwitch owner, SwitchMetrics metrics);
        
        /// <summary>
        /// Paint the switch track (background capsule/rectangle)
        /// Uses BackgroundPainterFactory and BorderPainterFactory for consistency
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="owner">The BeepSwitch control</param>
        /// <param name="trackPath">Path for the track shape</param>
        /// <param name="state">Current switch state</param>
        void PaintTrack(Graphics g, BeepSwitch owner, GraphicsPath trackPath, SwitchState state);
        
        /// <summary>
        /// Paint the toggle thumb/knob
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="owner">The BeepSwitch control</param>
        /// <param name="thumbRect">Rectangle for thumb position</param>
        /// <param name="state">Current switch state</param>
        void PaintThumb(Graphics g, BeepSwitch owner, Rectangle thumbRect, SwitchState state);
        
        /// <summary>
        /// Paint On/Off labels
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="owner">The BeepSwitch control</param>
        /// <param name="onLabelRect">Rectangle for On label</param>
        /// <param name="offLabelRect">Rectangle for Off label</param>
        void PaintLabels(Graphics g, BeepSwitch owner, Rectangle onLabelRect, Rectangle offLabelRect);
        
        /// <summary>
        /// Get animation duration in milliseconds for this painter style
        /// </summary>
        int GetAnimationDuration();
        
        /// <summary>
        /// Get track size ratio (width:height) for this painter style
        /// iOS uses 51:31 (~1.65), Material uses 52:32 (~1.625)
        /// </summary>
        float GetTrackSizeRatio();
        
        /// <summary>
        /// Get thumb size as percentage of track height
        /// Typically 0.85-0.95 (85-95% of track height)
        /// </summary>
        float GetThumbSizeRatio();
    }
}

