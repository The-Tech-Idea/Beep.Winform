using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = BackgroundColor;
            this.DisabledBackColor = BackgroundColor;
            this.DisabledForeColor = InactiveBorderColor;
            this.DisabledBorderColor = SecondaryColor;
            this.BlockquoteBorderColor = SecondaryColor;
            this.InlineCodeBackgroundColor = BackgroundColor;
            this.CodeBlockBackgroundColor = BackgroundColor;
            this.CodeBlockBorderColor = SecondaryColor;
            this.RowBackColor = BackgroundColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = PanelBackColor;
            this.SelectedRowBackColor = PanelBackColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = InactiveBorderColor;
            this.SubLabelBackColor = BackgroundColor;
            this.SubLabelHoverBackColor = PanelBackColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = PrimaryColor;
            this.HoverLinkColor = SecondaryColor;
            this.PrimaryTextColor = ForeColor;
            this.SecondaryTextColor = InactiveBorderColor;
            this.AccentTextColor = AccentColor;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.ApplyThemeToIcons = false;
            this.ShadowColor = BorderColor;
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.ScrollBarBackColor = PanelBackColor;
            this.ScrollBarThumbColor = PanelGradiantMiddleColor;
            this.ScrollBarTrackColor = PanelBackColor;
            this.ScrollBarHoverThumbColor = PanelGradiantMiddleColor;
            this.ScrollBarHoverTrackColor = PanelBackColor;
            this.ScrollBarActiveThumbColor = PanelGradiantStartColor;
            this.ScrollListBackColor = PanelBackColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = SecondaryColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = PanelBackColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = PanelBackColor;
            this.ScrollListItemSelectedBorderColor = SecondaryColor;
            this.ScrollListItemBorderColor = SecondaryColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = BackgroundColor;
            this.StarRatingBorderColor = SecondaryColor;
            this.StarRatingFillColor = BackgroundColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelBackColor;
            this.StarRatingHoverBorderColor = SecondaryColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = PanelBackColor;
            this.StarRatingSelectedBorderColor = SecondaryColor;
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = BackgroundColor;
            this.ActiveTabBackColor = PanelBackColor;
            this.ActiveTabForeColor = ForeColor;
            this.InactiveTabBackColor = PanelBackColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = BackgroundColor;
            this.TestimonialTextColor = ForeColor;
            this.TestimonialNameColor = ForeColor;
            this.TestimonialDetailsColor = InactiveBorderColor;
            this.TestimonialDateColor = InactiveBorderColor;
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