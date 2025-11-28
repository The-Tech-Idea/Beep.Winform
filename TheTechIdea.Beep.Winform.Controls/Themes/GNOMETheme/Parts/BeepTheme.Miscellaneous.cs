using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = SurfaceColor;
            this.DisabledBackColor = SurfaceColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = BorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = SurfaceColor;
            this.CodeBlockBackgroundColor = SurfaceColor;
            this.CodeBlockBorderColor = BorderColor;
            this.RowBackColor = SurfaceColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = SurfaceColor;
            this.SelectedRowBackColor = SurfaceColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = ForeColor;
            this.SubLabelBackColor = SurfaceColor;
            this.SubLabelHoverBackColor = PanelGradiantMiddleColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = AccentColor;
            this.HoverLinkColor = AccentColor;
            this.PrimaryTextColor = AccentColor;
            this.SecondaryTextColor = SecondaryColor;
            this.AccentTextColor = SuccessColor;
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
            this.ScrollBarBackColor = SurfaceColor;
            this.ScrollBarThumbColor = SurfaceColor;
            this.ScrollBarTrackColor = SurfaceColor;
            this.ScrollBarHoverThumbColor = PanelGradiantMiddleColor;
            this.ScrollBarHoverTrackColor = PanelGradiantMiddleColor;
            this.ScrollBarActiveThumbColor = PanelGradiantMiddleColor;
            this.ScrollListBackColor = SurfaceColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = BorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = PanelGradiantMiddleColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = SurfaceColor;
            this.ScrollListItemSelectedBorderColor = BorderColor;
            this.ScrollListItemBorderColor = BorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = SurfaceColor;
            this.StarRatingBorderColor = BorderColor;
            this.StarRatingFillColor = SurfaceColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelGradiantMiddleColor;
            this.StarRatingHoverBorderColor = BorderColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = SurfaceColor;
            this.StarRatingSelectedBorderColor = BorderColor;
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