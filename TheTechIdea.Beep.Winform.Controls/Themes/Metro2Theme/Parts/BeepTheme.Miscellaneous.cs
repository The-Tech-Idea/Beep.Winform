using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = PanelBackColor;
            this.DisabledBackColor = PanelBackColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = InactiveBorderColor;
            this.BlockquoteBorderColor = InactiveBorderColor;
            this.InlineCodeBackgroundColor = PanelBackColor;
            this.CodeBlockBackgroundColor = PanelBackColor;
            this.CodeBlockBorderColor = InactiveBorderColor;
            this.RowBackColor = PanelBackColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = PanelBackColor;
            this.SelectedRowBackColor = PanelBackColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = PanelBackColor;
            this.SubLabelBackColor = PanelBackColor;
            this.SubLabelHoverBackColor = PanelBackColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = PrimaryColor;
            this.HoverLinkColor = PrimaryColor;
            this.PrimaryTextColor = PrimaryColor;
            this.SecondaryTextColor = AccentColor;
            this.AccentTextColor = SuccessColor;
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
            this.ScrollBarBackColor = PanelBackColor;
            this.ScrollBarThumbColor = PanelBackColor;
            this.ScrollBarTrackColor = PanelBackColor;
            this.ScrollBarHoverThumbColor = PanelBackColor;
            this.ScrollBarHoverTrackColor = PanelBackColor;
            this.ScrollBarActiveThumbColor = PanelBackColor;
            this.ScrollListBackColor = PanelBackColor;
            this.ScrollListForeColor = ForeColor;
            this.ScrollListBorderColor = InactiveBorderColor;
            this.ScrollListItemForeColor = ForeColor;
            this.ScrollListItemHoverForeColor = ForeColor;
            this.ScrollListItemHoverBackColor = PanelBackColor;
            this.ScrollListItemSelectedForeColor = ForeColor;
            this.ScrollListItemSelectedBackColor = PanelBackColor;
            this.ScrollListItemSelectedBorderColor = InactiveBorderColor;
            this.ScrollListItemBorderColor = InactiveBorderColor;
            this.StarRatingForeColor = ForeColor;
            this.StarRatingBackColor = PanelBackColor;
            this.StarRatingBorderColor = InactiveBorderColor;
            this.StarRatingFillColor = PanelBackColor;
            this.StarRatingHoverForeColor = ForeColor;
            this.StarRatingHoverBackColor = PanelBackColor;
            this.StarRatingHoverBorderColor = InactiveBorderColor;
            this.StarRatingSelectedForeColor = ForeColor;
            this.StarRatingSelectedBackColor = PanelBackColor;
            this.StarRatingSelectedBorderColor = InactiveBorderColor;
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