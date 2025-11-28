using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = BackgroundColor;
            this.DisabledBackColor = BackgroundColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = BorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = BackgroundColor;
            this.CodeBlockBackgroundColor = BackgroundColor;
            this.CodeBlockBorderColor = BorderColor;
            this.CompanyPopoverBackgroundColor = BackgroundColor;
            this.CompanyTitleColor = ForeColor;
            this.CompanySubtitleColor = ForeColor;
            this.CompanyDescriptionColor = ForeColor;
            this.CompanyLinkColor = PrimaryColor;
            this.CompanyButtonBackgroundColor = BackgroundColor;
            this.CompanyButtonTextColor = ForeColor;
            this.CompanyDropdownBackgroundColor = BackgroundColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = BackgroundColor;
            this.RowBackColor = BackgroundColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = BackgroundColor;
            this.SelectedRowBackColor = PanelBackColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = ForeColor;
            this.SubLabelBackColor = BackgroundColor;
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
            this.ShadowColor = BackgroundColor;
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.ScrollBarBackColor = BackgroundColor;
            this.ScrollBarThumbColor = ForeColor;
            this.ScrollBarTrackColor = BackgroundColor;
            this.ScrollBarHoverThumbColor = ThemeUtil.Lighten(ForeColor, 0.06);
            this.ScrollBarHoverTrackColor = BackgroundColor;
            this.ScrollBarActiveThumbColor = ForeColor;
            this.ScrollListBackColor = BackgroundColor;
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
            this.StarRatingFillColor = BackgroundColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = BackgroundColor;
            this.StarRatingHoverBorderColor = BorderColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = BackgroundColor;
            this.StarRatingSelectedBorderColor = BorderColor;
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = BackgroundColor;
            this.ActiveTabBackColor = BackgroundColor;
            this.ActiveTabForeColor = ForeColor;
            this.InactiveTabBackColor = BackgroundColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = BackgroundColor;
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