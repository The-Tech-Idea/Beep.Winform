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
                AdvancedButtonStyle.IconText => new IconTextButtonPainter(),
                AdvancedButtonStyle.Chip => new ChipButtonPainter(),
                AdvancedButtonStyle.Contact => new ContactButtonPainter(),
                AdvancedButtonStyle.NavigationChevron => new NavigationChevronButtonPainter(),
                AdvancedButtonStyle.NeonGlow => new NeonGlowButtonPainter(),
                AdvancedButtonStyle.NewsBanner => CreateNewsBannerPainter(newsBannerVariant),
                AdvancedButtonStyle.FlatWeb => new FlatWebButtonPainter(),
                AdvancedButtonStyle.LowerThird => new LowerThirdButtonPainter(),
                AdvancedButtonStyle.StickerLabel => new StickerLabelButtonPainter(),
                _ => new SolidButtonPainter()
            };
        }

        private static IAdvancedButtonPainter CreateNewsBannerPainter(NewsBannerVariant variant)
        {
            return variant switch
            {
                NewsBannerVariant.CircleBadgeLeft => new CircleBadge24NewsPainter(),
                NewsBannerVariant.RectangleBadgeLeft => new BreakingNewsRectanglePainter(),
                NewsBannerVariant.AngledBadgeLeft => new CircleBadgeAngledBannerPainter(),
                NewsBannerVariant.ChevronRight => new ChevronRightNewsPainter(),
                NewsBannerVariant.ChevronBoth => new HexagonWorldNewsPainter(),
                NewsBannerVariant.FlagLeft => new LiveBreakingNewsPainter(),
                NewsBannerVariant.AngledTwoTone => new PinkWhiteAngledBannerPainter(),
                NewsBannerVariant.SlantedEdges => new FakeNewsChevronPainter(),
                NewsBannerVariant.PillWithIcon => new IconCirclePillNewsPainter(),
                NewsBannerVariant.BNLiveCircleBanner => new BNLiveCircleBannerPainter(),
                NewsBannerVariant.BNSquareGreenBanner => new BNSquareGreenBannerPainter(),
                NewsBannerVariant.BreakingNewsGlobe => new BreakingNewsGlobePainter(),
                NewsBannerVariant.LightningBreakingNews => new LightningBreakingNewsPainter(),
                NewsBannerVariant.LightningBreakingNewsLive => new LightningBreakingNewsLivePainter(),
                NewsBannerVariant.LiveWorldNewsPill => new LiveWorldNewsPillPainter(),
                NewsBannerVariant.MorningLiveYellowBanner => new MorningLiveYellowBannerPainter(),
                NewsBannerVariant.NewsLiveCirclePink => new NewsLiveCirclePinkPainter(),
                NewsBannerVariant.SportNewsCirclePill => new SportNewsCirclePillPainter(),
                NewsBannerVariant.TwentyFourTVNews => new TwentyFourTVNewsPainter(),
                NewsBannerVariant.TwentyFourWorldNewsHexagon => new TwentyFourWorldNewsHexagonPainter(),
                NewsBannerVariant.WorldNewsGlobePill => new WorldNewsGlobePillPainter(),
                _ => new NewsBannerButtonPainter()
            };
        }
    }
}
