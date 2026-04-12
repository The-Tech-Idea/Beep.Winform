using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class TabStripPaintContext
    {
        public Control OwnerControl { get; }
        public IBeepTheme Theme { get; }
        public Font TextFont { get; }
        public Font BadgeFont { get; }
        public DocumentTabStyle TabStyle { get; }
        public TabCloseMode CloseMode { get; }
        public TabColorMode ColorMode { get; }
        public TabDensityMode Density { get; }
        internal TabResponsiveMode ResponsiveMode { get; }
        public int HoverTabIndex { get; }
        public bool HoverClose { get; }
        public bool IsVertical { get; }
        public int TabRadius { get; }
        public int Scale(int value) => DpiScalingHelper.ScaleValue(value, OwnerControl);

        internal TabStripPaintContext(Control owner, IBeepTheme theme, Font textFont, DocumentTabStyle style,
            TabCloseMode closeMode, TabColorMode colorMode, TabDensityMode density,
            TabResponsiveMode responsiveMode, int hoverTabIndex, bool hoverClose, bool isVertical, int tabRadius)
        {
            OwnerControl = owner;
            Theme = theme;
            TextFont = textFont;
            TabStyle = style;
            CloseMode = closeMode;
            ColorMode = colorMode;
            Density = density;
            ResponsiveMode = responsiveMode;
            HoverTabIndex = hoverTabIndex;
            HoverClose = hoverClose;
            IsVertical = isVertical;
            TabRadius = tabRadius;
            var badgeSize = Math.Max(6f, textFont.SizeInPoints - 2f);
            BadgeFont = BeepFontManager.GetCachedFont(textFont.FontFamily.Name, badgeSize, FontStyle.Bold);
        }

        public bool IsTabActive(BeepDocumentTab tab) => tab.IsActive;
        public bool IsTabHovered(int index) => index == HoverTabIndex;
        public bool ShowCloseButton(BeepDocumentTab tab, int index)
        {
            if (tab.CloseRect.IsEmpty || !tab.CanClose) return false;
            return CloseMode switch
            {
                TabCloseMode.Always => true,
                TabCloseMode.OnHover => IsTabHovered(index) || IsTabActive(tab),
                TabCloseMode.ActiveOnly => IsTabActive(tab),
                _ => false
            };
        }

        public Color GetTabBackground(BeepDocumentTab tab, int index)
        {
            bool active = IsTabActive(tab);
            bool hovered = IsTabHovered(index);
            return TabStyle switch
            {
                DocumentTabStyle.VSCode => active ? Theme.BackgroundColor
                    : hovered ? BeepDocumentTabStrip.Blend(Theme.PanelBackColor, Theme.BorderColor, 0.2f)
                    : Theme.PanelBackColor,
                DocumentTabStyle.Underline => Theme.PanelBackColor,
                DocumentTabStyle.Pill => Theme.PanelBackColor,
                DocumentTabStyle.Flat => active ? Theme.BackgroundColor
                    : hovered ? BeepDocumentTabStrip.Blend(Theme.PanelBackColor, Theme.BorderColor, 0.2f)
                    : Theme.PanelBackColor,
                DocumentTabStyle.Rounded => active ? Theme.BackgroundColor
                    : hovered ? BeepDocumentTabStrip.Blend(Theme.PanelBackColor, Theme.BorderColor, 0.25f)
                    : Theme.PanelBackColor,
                DocumentTabStyle.Trapezoid => active ? Theme.BackgroundColor
                    : hovered ? BeepDocumentTabStrip.Blend(Theme.PanelBackColor, Theme.BorderColor, 0.2f)
                    : Theme.PanelBackColor,
                DocumentTabStyle.Office => active ? Theme.BackgroundColor
                    : hovered ? BeepDocumentTabStrip.Blend(Theme.PanelBackColor, Theme.BorderColor, 0.18f)
                    : Theme.PanelBackColor,
                DocumentTabStyle.Fluent => active ? Theme.BackgroundColor
                    : hovered ? Color.FromArgb(32, Theme.ForeColor)
                    : Color.FromArgb(16, Theme.ForeColor),
                _ => active ? Theme.BackgroundColor
                    : hovered ? BeepDocumentTabStrip.Blend(Theme.PanelBackColor, Color.White, 0.08f)
                    : Theme.PanelBackColor
            };
        }

        public Color GetTextColor(BeepDocumentTab tab)
        {
            Color baseColor = IsTabActive(tab) ? Theme.ForeColor : Theme.SecondaryTextColor;
            return IsTabActive(tab) ? baseColor : Color.FromArgb(DocTokens.InactiveTabTextAlpha, baseColor);
        }

        public Color GetAccentColor(BeepDocumentTab tab)
        {
            return tab.AccentColor == Color.Empty ? Theme.PrimaryColor : tab.AccentColor;
        }
    }
}
