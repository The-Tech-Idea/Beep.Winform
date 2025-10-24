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

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for LanguageSelector background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);
                if (isHovered)
                {
                    using (var hoverBrush = new SolidBrush(Color.FromArgb(50, Color.Gray)))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }
    }
}
