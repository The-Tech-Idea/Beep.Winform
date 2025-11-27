using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = PanelGradiantMiddleColor;
            this.DisabledBackColor = PanelGradiantMiddleColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = InactiveBorderColor;
            this.BlockquoteBorderColor = InactiveBorderColor;
            this.InlineCodeBackgroundColor = PanelGradiantMiddleColor;
            this.CodeBlockBackgroundColor = PanelGradiantMiddleColor;
            this.CodeBlockBorderColor = InactiveBorderColor;
            this.RowBackColor = PanelGradiantMiddleColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = PanelGradiantMiddleColor;
            this.SelectedRowBackColor = PanelGradiantMiddleColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = PanelGradiantMiddleColor;
            this.SubLabelBackColor = PanelGradiantMiddleColor;
            this.SubLabelHoverBackColor = PanelGradiantMiddleColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = PanelGradiantMiddleColor;
            this.HoverLinkColor = PanelGradiantMiddleColor;
            this.PrimaryTextColor = ForeColor;
            this.SecondaryTextColor = ForeColor;
            this.AccentTextColor = ForeColor;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.ApplyThemeToIcons = false;
            this.ShadowColor = PanelGradiantMiddleColor;
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.ScrollBarBackColor = PanelGradiantMiddleColor;
            this.ScrollBarThumbColor = PanelGradiantMiddleColor;
            this.ScrollBarTrackColor = PanelGradiantMiddleColor;
            this.ScrollBarHoverThumbColor = PanelGradiantMiddleColor;
            this.ScrollBarHoverTrackColor = PanelGradiantMiddleColor;
            this.ScrollBarActiveThumbColor = PanelGradiantMiddleColor;
            this.ScrollListBackColor = PanelGradiantMiddleColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = InactiveBorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = PanelGradiantMiddleColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = PanelGradiantMiddleColor;
            this.ScrollListItemSelectedBorderColor = InactiveBorderColor;
            this.ScrollListItemBorderColor = InactiveBorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = PanelGradiantMiddleColor;
            this.StarRatingBorderColor = InactiveBorderColor;
            this.StarRatingFillColor = PanelGradiantMiddleColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelGradiantMiddleColor;
            this.StarRatingHoverBorderColor = InactiveBorderColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = PanelGradiantMiddleColor;
            this.StarRatingSelectedBorderColor = InactiveBorderColor;
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = PanelGradiantMiddleColor;
            this.ActiveTabBackColor = PanelGradiantMiddleColor;
            this.ActiveTabForeColor = ForeColor;
            this.InactiveTabBackColor = PanelGradiantMiddleColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = PanelGradiantMiddleColor;
            this.TestimonialTextColor = ForeColor;
            this.TestimonialNameColor = PanelGradiantMiddleColor;
            this.TestimonialDetailsColor = PanelGradiantMiddleColor;
            this.TestimonialDateColor = PanelGradiantMiddleColor;
            this.TestimonialRatingColor = PanelGradiantMiddleColor;
            this.TestimonialStatusColor = PanelGradiantMiddleColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}