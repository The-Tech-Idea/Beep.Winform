using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = PanelGradiantMiddleColor;
            this.DisabledBackColor = BackgroundColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = BorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = BackgroundColor;
            this.CodeBlockBackgroundColor = BackgroundColor;
            this.CodeBlockBorderColor = BorderColor;
            this.RowBackColor = PanelBackColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = PanelBackColor;
            this.SelectedRowBackColor = PanelBackColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = ForeColor;
            this.SubLabelBackColor = PanelBackColor;
            this.SubLabelHoverBackColor = PanelGradiantMiddleColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = AccentColor;
            this.HoverLinkColor = PanelGradiantMiddleColor;
            this.PrimaryTextColor = ForeColor;
            this.SecondaryTextColor = ForeColor;
            this.AccentTextColor = AccentColor;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.ApplyThemeToIcons = false;
            this.ShadowColor = BackgroundColor;
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.ScrollBarBackColor = PanelBackColor;
            this.ScrollBarThumbColor = PanelBackColor;
            this.ScrollBarTrackColor = PanelBackColor;
            this.ScrollBarHoverThumbColor = PanelGradiantMiddleColor;
            this.ScrollBarHoverTrackColor = PanelGradiantMiddleColor;
            this.ScrollBarActiveThumbColor = PanelGradiantMiddleColor;
            this.ScrollListBackColor = PanelBackColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = BorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = PanelGradiantMiddleColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = PanelBackColor;
            this.ScrollListItemSelectedBorderColor = BorderColor;
            this.ScrollListItemBorderColor = BorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = BackgroundColor;
            this.StarRatingBorderColor = BorderColor;
            this.StarRatingFillColor = PrimaryColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelGradiantMiddleColor;
            this.StarRatingHoverBorderColor = BorderColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = PrimaryColor;
            this.StarRatingSelectedBorderColor = ActiveBorderColor;
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = BackgroundColor;
            this.ActiveTabBackColor = PanelBackColor;
            this.ActiveTabForeColor = ForeColor;
            this.InactiveTabBackColor = PanelBackColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = PanelBackColor;
            this.TestimonialTextColor = ForeColor;
            this.TestimonialNameColor = BackgroundColor;
            this.TestimonialDetailsColor = BackgroundColor;
            this.TestimonialDateColor = BackgroundColor;
            this.TestimonialRatingColor = BackgroundColor;
            this.TestimonialStatusColor = BackgroundColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}