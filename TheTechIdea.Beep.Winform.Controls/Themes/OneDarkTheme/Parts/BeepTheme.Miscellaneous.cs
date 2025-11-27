using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = PanelGradiantMiddleColor;
            this.DisabledBackColor = PanelBackColor;
            this.DisabledForeColor = InactiveBorderColor;
            this.DisabledBorderColor = InactiveBorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = PanelBackColor;
            this.CodeBlockBackgroundColor = PanelBackColor;
            this.CodeBlockBorderColor = BorderColor;
            this.RowBackColor = PanelBackColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = PanelBackColor;
            this.SelectedRowBackColor = PanelGradiantMiddleColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = PanelBackColor;
            this.SubLabelBackColor = PanelBackColor;
            this.SubLabelHoverBackColor = PanelGradiantMiddleColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = PrimaryColor;
            this.HoverLinkColor = ThemeUtil.Lighten(PrimaryColor, 0.15);
            this.PrimaryTextColor = PrimaryColor;
            this.SecondaryTextColor = SecondaryColor;
            this.AccentTextColor = AccentColor;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.ApplyThemeToIcons = false;
            this.ShadowColor = InactiveBorderColor;
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.ScrollBarBackColor = PanelBackColor;
            this.ScrollBarThumbColor = InactiveBorderColor;
            this.ScrollBarTrackColor = PanelBackColor;
            this.ScrollBarHoverThumbColor = ActiveBorderColor;
            this.ScrollBarHoverTrackColor = PanelGradiantMiddleColor;
            this.ScrollBarActiveThumbColor = ActiveBorderColor;
            this.ScrollListBackColor = PanelBackColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = BorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = PanelGradiantMiddleColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = PanelGradiantMiddleColor;
            this.ScrollListItemSelectedBorderColor = ActiveBorderColor;
            this.ScrollListItemBorderColor = BorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = PanelBackColor;
            this.StarRatingBorderColor = BorderColor;
            this.StarRatingFillColor = PrimaryColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelGradiantMiddleColor;
            this.StarRatingHoverBorderColor = ActiveBorderColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = PanelGradiantMiddleColor;
            this.StarRatingSelectedBorderColor = ActiveBorderColor;
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = PanelBackColor;
            this.ActiveTabBackColor = PanelGradiantMiddleColor;
            this.ActiveTabForeColor = ForeColor;
            this.InactiveTabBackColor = PanelBackColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = PanelBackColor;
            this.TestimonialTextColor = ForeColor;
            this.TestimonialNameColor = ForeColor;
            this.TestimonialDetailsColor = ForeColor;
            this.TestimonialDateColor = ForeColor;
            this.TestimonialRatingColor = ForeColor;
            this.TestimonialStatusColor = PanelBackColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}