using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Tooltips;

namespace TheTechIdea.Beep.Winform.Controls
{
    public sealed class RibbonCommandInvokedEventArgs : EventArgs
    {
        public RibbonCommandInvokedEventArgs(SimpleItem command, ToolStripItem source)
        {
            Command = command;
            Source = source;
        }

        public SimpleItem Command { get; }
        public ToolStripItem Source { get; }
    }

    public sealed class BackstageCommandInvokedEventArgs : EventArgs
    {
        public BackstageCommandInvokedEventArgs(SimpleItem section, SimpleItem command)
        {
            Section = section;
            Command = command;
        }

        public SimpleItem Section { get; }
        public SimpleItem Command { get; }
    }

    public sealed class BackstageSectionChangedEventArgs : EventArgs
    {
        public BackstageSectionChangedEventArgs(SimpleItem section, int sectionIndex)
        {
            Section = section;
            SectionIndex = sectionIndex;
        }

        public SimpleItem Section { get; }
        public int SectionIndex { get; }
    }

    public sealed class RibbonMergedEventArgs : EventArgs
    {
        public RibbonMergedEventArgs(RibbonMergeMode mergeMode, int sourceTabCount, int resultTabCount)
        {
            MergeMode = mergeMode;
            SourceTabCount = sourceTabCount;
            ResultTabCount = resultTabCount;
        }

        public RibbonMergeMode MergeMode { get; }
        public int SourceTabCount { get; }
        public int ResultTabCount { get; }
    }

    public sealed class RibbonCustomizationAppliedEventArgs : EventArgs
    {
        public RibbonCustomizationAppliedEventArgs(RibbonCustomizationAction action, int tabCount, int quickAccessCount)
        {
            Action = action;
            TabCount = tabCount;
            QuickAccessCount = quickAccessCount;
        }

        public RibbonCustomizationAction Action { get; }
        public int TabCount { get; }
        public int QuickAccessCount { get; }
    }

    public sealed class RibbonSearchExecutedEventArgs : EventArgs
    {
        public RibbonSearchExecutedEventArgs(string query, RibbonSearchMode mode, int resultCount, bool providerUsed, bool providerFailed, bool usedLocalFallback)
        {
            Query = query;
            Mode = mode;
            ResultCount = resultCount;
            ProviderUsed = providerUsed;
            ProviderFailed = providerFailed;
            UsedLocalFallback = usedLocalFallback;
        }

        public string Query { get; }
        public RibbonSearchMode Mode { get; }
        public int ResultCount { get; }
        public bool ProviderUsed { get; }
        public bool ProviderFailed { get; }
        public bool UsedLocalFallback { get; }
    }

    public sealed class RibbonTooltipActionRequestedEventArgs : EventArgs
    {
        public RibbonTooltipActionRequestedEventArgs(SimpleItem command, RibbonSuperTooltipModel model, string action)
        {
            Command = command;
            Model = model;
            Action = action ?? string.Empty;
        }

        public SimpleItem Command { get; }
        public RibbonSuperTooltipModel Model { get; }
        public string Action { get; }
    }

    internal sealed class ContextualGroup
    {
        public string Name { get; set; } = string.Empty;
        public Color Color { get; set; } = Color.CornflowerBlue;
        public bool Visible { get; set; }
        public List<TabPage> Pages { get; } = [];
    }

    public sealed class RibbonCustomizationState
    {
        public int SchemaVersion { get; set; } = 2;
        public RibbonLayoutMode LayoutMode { get; set; } = RibbonLayoutMode.Classic;
        public RibbonDensity Density { get; set; } = RibbonDensity.Comfortable;
        public RibbonSearchMode SearchMode { get; set; } = RibbonSearchMode.Off;
        public bool SearchIncludeBackstage { get; set; }
        public int SearchMaxResults { get; set; } = 12;
        public bool EnableKeyTips { get; set; } = true;
        public bool QuickAccessAboveRibbon { get; set; } = true;
        public bool IsMinimized { get; set; }
        public bool ShowMinimizedPopupOnTabClick { get; set; } = true;
        public int BackstageSelectedIndex { get; set; } = 0;
        public List<string> QuickAccessCommandKeys { get; set; } = [];
        public List<RibbonTabState> Tabs { get; set; } = [];
    }

