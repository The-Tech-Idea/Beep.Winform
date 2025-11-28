using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = PanelGradiantStartColor;
            this.DisabledBackColor = SurfaceColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = InactiveBorderColor;
            this.BlockquoteBorderColor = InactiveBorderColor;
            this.InlineCodeBackgroundColor = SurfaceColor;
            this.CodeBlockBackgroundColor = SurfaceColor;
            this.CodeBlockBorderColor = InactiveBorderColor;
            this.RowBackColor = SurfaceColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = PanelGradiantStartColor;
            this.SelectedRowBackColor = PrimaryColor;
            this.SelectedRowForeColor = OnPrimaryColor;
            this.SubLabelForColor = ForeColor;
            this.SubLabelBackColor = SurfaceColor;
            this.SubLabelHoverBackColor = PanelGradiantStartColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = SecondaryColor;
            this.HoverLinkColor = PrimaryColor;
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
            this.ScrollBarBackColor = SurfaceColor;
            this.ScrollBarThumbColor = SurfaceColor;
            this.ScrollBarTrackColor = PanelBackColor;
            this.ScrollBarHoverThumbColor = PanelGradiantStartColor;
            this.ScrollBarHoverTrackColor = PanelGradiantMiddleColor;
            this.ScrollBarActiveThumbColor = PrimaryColor;
            this.ScrollListBackColor = SurfaceColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = InactiveBorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = PanelGradiantStartColor;
            this.ScrollListItemSelectedForeColor = OnPrimaryColor;
            this.ScrollListItemSelectedBackColor = PrimaryColor;
            this.ScrollListItemSelectedBorderColor = PrimaryColor;
            this.ScrollListItemBorderColor = InactiveBorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = SurfaceColor;
            this.StarRatingBorderColor = InactiveBorderColor;
            this.StarRatingFillColor = PrimaryColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelGradiantStartColor;
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
            this.TestimonialNameColor = ForeColor;
            this.TestimonialDetailsColor = ForeColor;
            this.TestimonialDateColor = ForeColor;
            this.TestimonialRatingColor = PrimaryColor;
            this.TestimonialStatusColor = SecondaryColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}