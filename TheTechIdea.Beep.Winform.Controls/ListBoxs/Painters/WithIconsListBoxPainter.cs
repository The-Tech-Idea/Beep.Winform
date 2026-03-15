using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// List with icons/flags next to items (from image 2 - country selector)
    /// </summary>
    internal class WithIconsListBoxPainter : OutlinedListBoxPainter
    {
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(Scale(16), Scale(6), Scale(12), Scale(6));
        }
        
        public override int GetPreferredItemHeight()
        {
            return Scale(40);
        }
    }
}
