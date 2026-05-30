using System;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Factory for creating button painters based on style
    /// </summary>
    public static class ButtonPainterFactory
    {
        public static IAdvancedButtonPainter CreatePainter(AdvancedButtonStyle style)
        {
            return CreatePainter(style, NewsBannerVariant.Auto, ContactVariant.Auto, ChevronVariant.Auto);
        }

        public static IAdvancedButtonPainter CreatePainter(
            AdvancedButtonStyle style,
            NewsBannerVariant newsBannerVariant,
            ContactVariant contactVariant,
            ChevronVariant chevronVariant)
        {
            // Contact/Chevron variants are currently selected within their painter classes.
            _ = contactVariant;
            _ = chevronVariant;

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
                AdvancedButtonStyle.IconText => new SplitColorButtonPainter(),
                AdvancedButtonStyle.Chip => new ChipButtonPainter(),
                AdvancedButtonStyle.Contact => new ContactButtonPainter(),
                AdvancedButtonStyle.NavigationChevron => new NavigationChevronButtonPainter(),
                AdvancedButtonStyle.NeonGlow => new NeonGlowButtonPainter(),
                AdvancedButtonStyle.NewsBanner => CreateNewsBannerPainter(newsBannerVariant),
                AdvancedButtonStyle.FlatWeb => new FlatWebButtonPainter(),
                AdvancedButtonStyle.LowerThird => new LowerThirdButtonPainter(),
                AdvancedButtonStyle.StickerLabel => new StickerLabelButtonPainter(),
                
                // Image 1 - Column 1
                AdvancedButtonStyle.OutlinePill => new OutlinePillButtonPainter(),
                AdvancedButtonStyle.AccentEdge => new AccentEdgeButtonPainter(),
                AdvancedButtonStyle.SpeechBubble => new SpeechBubbleButtonPainter(),
                AdvancedButtonStyle.GradientSubButton => new GradientSubButtonPainter(),
                AdvancedButtonStyle.MiniGradientPill => new MiniGradientPillButtonPainter(),
                
                // Image 1 - Column 2
                AdvancedButtonStyle.GradientSpeechBubble => new GradientSpeechBubblePainter(),
                AdvancedButtonStyle.SplitGradient => new SplitGradientButtonPainter(),
                
                // Image 1 - Column 3
                AdvancedButtonStyle.IconCircleRight => new IconCircleRightButtonPainter(),
                AdvancedButtonStyle.SplitIconLeft => new SplitIconLeftButtonPainter(),
                
                _ => new SolidButtonPainter()
            };
        }

        private static IAdvancedButtonPainter CreateNewsBannerPainter(NewsBannerVariant variant)
        {
            return variant switch
            {
                // Circle badge styles (24, Live, Sport, etc.)
                NewsBannerVariant.CircleBadgeLeft => new NewsCircleBadgePainter(),
                NewsBannerVariant.BNLiveCircleBanner => new NewsCircleBadgePainter(),
                NewsBannerVariant.NewsLiveCirclePink => new NewsCircleBadgePainter(),
                NewsBannerVariant.SportNewsCirclePill => new NewsCircleBadgePainter(),
                NewsBannerVariant.TwentyFourTVNews => new NewsCircleBadgePainter(),
                
                // Bar/Badge styles (Breaking News, Live, etc.)
                NewsBannerVariant.RectangleBadgeLeft => new NewsBarPainter(),
                NewsBannerVariant.FlagLeft => new NewsBarPainter(),
                NewsBannerVariant.BNSquareGreenBanner => new NewsBarPainter(),
                NewsBannerVariant.LiveWorldNewsPill => new NewsBarPainter(),
                NewsBannerVariant.MorningLiveYellowBanner => new NewsBarPainter(),
                
                // Arrow/Chevron styles
                NewsBannerVariant.ChevronRight => new NewsArrowPainter(),
                NewsBannerVariant.ChevronBoth => new NewsArrowPainter(),
                NewsBannerVariant.TwentyFourWorldNewsHexagon => new NewsArrowPainter(),
                
                // Angled/Slanted styles
                NewsBannerVariant.AngledBadgeLeft => new NewsAngledPainter(),
                NewsBannerVariant.AngledTwoTone => new NewsAngledPainter(),
                NewsBannerVariant.SlantedEdges => new NewsAngledPainter(),
                
                // Icon badge styles (Globe, Lightning, etc.)
                NewsBannerVariant.PillWithIcon => new NewsIconBadgePainter(),
                NewsBannerVariant.BreakingNewsGlobe => new NewsIconBadgePainter(),
                NewsBannerVariant.LightningBreakingNews => new NewsIconBadgePainter(),
                NewsBannerVariant.LightningBreakingNewsLive => new NewsIconBadgePainter(),
                NewsBannerVariant.WorldNewsGlobePill => new NewsIconBadgePainter(),
                
                // Default
                _ => new NewsBarPainter()
            };
        }
    }
}
