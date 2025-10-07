using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Card-style list with elevated items (taller than standard filled)
    /// </summary>
    internal class CardListPainter : FilledListBoxPainter
    {
        public override int GetPreferredItemHeight()
        {
            return 60; // Taller for card appearance
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(16, 8, 16, 8);
        }
    }
}