    public sealed class RibbonTabState
    {
        public string TabKey { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
        public int Order { get; set; }
        public List<RibbonGroupState> Groups { get; set; } = [];
    }

    public sealed class RibbonGroupState
    {
        public string GroupKey { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
        public int Order { get; set; }
    }

    public sealed class RibbonThemeTokens
    {
        public int Background { get; set; }
        public int TabActiveBack { get; set; }
        public int TabInactiveBack { get; set; }
        public int TabBorder { get; set; }
        public int GroupBack { get; set; }
        public int GroupBorder { get; set; }
        public int HoverBack { get; set; }
        public int PressedBack { get; set; }
        public int FocusBorder { get; set; }
        public int Separator { get; set; }
        public int Text { get; set; }
        public int IconColor { get; set; }
        public int QuickAccessBack { get; set; }
        public int QuickAccessBorder { get; set; }
        public int DisabledBack { get; set; }
        public int DisabledText { get; set; }
        public int DisabledBorder { get; set; }
        public int SelectionBack { get; set; }
        public int ElevationColor { get; set; }
        public int CornerRadius { get; set; }
        public int GroupSpacing { get; set; }
        public int ItemSpacing { get; set; }
        public int ElevationLevel { get; set; }
        public int ElevationStrongLevel { get; set; }
        public float FocusBorderThickness { get; set; }
        public RibbonTypographyToken? TabTypography { get; set; }
        public RibbonTypographyToken? GroupTypography { get; set; }
        public RibbonTypographyToken? CommandTypography { get; set; }
        public RibbonTypographyToken? ContextHeaderTypography { get; set; }

        public static RibbonThemeTokens FromTheme(RibbonTheme theme)
        {
            return new RibbonThemeTokens
            {
                Background = theme.Background.ToArgb(),
                TabActiveBack = theme.TabActiveBack.ToArgb(),
                TabInactiveBack = theme.TabInactiveBack.ToArgb(),
                TabBorder = theme.TabBorder.ToArgb(),
                GroupBack = theme.GroupBack.ToArgb(),
                GroupBorder = theme.GroupBorder.ToArgb(),
                HoverBack = theme.HoverBack.ToArgb(),
                PressedBack = theme.PressedBack.ToArgb(),
                FocusBorder = theme.FocusBorder.ToArgb(),
                Separator = theme.Separator.ToArgb(),
                Text = theme.Text.ToArgb(),
                IconColor = theme.IconColor.ToArgb(),
                QuickAccessBack = theme.QuickAccessBack.ToArgb(),
                QuickAccessBorder = theme.QuickAccessBorder.ToArgb(),
                DisabledBack = theme.DisabledBack.ToArgb(),
                DisabledText = theme.DisabledText.ToArgb(),
                DisabledBorder = theme.DisabledBorder.ToArgb(),
                SelectionBack = theme.SelectionBack.ToArgb(),
                ElevationColor = theme.ElevationColor.ToArgb(),
                CornerRadius = theme.CornerRadius,
                GroupSpacing = theme.GroupSpacing,
                ItemSpacing = theme.ItemSpacing,
                ElevationLevel = theme.ElevationLevel,
                ElevationStrongLevel = theme.ElevationStrongLevel,
                FocusBorderThickness = theme.FocusBorderThickness,
                TabTypography = RibbonTypographyToken.FromStyle(theme.TabTypography),
                GroupTypography = RibbonTypographyToken.FromStyle(theme.GroupTypography),
                CommandTypography = RibbonTypographyToken.FromStyle(theme.CommandTypography),
                ContextHeaderTypography = RibbonTypographyToken.FromStyle(theme.ContextHeaderTypography)
            };
        }

