using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// Factory for creating checkbox painters based on style
    /// </summary>
    public static class CheckBoxPainterFactory
    {
        /// <summary>
        /// Gets a painter instance for the specified checkbox style
        /// </summary>
        public static ICheckBoxPainter GetPainter(CheckBoxStyle style)
        {
            return style switch
            {
                CheckBoxStyle.Material3 => new Material3CheckBoxPainter(),
                CheckBoxStyle.Modern => new ModernCheckBoxPainter(),
                CheckBoxStyle.Classic => new ClassicCheckBoxPainter(),
                CheckBoxStyle.Minimal => new MinimalCheckBoxPainter(),
                CheckBoxStyle.iOS => new iOSCheckBoxPainter(),
                CheckBoxStyle.Fluent2 => new Fluent2CheckBoxPainter(),
                CheckBoxStyle.Switch => new SwitchCheckBoxPainter(),
                CheckBoxStyle.Button => new ButtonCheckBoxPainter(),
                _ => new Material3CheckBoxPainter()
            };
        }
    }
}
