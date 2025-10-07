using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Compact list with smaller items for high-density display
    /// </summary>
    internal class CompactListPainter : MinimalListBoxPainter
    {
        public override int GetPreferredItemHeight()
        {
            return 24; // Smaller height for compact display
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(6, 2, 6, 2);
        }
    }
}
