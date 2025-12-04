using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Models
{
    /// <summary>
    /// Layout metrics for BeepSwitch
    /// Calculated by painters and used for rendering and hit testing
    /// </summary>
    public class SwitchMetrics
    {
        #region Track Dimensions
        
        /// <summary>
        /// Width of the switch track (capsule background)
        /// </summary>
        public int TrackWidth { get; set; }
        
        /// <summary>
        /// Height of the switch track
        /// </summary>
        public int TrackHeight { get; set; }
        
        /// <summary>
        /// Rectangle for the track area
        /// </summary>
        public Rectangle TrackRect { get; set; }
        
        #endregion

        #region Thumb Dimensions
        
        /// <summary>
        /// Size of the toggle thumb/knob (circular)
        /// </summary>
        public int ThumbSize { get; set; }
        
        /// <summary>
        /// Thumb position when switch is OFF
        /// </summary>
        public Rectangle ThumbOffRect { get; set; }
        
        /// <summary>
        /// Thumb position when switch is ON
        /// </summary>
        public Rectangle ThumbOnRect { get; set; }
        
        /// <summary>
        /// Current thumb position (interpolated during animation)
        /// </summary>
        public Rectangle ThumbCurrentRect { get; set; }
        
        #endregion

        #region Label Positions
        
        /// <summary>
        /// Rectangle for On label text
        /// </summary>
        public Rectangle OnLabelRect { get; set; }
        
        /// <summary>
        /// Rectangle for Off label text
        /// </summary>
        public Rectangle OffLabelRect { get; set; }
        
        #endregion

        #region Spacing
        
        /// <summary>
        /// General padding around control
        /// </summary>
        public int Padding { get; set; } = 8;
        
        /// <summary>
        /// Padding between labels and track
        /// </summary>
        public int LabelPadding { get; set; } = 8;
        
        /// <summary>
        /// Padding inside track (between track edge and thumb)
        /// </summary>
        public int TrackPadding { get; set; } = 2;
        
        #endregion

        #region Animation
        
        /// <summary>
        /// Animation progress (0.0 = fully Off, 1.0 = fully On)
        /// </summary>
        public float AnimationProgress { get; set; } = 0f;
        
        #endregion
    }
}

