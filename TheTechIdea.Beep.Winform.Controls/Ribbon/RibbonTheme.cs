namespace TheTechIdea.Beep.Winform.Controls
{
    public class RibbonTheme
    {
        public Color Background { get; set; } = SystemColors.ControlLight;
        public Color TabActiveBack { get; set; } = Color.White;
        public Color TabInactiveBack { get; set; } = Color.FromArgb(235,235,235);
        public Color TabBorder { get; set; } = Color.FromArgb(180,180,180);
        public Color GroupBack { get; set; } = Color.FromArgb(245,245,245);
        public Color GroupBorder { get; set; } = Color.FromArgb(200,200,200);
        public Color Text { get; set; } = SystemColors.ControlText;
        public Color QuickAccessBack { get; set; } = Color.FromArgb(228, 240, 252);
        public Color QuickAccessBorder { get; set; } = Color.FromArgb(120, 160, 200);
    }
}