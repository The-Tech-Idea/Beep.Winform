using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Gallery
{
    public sealed class RibbonGalleryRenderer
    {
        public RibbonTheme Theme { get; private set; } = new();
        public RibbonDensity Density { get; private set; } = RibbonDensity.Comfortable;

        public void ApplyTheme(RibbonTheme theme, RibbonDensity density)
        {
            Theme = theme ?? new RibbonTheme();
            Density = density;
        }

        public void StyleButton(Button button, bool selected)
        {
            if (button == null)
            {
                return;
            }

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = selected ? Theme.FocusBorder : Theme.GroupBorder;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseOverBackColor = Theme.HoverBack;
            button.FlatAppearance.MouseDownBackColor = Theme.PressedBack;
            button.BackColor = selected ? Theme.HoverBack : Theme.TabActiveBack;
            button.ForeColor = Theme.Text;
            button.Font = BeepThemesManager.ToFont(Theme.CommandTypography);
        }

        public Size GetTileSize(bool compact)
        {
            return Density switch
            {
                RibbonDensity.Compact => compact ? new Size(68, 24) : new Size(88, 26),
                RibbonDensity.Touch => compact ? new Size(92, 32) : new Size(110, 36),
                _ => compact ? new Size(78, 26) : new Size(96, 30)
            };
        }
    }
}
