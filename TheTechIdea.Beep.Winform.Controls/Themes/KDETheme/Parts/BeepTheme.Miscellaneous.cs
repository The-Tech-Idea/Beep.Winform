using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = PanelBackColor;
            this.DisabledBackColor = PanelBackColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = BorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = PanelBackColor;
            this.CodeBlockBackgroundColor = PanelBackColor;
            this.CodeBlockBorderColor = BorderColor;
            this.RowBackColor = PanelBackColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = PanelBackColor;
            this.SelectedRowBackColor = PanelBackColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = PanelBackColor;
            this.SubLabelBackColor = PanelBackColor;
            this.SubLabelHoverBackColor = PanelBackColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = PanelBackColor;
            this.HoverLinkColor = PanelBackColor;
            this.PrimaryTextColor = PrimaryColor;
            this.SecondaryTextColor = SecondaryColor;
            this.AccentTextColor = AccentColor;
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
            this.ScrollBarHoverThumbColor = PanelBackColor;
            this.ScrollBarHoverTrackColor = PanelBackColor;
            this.ScrollBarActiveThumbColor = PanelBackColor;
            this.ScrollListBackColor = PanelBackColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = BorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = PanelBackColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = PanelBackColor;
            this.ScrollListItemSelectedBorderColor = BorderColor;
            this.ScrollListItemBorderColor = BorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = PanelBackColor;
            this.StarRatingBorderColor = BorderColor;
            this.StarRatingFillColor = PanelBackColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelBackColor;
            this.StarRatingHoverBorderColor = BorderColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = PanelBackColor;
            this.StarRatingSelectedBorderColor = BorderColor;
            this.StarTitleForeColor = ForeColor;
            this.StarTitleBackColor = PanelBackColor;
            this.ActiveTabBackColor = PanelBackColor;
            this.ActiveTabForeColor = ForeColor;
            this.InactiveTabBackColor = PanelBackColor;
            this.InactiveTabForeColor = ForeColor;
            this.TestimonialBackColor = PanelBackColor;
            this.TestimonialTextColor = ForeColor;
            this.TestimonialNameColor = PanelBackColor;
            this.TestimonialDetailsColor = PanelBackColor;
            this.TestimonialDateColor = PanelBackColor;
            this.TestimonialRatingColor = PanelBackColor;
            this.TestimonialStatusColor = PanelBackColor;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}