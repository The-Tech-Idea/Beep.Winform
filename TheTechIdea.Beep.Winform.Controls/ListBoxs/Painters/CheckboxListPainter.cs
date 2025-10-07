using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// List with checkboxes for multi-select (from images 1, 3)
    /// </summary>
    internal class CheckboxListPainter : OutlinedListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(12, 6, 12, 6);
        }
    }
}
