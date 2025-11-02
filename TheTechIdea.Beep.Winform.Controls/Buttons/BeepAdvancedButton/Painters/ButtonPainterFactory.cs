using System;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Factory for creating button painters based on style
    /// </summary>
    public static class ButtonPainterFactory
    {
        public static IAdvancedButtonPainter CreatePainter(AdvancedButtonStyle style)
        {
            return style switch
            {
                AdvancedButtonStyle.Solid => new SolidButtonPainter(),
                AdvancedButtonStyle.Icon => new IconButtonPainter(),
                AdvancedButtonStyle.Text => new TextButtonPainter(),
                AdvancedButtonStyle.Toggle => new ToggleButtonPainter(),
                AdvancedButtonStyle.FAB => new FABButtonPainter(),
                AdvancedButtonStyle.Ghost => new GhostButtonPainter(),
                AdvancedButtonStyle.Outlined => new OutlinedButtonPainter(),
                AdvancedButtonStyle.Link => new LinkButtonPainter(),
                AdvancedButtonStyle.Gradient => new GradientButtonPainter(),
                AdvancedButtonStyle.IconText => new IconTextButtonPainter(),
                AdvancedButtonStyle.Chip => new ChipButtonPainter(),
                AdvancedButtonStyle.Contact => new ContactButtonPainter(),
                AdvancedButtonStyle.NavigationChevron => new NavigationChevronButtonPainter(),
                AdvancedButtonStyle.NeonGlow => new NeonGlowButtonPainter(),
                AdvancedButtonStyle.NewsBanner => new NewsBannerButtonPainter(),
                _ => new SolidButtonPainter()
            };
        }
    }
}
