using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = SurfaceColor;
            this.DisabledBackColor = SurfaceColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.BlockquoteBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.InlineCodeBackgroundColor = SurfaceColor;
            this.CodeBlockBackgroundColor = SurfaceColor;
            this.CodeBlockBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.RowBackColor = SurfaceColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = SurfaceColor;
            this.SelectedRowBackColor = SurfaceColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = SurfaceColor;
            this.SubLabelBackColor = SurfaceColor;
            this.SubLabelHoverBackColor = SurfaceColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = SurfaceColor;
            this.HoverLinkColor = SurfaceColor;
            this.PrimaryTextColor = PrimaryColor;
            this.SecondaryTextColor = ActiveBorderColor;
            this.AccentTextColor = AccentColor;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.ApplyThemeToIcons = false;
            this.ShadowColor = SurfaceColor;
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.ScrollBarBackColor = SurfaceColor;
            this.ScrollBarThumbColor = SurfaceColor;
            this.ScrollBarTrackColor = SurfaceColor;
            this.ScrollBarHoverThumbColor = SurfaceColor;
            this.ScrollBarHoverTrackColor = SurfaceColor;
            this.ScrollBarActiveThumbColor = SurfaceColor;
            this.ScrollListBackColor = SurfaceColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = SurfaceColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = SurfaceColor;
            this.ScrollListItemSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ScrollListItemBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = SurfaceColor;
            this.StarRatingBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StarRatingFillColor = SurfaceColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = SurfaceColor;
            this.StarRatingHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = SurfaceColor;
            this.StarRatingSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = SurfaceColor;
            this.ActiveTabBackColor = SurfaceColor;
            this.ActiveTabForeColor = ForeColor;
            this.InactiveTabBackColor = SurfaceColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = SurfaceColor;
            this.TestimonialTextColor = ForeColor;
            this.TestimonialNameColor = SurfaceColor;
            this.TestimonialDetailsColor = SurfaceColor;
            this.TestimonialDateColor = SurfaceColor;
            this.TestimonialRatingColor = SurfaceColor;
            this.TestimonialStatusColor = SurfaceColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}
