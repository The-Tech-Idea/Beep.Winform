using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// List with icons/flags next to items (from image 2 - country selector)
    /// </summary>
    internal class WithIconsListBoxPainter : OutlinedListBoxPainter
    {
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            int left = Scale(ListBoxTokens.ItemPaddingH + (ListBoxTokens.IconTextGap / 2));
            int top = Scale(ListBoxTokens.ItemPaddingV);
            int right = Scale(ListBoxTokens.ItemPaddingH);
            int bottom = Scale(ListBoxTokens.ItemPaddingV);
            return new System.Windows.Forms.Padding(left, top, right, bottom);
        }
        
        public override int GetPreferredItemHeight()
        {
            return Scale(ListBoxTokens.ItemHeightCompact);
        }
    }
}
