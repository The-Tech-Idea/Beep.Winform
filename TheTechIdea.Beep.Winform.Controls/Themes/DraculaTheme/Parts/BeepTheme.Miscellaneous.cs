using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = PanelGradiantMiddleColor;
            this.DisabledBackColor = SurfaceColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = InactiveBorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = PanelBackColor;
            this.CodeBlockBackgroundColor = PanelBackColor;
            this.CodeBlockBorderColor = BorderColor;
            this.RowBackColor = BackgroundColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = PanelBackColor;
            this.SelectedRowBackColor = PanelGradiantMiddleColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = ForeColor;
            this.SubLabelBackColor = PanelBackColor;
            this.SubLabelHoverBackColor = PanelGradiantMiddleColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = AccentColor;
            this.HoverLinkColor = PrimaryColor;
            this.PrimaryTextColor = ForeColor;
            this.SecondaryTextColor = ForeColor;
            this.AccentTextColor = ForeColor;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.ApplyThemeToIcons = false;
            this.ShadowColor = PanelBackColor;
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
            this.ScrollListItemSelectedBackColor = PanelGradiantMiddleColor;
            this.ScrollListItemSelectedBorderColor = ActiveBorderColor;
            this.ScrollListItemBorderColor = BorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = PanelBackColor;
            this.StarRatingBorderColor = BorderColor;
            this.StarRatingFillColor = PrimaryColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelBackColor;
            this.StarRatingHoverBorderColor = BorderColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = PanelBackColor;
            this.StarRatingSelectedBorderColor = BorderColor;
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
            this.TestimonialStatusColor = ForeColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}