        public RibbonTheme ToTheme(RibbonTheme? target = null)
        {
            var theme = target ?? new RibbonTheme();
            theme.Background = Color.FromArgb(Background);
            theme.TabActiveBack = Color.FromArgb(TabActiveBack);
            theme.TabInactiveBack = Color.FromArgb(TabInactiveBack);
            theme.TabBorder = Color.FromArgb(TabBorder);
            theme.GroupBack = Color.FromArgb(GroupBack);
            theme.GroupBorder = Color.FromArgb(GroupBorder);
            theme.HoverBack = Color.FromArgb(HoverBack);
            theme.PressedBack = Color.FromArgb(PressedBack);
            theme.FocusBorder = Color.FromArgb(FocusBorder);
            theme.Separator = Color.FromArgb(Separator);
            theme.Text = Color.FromArgb(Text);
            theme.IconColor = Color.FromArgb(IconColor);
            theme.QuickAccessBack = Color.FromArgb(QuickAccessBack);
            theme.QuickAccessBorder = Color.FromArgb(QuickAccessBorder);
            theme.DisabledBack = Color.FromArgb(DisabledBack);
            theme.DisabledText = Color.FromArgb(DisabledText);
            theme.DisabledBorder = Color.FromArgb(DisabledBorder);
            theme.SelectionBack = Color.FromArgb(SelectionBack);
            theme.ElevationColor = Color.FromArgb(ElevationColor);
            theme.CornerRadius = CornerRadius;
            theme.GroupSpacing = GroupSpacing;
            theme.ItemSpacing = ItemSpacing;
            theme.ElevationLevel = ElevationLevel;
            theme.ElevationStrongLevel = ElevationStrongLevel;
            theme.FocusBorderThickness = FocusBorderThickness <= 0 ? theme.FocusBorderThickness : FocusBorderThickness;
            theme.TabTypography = TabTypography?.ToStyle(theme.TabTypography) ?? theme.TabTypography;
            theme.GroupTypography = GroupTypography?.ToStyle(theme.GroupTypography) ?? theme.GroupTypography;
            theme.CommandTypography = CommandTypography?.ToStyle(theme.CommandTypography) ?? theme.CommandTypography;
            theme.ContextHeaderTypography = ContextHeaderTypography?.ToStyle(theme.ContextHeaderTypography) ?? theme.ContextHeaderTypography;
            return theme;
        }
    }

    public sealed class RibbonTypographyToken
    {
        public string FontFamily { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 9f;
        public int FontWeight { get; set; } = (int)TheTechIdea.Beep.Vis.Modules.FontWeight.Normal;
        public int FontStyle { get; set; } = (int)System.Drawing.FontStyle.Regular;
        public bool IsUnderlined { get; set; }
        public bool IsStrikeout { get; set; }

        public static RibbonTypographyToken FromStyle(TypographyStyle style)
        {
            return new RibbonTypographyToken
            {
                FontFamily = style.FontFamily,
                FontSize = style.FontSize,
                FontWeight = (int)style.FontWeight,
                FontStyle = (int)style.FontStyle,
                IsUnderlined = style.IsUnderlined,
                IsStrikeout = style.IsStrikeout
            };
        }

        public TypographyStyle ToStyle(TypographyStyle fallback)
        {
            var style = new TypographyStyle
            {
                FontFamily = string.IsNullOrWhiteSpace(FontFamily) ? fallback.FontFamily : FontFamily,
                FontSize = FontSize <= 0 ? fallback.FontSize : FontSize,
                FontWeight = Enum.IsDefined(typeof(TheTechIdea.Beep.Vis.Modules.FontWeight), FontWeight)
                    ? (TheTechIdea.Beep.Vis.Modules.FontWeight)FontWeight
                    : fallback.FontWeight,
                FontStyle = Enum.IsDefined(typeof(System.Drawing.FontStyle), FontStyle)
                    ? (System.Drawing.FontStyle)FontStyle
                    : fallback.FontStyle,
                IsUnderlined = IsUnderlined,
                IsStrikeout = IsStrikeout
            };

            return style;
        }
    }
}