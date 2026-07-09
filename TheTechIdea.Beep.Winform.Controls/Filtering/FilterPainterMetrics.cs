using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Metrics and measurements for filter painter layout
    /// Defines spacing, sizing, and layout measurements (NOT colors/borders - those come from BeepStyling)
    /// Similar to how FormPainterMetrics works in BeepiFormPro
    /// </summary>
    public class FilterPainterMetrics
    {
        #region Dimensions & Spacing

        /// <summary>
        /// Height of the filter bar/panel
        /// </summary>
        public int FilterHeight { get; set; } = 45;

        /// <summary>
        /// Width of the filter panel (for sidebar modes)
        /// </summary>
        public int FilterWidth { get; set; } = 300;

        /// <summary>
        /// Padding inside filter container
        /// </summary>
        public int Padding { get; set; } = 10;

        /// <summary>
        /// Spacing between filter items/chips
        /// </summary>
        public int ItemSpacing { get; set; } = 8;

        /// <summary>
        /// Height of individual filter rows
        /// </summary>
        public int RowHeight { get; set; } = 36;

        /// <summary>
        /// Corner radius for rounded elements
        /// </summary>
        public int CornerRadius { get; set; } = 4;

        /// <summary>
        /// Border width
        /// </summary>
        public int BorderWidth { get; set; } = 1;

        /// <summary>
        /// Indentation for nested groups
        /// </summary>
        public int GroupIndentation { get; set; } = 20;

        /// <summary>
        /// Height of logic connectors (AND/OR)
        /// </summary>
        public int ConnectorHeight { get; set; } = 24;

        /// <summary>
        /// Width of remove/close buttons
        /// </summary>
        public int CloseButtonSize { get; set; } = 20;

        /// <summary>
        /// Size of badge showing filter count
        /// </summary>
        public int BadgeSize { get; set; } = 20;

        #endregion

        #region Layout Behavior

        /// <summary>
        /// Whether to show shadow/elevation (BeepStyling handles the actual shadow)
        /// </summary>
        public bool ShowShadow { get; set; } = true;

        /// <summary>
        /// Shadow depth/offset (passed to BeepStyling)
        /// </summary>
        public int ShadowDepth { get; set; } = 2;

        /// <summary>
        /// Whether to enable animations
        /// </summary>
        public bool EnableAnimations { get; set; } = true;

        /// <summary>
        /// Animation duration in milliseconds
        /// </summary>
        public int AnimationDuration { get; set; } = 200;

        /// <summary>
        /// Whether to show drag handles for reordering
        /// </summary>
        public bool ShowDragHandles { get; set; } = false;

        /// <summary>
        /// Whether to show visual tree branches for QueryBuilder style
        /// </summary>
        public bool ShowLogicBranches { get; set; } = true;

        /// <summary>
        /// Whether to show filter count badges
        /// </summary>
        public bool ShowCountBadges { get; set; } = true;

        #endregion

        #region Typography

        /// <summary>
        /// Font for filter labels
        /// </summary>
        public Font LabelFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 9F, FontWeight.Normal, FontStyle.Regular);

        /// <summary>
        /// Font for filter values
        /// </summary>
        public Font ValueFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 9F, FontWeight.Normal, FontStyle.Regular);

        /// <summary>
        /// Font for filter count/badge
        /// </summary>
        public Font BadgeFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 8F, FontWeight.Bold, FontStyle.Bold);

        /// <summary>
        /// Font for logic connectors (AND/OR)
        /// </summary>
        public Font ConnectorFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 8F, FontWeight.Normal, FontStyle.Regular);

        #endregion

        /// <summary>
        /// Creates default metrics for the specified filter style
        /// Note: Visual styling comes from BeepStyling - this only sets layout/spacing
        /// </summary>
        public static FilterPainterMetrics DefaultFor(FilterStyle style, IBeepTheme theme = null)
        {
            var metrics = new FilterPainterMetrics();

            // Style-specific layout adjustments
            switch (style)
            {
                case FilterStyle.TagPills:
                    metrics.CornerRadius = 16; // Pill shape
                    metrics.Padding = 8;
                    metrics.FilterHeight = 42;
                    metrics.ItemSpacing = 6;
                    metrics.ShowShadow = false;
                    metrics.RowHeight = 32;
                    break;

                case FilterStyle.GroupedRows:
                    metrics.Padding = 16;
                    metrics.FilterHeight = 60;
                    metrics.ItemSpacing = 8;
                    metrics.GroupIndentation = 24;
                    metrics.ConnectorHeight = 28;
                    metrics.ShowLogicBranches = true;
                    metrics.ShowDragHandles = true;
                    break;

                case FilterStyle.QueryBuilder:
                    metrics.Padding = 20;
                    metrics.FilterHeight = 80;
                    metrics.ItemSpacing = 12;
                    metrics.GroupIndentation = 32;
                    metrics.ShowLogicBranches = true;
                    metrics.ConnectorHeight = 32;
                    metrics.RowHeight = 40;
                    break;

                case FilterStyle.DropdownMultiSelect:
                    metrics.Padding = 12;
                    metrics.FilterHeight = 48;
                    metrics.ItemSpacing = 10;
                    metrics.ShowCountBadges = true;
                    metrics.BadgeSize = 22;
                    break;

                case FilterStyle.InlineRow:
                    metrics.Padding = 6;
                    metrics.FilterHeight = 36;
                    metrics.ItemSpacing = 4;
                    metrics.RowHeight = 32;
                    metrics.ShowShadow = false;
                    metrics.CornerRadius = 2;
                    break;

                case FilterStyle.SidebarPanel:
                    metrics.Padding = 16;
                    metrics.FilterWidth = 280;
                    metrics.ItemSpacing = 10;
                    metrics.RowHeight = 36;
                    metrics.ShowShadow = true;
                    metrics.ShadowDepth = 4;
                    break;

                case FilterStyle.QuickSearch:
                    metrics.Padding = 10;
                    metrics.FilterHeight = 40;
                    metrics.ShowCountBadges = true;
                    metrics.BadgeSize = 20;
                    metrics.ShowShadow = false;
                    break;

                case FilterStyle.AdvancedDialog:
                    metrics.Padding = 20;
                    metrics.FilterHeight = 70;
                    metrics.ItemSpacing = 12;
                    metrics.ShowShadow = true;
                    metrics.ShadowDepth = 6;
                    metrics.GroupIndentation = 20;
                    break;
            }

            return metrics;
        }

        /// <summary>
        /// Returns a copy of these metrics with all dimension properties scaled for the given control's DPI.
        /// Use this in <c>CalculateLayout</c> so layout and drawing rects are DPI-aware.
        /// </summary>
        public FilterPainterMetrics GetScaledFor(Control owner)
        {
            return new FilterPainterMetrics
            {
                FilterHeight      = DpiScalingHelper.ScaleValue(this.FilterHeight,      owner),
                FilterWidth       = DpiScalingHelper.ScaleValue(this.FilterWidth,       owner),
                Padding           = DpiScalingHelper.ScaleValue(this.Padding,            owner),
                ItemSpacing       = DpiScalingHelper.ScaleValue(this.ItemSpacing,       owner),
                RowHeight         = DpiScalingHelper.ScaleValue(this.RowHeight,          owner),
                CornerRadius      = DpiScalingHelper.ScaleValue(this.CornerRadius,      owner),
                BorderWidth       = DpiScalingHelper.ScaleValue(this.BorderWidth,       owner),
                GroupIndentation  = DpiScalingHelper.ScaleValue(this.GroupIndentation,  owner),
                ConnectorHeight   = DpiScalingHelper.ScaleValue(this.ConnectorHeight,   owner),
                CloseButtonSize   = DpiScalingHelper.ScaleValue(this.CloseButtonSize,   owner),
                BadgeSize         = DpiScalingHelper.ScaleValue(this.BadgeSize,          owner),
                ShadowDepth       = DpiScalingHelper.ScaleValue(this.ShadowDepth,       owner),
                ShowShadow        = this.ShowShadow,
                EnableAnimations  = this.EnableAnimations,
                AnimationDuration = this.AnimationDuration,
                ShowDragHandles   = this.ShowDragHandles,
                ShowLogicBranches = this.ShowLogicBranches,
                ShowCountBadges   = this.ShowCountBadges,
                // Shared cached fonts (no DPI scaling needed — WinForms auto-scales Point fonts)
                LabelFont    = this.LabelFont,
                ValueFont    = this.ValueFont,
                BadgeFont    = this.BadgeFont,
                ConnectorFont = this.ConnectorFont
            };
        }

        /// <summary>
        /// Clones the current metrics
        /// </summary>
        public FilterPainterMetrics Clone()
        {
            return new FilterPainterMetrics
            {
                FilterHeight = this.FilterHeight,
                FilterWidth = this.FilterWidth,
                Padding = this.Padding,
                ItemSpacing = this.ItemSpacing,
                RowHeight = this.RowHeight,
                CornerRadius = this.CornerRadius,
                BorderWidth = this.BorderWidth,
                GroupIndentation = this.GroupIndentation,
                ConnectorHeight = this.ConnectorHeight,
                CloseButtonSize = this.CloseButtonSize,
                BadgeSize = this.BadgeSize,
                ShowShadow = this.ShowShadow,
                ShadowDepth = this.ShadowDepth,
                EnableAnimations = this.EnableAnimations,
                AnimationDuration = this.AnimationDuration,
                ShowDragHandles = this.ShowDragHandles,
                ShowLogicBranches = this.ShowLogicBranches,
                ShowCountBadges = this.ShowCountBadges,
                // Shared cached fonts from BeepThemesManager - share the reference (do NOT clone/dispose)
                LabelFont = this.LabelFont,
                ValueFont = this.ValueFont,
                BadgeFont = this.BadgeFont,
                ConnectorFont = this.ConnectorFont
            };
        }
    }
}
