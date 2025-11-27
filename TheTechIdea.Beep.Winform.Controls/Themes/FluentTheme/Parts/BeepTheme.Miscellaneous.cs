using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = AccentColor;
            this.DisabledBackColor = DisabledColor;
            this.DisabledForeColor = OnDisabledColor;
            this.DisabledBorderColor = DisabledBorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = SurfaceColor;
            this.CodeBlockBackgroundColor = SurfaceColor;
            this.CodeBlockBorderColor = BorderColor;
            this.CompanyPopoverBackgroundColor = SurfaceColor;
            this.CompanyTitleColor = ForeColor;
            this.CompanySubtitleColor = SecondaryColor;
            this.CompanyDescriptionColor = ForeColor;
            this.CompanyLinkColor = AccentColor;
            this.CompanyButtonBackgroundColor = PrimaryColor;
            this.CompanyButtonTextColor = OnPrimaryColor;
            this.CompanyDropdownBackgroundColor = SurfaceColor;
            this.CompanyDropdownTextColor = ForeColor;
            this.CompanyLogoBackgroundColor = SurfaceColor;
            this.RowBackColor = SurfaceColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = SecondaryColor;
            this.SelectedRowBackColor = PrimaryColor;
            this.SelectedRowForeColor = OnPrimaryColor;
            this.SubLabelForColor = ForeColor;
            this.SubLabelBackColor = SurfaceColor;
            this.SubLabelHoverBackColor = SecondaryColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = AccentColor;
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
            this.ScrollBarBackColor = SurfaceColor;
            this.ScrollBarThumbColor = AccentColor;
            this.ScrollBarTrackColor = SurfaceColor;
            this.ScrollBarHoverThumbColor = PrimaryColor;
            this.ScrollBarHoverTrackColor = SecondaryColor;
            this.ScrollBarActiveThumbColor = PrimaryColor;
            this.ScrollListBackColor = SurfaceColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = BorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = SecondaryColor;
            this.ScrollListItemSelectedForeColor = OnPrimaryColor;
            this.ScrollListItemSelectedBackColor = PrimaryColor;
            this.ScrollListItemSelectedBorderColor = PrimaryColor;
            this.ScrollListItemBorderColor = BorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = SurfaceColor;
            this.StarRatingBorderColor = BorderColor;
            this.StarRatingFillColor = AccentColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = SecondaryColor;
            this.StarRatingHoverBorderColor = ActiveBorderColor;
            this.StarRatingSelectedForeColor = OnPrimaryColor;
            this.StarRatingSelectedBackColor = PrimaryColor;
            this.StarRatingSelectedBorderColor = PrimaryColor;
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = SurfaceColor;
            this.ActiveTabBackColor = PrimaryColor;
            this.ActiveTabForeColor = OnPrimaryColor;
            this.InactiveTabBackColor = SurfaceColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = SurfaceColor;
            this.TestimonialTextColor = ForeColor;
            this.TestimonialNameColor = AccentColor;
            this.TestimonialDetailsColor = SecondaryColor;
            this.TestimonialDateColor = SecondaryColor;
            this.TestimonialRatingColor = AccentColor;
            this.TestimonialStatusColor = AccentColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}