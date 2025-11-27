using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyMiscellaneous()
        {
            this.HighlightBackColor = SecondaryColor;
            this.DisabledBackColor = SurfaceColor;
            this.DisabledForeColor = ForeColor;
            this.DisabledBorderColor = BorderColor;
            this.BlockquoteBorderColor = BorderColor;
            this.InlineCodeBackgroundColor = SurfaceColor;
            this.CodeBlockBackgroundColor = SurfaceColor;
            this.CodeBlockBorderColor = BorderColor;
            this.RowBackColor = SurfaceColor;
            this.RowForeColor = ForeColor;
            this.AltRowBackColor = SecondaryColor;
            this.SelectedRowBackColor = AccentColor;
            this.SelectedRowForeColor = ForeColor;
            this.SubLabelForColor = ForeColor;
            this.SubLabelBackColor = SurfaceColor;
            this.SubLabelHoverBackColor = SecondaryColor;
            this.SubLabelHoverForeColor = ForeColor;
            this.VisitedLinkColor = AccentColor;
            this.HoverLinkColor = AccentColor;
            this.PrimaryTextColor = ForeColor;
            this.SecondaryTextColor = SecondaryColor;
            this.AccentTextColor = AccentColor;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.ApplyThemeToIcons = false;
            this.ShadowColor = Color.FromArgb(255,251,235);
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.ScrollBarBackColor = Color.FromArgb(255,251,235);
            this.ScrollBarThumbColor = Color.FromArgb(255,251,235);
            this.ScrollBarTrackColor = Color.FromArgb(255,251,235);
            this.ScrollBarHoverThumbColor = Color.FromArgb(255,251,235);
            this.ScrollBarHoverTrackColor = Color.FromArgb(255,251,235);
            this.ScrollBarActiveThumbColor = Color.FromArgb(255,251,235);
            this.ScrollListBackColor = Color.FromArgb(255,251,235);
            this.ScrollListForeColor = Color.FromArgb(33,37,41);
            this.ScrollListBorderColor = Color.FromArgb(247,208,136);
            this.ScrollListItemForeColor = Color.FromArgb(33,37,41);
            this.ScrollListItemHoverForeColor = Color.FromArgb(33,37,41);
            this.ScrollListItemHoverBackColor = Color.FromArgb(255,251,235);
            this.ScrollListItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.ScrollListItemSelectedBackColor = Color.FromArgb(255,251,235);
            this.ScrollListItemSelectedBorderColor = Color.FromArgb(247,208,136);
            this.ScrollListItemBorderColor = Color.FromArgb(247,208,136);
            this.StarRatingForeColor = Color.FromArgb(33,37,41);
            this.StarRatingBackColor = Color.FromArgb(255,251,235);
            this.StarRatingBorderColor = Color.FromArgb(247,208,136);
            this.StarRatingFillColor = Color.FromArgb(255,251,235);
            this.StarRatingHoverForeColor = Color.FromArgb(33,37,41);
            this.StarRatingHoverBackColor = Color.FromArgb(255,251,235);
            this.StarRatingHoverBorderColor = Color.FromArgb(247,208,136);
            this.StarRatingSelectedForeColor = Color.FromArgb(33,37,41);
            this.StarRatingSelectedBackColor = Color.FromArgb(255,251,235);
            this.StarRatingSelectedBorderColor = Color.FromArgb(247,208,136);
            this.StarTitleForeColor = Color.FromArgb(33,37,41);
            this.StarTitleBackColor = Color.FromArgb(255,251,235);
            this.ActiveTabBackColor = Color.FromArgb(255,251,235);
            this.ActiveTabForeColor = Color.FromArgb(33,37,41);
            this.InactiveTabBackColor = Color.FromArgb(255,251,235);
            this.InactiveTabForeColor = Color.FromArgb(33,37,41);
            this.TestimonialBackColor = Color.FromArgb(255,251,235);
            this.TestimonialTextColor = Color.FromArgb(33,37,41);
            this.TestimonialNameColor = Color.FromArgb(255,251,235);
            this.TestimonialDetailsColor = Color.FromArgb(255,251,235);
            this.TestimonialDateColor = Color.FromArgb(255,251,235);
            this.TestimonialRatingColor = Color.FromArgb(255,251,235);
            this.TestimonialStatusColor = Color.FromArgb(255,251,235);
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
        }
    }
}