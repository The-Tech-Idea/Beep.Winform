using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Painters
{
    /// <summary>
    /// Factory for creating accordion painters based on style
    /// </summary>
    public static class AccordionPainterFactory
    {
        /// <summary>
        /// Gets a painter instance for the specified accordion style
        /// </summary>
        public static IAccordionPainter GetPainter(AccordionStyle style)
        {
            return style switch
            {
                AccordionStyle.Material3 => new Material3AccordionPainter(),
                AccordionStyle.Modern => new ModernAccordionPainter(),
                AccordionStyle.Classic => new ClassicAccordionPainter(),
                AccordionStyle.Minimal => new MinimalAccordionPainter(),
                AccordionStyle.iOS => new iOSAccordionPainter(),
                AccordionStyle.Fluent2 => new Fluent2AccordionPainter(),
                _ => new Material3AccordionPainter()
            };
        }
    }
}
