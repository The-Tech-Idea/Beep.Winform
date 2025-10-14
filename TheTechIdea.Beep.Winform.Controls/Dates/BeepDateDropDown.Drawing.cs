using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown
    {
        // Flag to trigger segment recalculation
        private bool _segmentsNeedRecalculation = false;
        
        /// <summary>
        /// Override DrawContent to customize dropdown appearance
        /// Most drawing is handled by base BeepTextBox
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            // Call base to draw text, border, image, etc.
            base.DrawContent(g);
            
            // Paint segment highlights if enabled
            if (EnableSegmentEditing && !string.IsNullOrEmpty(Text))
            {
                // Calculate segments if text changed
                if (_segments.Count == 0 || _segmentsNeedRecalculation)
                {
                    CalculateSegments();
                    _segmentsNeedRecalculation = false;
                }
                
                // Paint highlights
                PaintSegmentHighlights(g);
            }
        }
    }
}

