using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = ThemeUtil.Lighten(SurfaceColor, 0.06);
            this.DisabledBackColor = BackgroundColor;
            this.DisabledForeColor = InactiveBorderColor;
            this.DisabledBorderColor = InactiveBorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = SurfaceColor;
            this.CodeBlockBackgroundColor = SurfaceColor;
            this.CodeBlockBorderColor = BorderColor;
            this.CompanyPopoverBackgroundColor = SurfaceColor;
            this.CompanyTitleColor = ForeColor;
            this.CompanySubtitleColor = SecondaryColor;
            this.CompanyDescriptionColor = ForeColor;
            this.CompanyLinkColor = PrimaryColor;
            this.CompanyButtonBackgroundColor = PrimaryColor;
            this.CompanyButtonTextColor = OnPrimaryColor;
            this.CompanyDropdownBackgroundColor = SurfaceColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = SurfaceColor;
            this.RowBackColor = BackgroundColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = SurfaceColor;
            this.SelectedRowBackColor = ThemeUtil.Lighten(SurfaceColor, 0.04);
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = SecondaryColor;
            this.SubLabelBackColor = BackgroundColor;
            this.SubLabelHoverBackColor = SurfaceColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = SecondaryColor;
            this.HoverLinkColor = PrimaryColor;
            this.PrimaryTextColor = PrimaryColor;
            this.SecondaryTextColor = SecondaryColor;
            this.AccentTextColor = AccentColor;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.ApplyThemeToIcons = false;
            this.ShadowColor = ShadowColor;
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.ScrollBarBackColor = BackgroundColor;
            this.ScrollBarThumbColor = BorderColor;
            this.ScrollBarTrackColor = SurfaceColor;
            this.ScrollBarHoverThumbColor = ActiveBorderColor;
            this.ScrollBarHoverTrackColor = SurfaceColor;
            this.ScrollBarActiveThumbColor = ActiveBorderColor;
            this.ScrollListBackColor = BackgroundColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = BorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = SurfaceColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.04);
            this.ScrollListItemSelectedBorderColor = ActiveBorderColor;
            this.ScrollListItemBorderColor = BorderColor;
            this.StarRatingForeColor = AccentColor;
            this.StarRatingBackColor = SurfaceColor;
            this.StarRatingBorderColor = ActiveBorderColor;
            this.StarRatingFillColor = AccentColor;
            this.StarRatingHoverForeColor = AccentColor;
            this.StarRatingHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.02);
            this.StarRatingHoverBorderColor = ActiveBorderColor;
            this.StarRatingSelectedForeColor = AccentColor;
            this.StarRatingSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.04);
            this.StarRatingSelectedBorderColor = ActiveBorderColor;
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = BackgroundColor;
            this.ActiveTabBackColor = SurfaceColor;
            this.ActiveTabForeColor = ForeColor;
            this.InactiveTabBackColor = BackgroundColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = SurfaceColor;
            this.TestimonialTextColor = ForeColor;
            this.TestimonialNameColor = ForeColor;
            this.TestimonialDetailsColor = SecondaryColor;
            this.TestimonialDateColor = SecondaryColor;
            this.TestimonialRatingColor = AccentColor;
            this.TestimonialStatusColor = SecondaryColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}
