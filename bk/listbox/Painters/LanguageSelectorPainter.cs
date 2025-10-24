using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Language/locale selector with flags (from image 5)
    /// </summary>
    internal class LanguageSelectorPainter : WithIconsListBoxPainter
    {
        // Inherits icon rendering from WithIconsListBoxPainter
        public override bool SupportsSearch() => true;
    }
}
