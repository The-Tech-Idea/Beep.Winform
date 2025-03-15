using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
  
        [Serializable]
        public class BeepTheme
    {
        public BeepTheme()
        {
            ThemeGuid = Guid.NewGuid().ToString();
          
        }
        // UI Elements
        public string ThemeGuid { get; set; }
        public string ThemeName => this.GetType().Name;
        public Color CloseButtonColor { get; set; }
        public Color MaxButtonColor { get; set; }
        public Color MinButtonColor { get; set; }
        public Color TitleBarColor { get; set; }
        public Color TitleBarTextColor { get; set; }
        public Color TitleBarIconColor { get; set; }
        public Color TitleBarHoverColor { get; set; }
        public Color TitleBarHoverTextColor { get; set; }
        public Color TitleBarHoverIconColor { get; set; }
        public Color TitleBarActiveColor { get; set; }
        public Color TitleBarActiveTextColor { get; set; }
        public Color TitleBarActiveIconColor { get; set; }
        public Color TitleBarInactiveColor { get; set; }
        public Color TitleBarInactiveTextColor { get; set; }
        public Color TitleBarInactiveIconColor { get; set; }
        public Color TitleBarBorderColor { get; set; }
        public Color TitleBarBorderHoverColor { get; set; }
        public Color TitleBarBorderActiveColor { get; set; }
        public Color TitleBarBorderInactiveColor { get; set; }
        public Color TitleBarCloseHoverColor { get; set; }
        public Color TitleBarCloseHoverTextColor { get; set; }
        public Color TitleBarCloseHoverIconColor { get; set; }
        public Color TitleBarCloseActiveColor { get; set; }
        public Color TitleBarCloseActiveTextColor { get; set; }
        public Color TitleBarCloseActiveIconColor { get; set; }
        public Color TitleBarCloseInactiveColor { get; set; }
        public Color TitleBarCloseInactiveTextColor { get; set; }
        public Color TitleBarCloseInactiveIconColor { get; set; }
        public Color TitleBarCloseBorderColor { get; set; }
        public Color TitleBarCloseBorderHoverColor { get; set; }
        public Color TitleBarCloseBorderActiveColor { get; set; }
        public Color TitleBarCloseBorderInactiveColor { get; set; }
        public Color TitleBarMaxHoverColor { get; set; }
        public Color TitleBarMaxHoverTextColor { get; set; }
        public Color TitleBarMaxHoverIconColor { get; set; }
        public Color TitleBarMaxActiveColor { get; set; }
        public Color TitleBarMaxActiveTextColor { get; set; }
        public Color TitleBarMaxActiveIconColor { get; set; }
        public Color TitleBarMaxInactiveColor { get; set; }
        public Color TitleBarMaxInactiveTextColor { get; set; }
        public Color TitleBarMaxInactiveIconColor { get; set; }
        public Color TitleBarMaxBorderColor { get; set; }
        public Color TitleBarMaxBorderHoverColor { get; set; }
        public Color TitleBarMaxBorderActiveColor { get; set; }
        public Color TitleBarMaxBorderInactiveColor { get; set; }
        public Color TitleBarMinHoverColor { get; set; }
        public Color TitleBarMinHoverTextColor { get; set; }
        public Color TitleBarMinHoverIconColor { get; set; }
        public Color TitleBarMinActiveColor { get; set; }
        public Color TitleBarMinActiveTextColor { get; set; }
        public Color TitleBarMinActiveIconColor { get; set; }
        public Color TitleBarMinInactiveColor { get; set; }
        public Color TitleBarMinInactiveTextColor { get; set; }
        public Color TitleBarMinInactiveIconColor { get; set; }
        public Color TitleBarMinBorderColor { get; set; }
        public Color TitleBarMinBorderHoverColor { get; set; }
        public Color TitleBarMinBorderActiveColor { get; set; }
        public Color TitleBarMinBorderInactiveColor { get; set; }

        // Missing TitleBarMinimize properties
        public Color TitleBarMinimizeHoverColor { get; set; }
        public Color TitleBarMinimizeHoverTextColor { get; set; }
        public Color TitleBarMinimizeHoverIconColor { get; set; }
        public Color TitleBarMinimizeActiveColor { get; set; }
        public Color TitleBarMinimizeActiveTextColor { get; set; }
        public Color TitleBarMinimizeActiveIconColor { get; set; }
        public Color TitleBarMinimizeInactiveColor { get; set; }
        public Color TitleBarMinimizeInactiveTextColor { get; set; }
        public Color TitleBarMinimizeInactiveIconColor { get; set; }
        public Color TitleBarMinimizeBorderColor { get; set; }
        public Color TitleBarMinimizeBorderHoverColor { get; set; }
        public Color TitleBarMinimizeBorderActiveColor { get; set; }
        public Color TitleBarMinimizeBorderInactiveColor { get; set; }

        // Font and Typography properties
        public string FontName { get; set; }
        public float FontSize { get; set; }

        // Additional color properties for titles
        public Color TitleForColor { get; set; }
        public Color TitleBarForColor { get; set; }

        // Styles
        public TypographyStyle TitleStyle { get; set; }
            public TypographyStyle SubtitleStyle { get; set; }
            public TypographyStyle BodyStyle { get; set; }
            public TypographyStyle CaptionStyle { get; set; }
            public TypographyStyle ButtonStyle { get; set; }
            public TypographyStyle LinkStyle { get; set; }
            public TypographyStyle OverlineStyle { get; set; }

            // Additional UI Element Colors
            public Color DescriptionForColor { get; set; }
            public Color BeforeForColor { get; set; }
            public Color LatestForColor { get; set; }
            public Color BackColor { get; set; }
            public Color ButtonBackColor { get; set; }
            public Color ButtonForeColor { get; set; }
            public Color TextBoxBackColor { get; set; }
            public Color TextBoxForeColor { get; set; }
            public Color LabelBackColor { get; set; }
            public Color LabelForeColor { get; set; }
            public Color PanelBackColor { get; set; }
            public Color HeaderBackColor { get; set; }
            public Color HeaderForeColor { get; set; }
            public Color GridLineColor { get; set; }
            public Color RowBackColor { get; set; }
            public Color RowForeColor { get; set; }
            public Color AltRowBackColor { get; set; }
            public Color SelectedRowBackColor { get; set; }
            public Color SelectedRowForeColor { get; set; }
            public Color ComboBoxBackColor { get; set; }
            public Color ComboBoxForeColor { get; set; }
            public Color CheckBoxBackColor { get; set; }
            public Color CheckBoxForeColor { get; set; }
            public Color RadioButtonBackColor { get; set; }
            public Color RadioButtonForeColor { get; set; }
            public Color BorderColor { get; set; }
            public Color ActiveBorderColor { get; set; }
            public Color InactiveBorderColor { get; set; }
            public Color ButtonHoverBackColor { get; set; }
            public Color ButtonHoverForeColor { get; set; }
            public Color ButtonActiveBackColor { get; set; }
            public Color ButtonActiveForeColor { get; set; }
            public Color LinkColor { get; set; }
            public Color VisitedLinkColor { get; set; }
            public Color HoverLinkColor { get; set; }
            public Color ToolTipBackColor { get; set; }
            public Color ToolTipForeColor { get; set; }
            public Color ScrollBarBackColor { get; set; }
            public Color ScrollBarThumbColor { get; set; }
            public Color ScrollBarTrackColor { get; set; }
            public Color StatusBarBackColor { get; set; }
            public Color StatusBarForeColor { get; set; }
            public Color TabBackColor { get; set; }
            public Color TabForeColor { get; set; }
            public Color ActiveTabBackColor { get; set; }
            public Color ActiveTabForeColor { get; set; }
            public Color DialogBackColor { get; set; }
            public Color DialogForeColor { get; set; }
            public Color DialogButtonBackColor { get; set; }
            public Color DialogButtonForeColor { get; set; }

            // Gradient Properties
            public Color GradientStartColor { get; set; }
            public Color GradientEndColor { get; set; }
            public LinearGradientMode GradientDirection { get; set; }
        // Grid Colors
        public Color GridBackColor { get; set; }
        public Color GridForeColor { get; set; }
        public Color GridHeaderBackColor { get; set; }
        public Color GridHeaderForeColor { get; set; }
        public Color GridHeaderBorderColor { get; set; }
        public Color GridHeaderHoverBackColor { get; set; }
        public Color GridHeaderHoverForeColor { get; set; }
        public Color GridHeaderSelectedBackColor { get; set; }
        public Color GridHeaderSelectedForeColor { get; set; }
        public Color GridHeaderHoverBorderColor { get; set; }
        public Color GridHeaderSelectedBorderColor { get; set; }
        public Color GridRowHoverBackColor { get; set; }
        public Color GridRowHoverForeColor { get; set; }
        public Color GridRowSelectedBackColor { get; set; }
        public Color GridRowSelectedForeColor { get; set; }
        public Color GridRowHoverBorderColor { get; set; }
        public Color GridRowSelectedBorderColor { get; set; }



        // Side Menu Colors
        public Color SideMenuBackColor { get; set; }
            public Color SideMenuHoverBackColor { get; set; }
            public Color SideMenuSelectedBackColor { get; set; }
            public Color SideMenuForeColor { get; set; }
            public Color SideMenuSelectedForeColor { get; set; }
            public Color SideMenuHoverForeColor { get; set; }
            public Color SideMenuBorderColor { get; set; }
            public Color SideMenuIconColor { get; set; }
            public Color SideMenuSelectedIconColor { get; set; }

            // Title Bar and Navigation Colors
            public Color TitleBarBackColor { get; set; }
            public Color TitleBarForeColor { get; set; }
            public Color TitleBarHoverBackColor { get; set; }
            public Color TitleBarHoverForeColor { get; set; }
            public Color DashboardBackColor { get; set; }
            public Color DashboardCardBackColor { get; set; }
            public Color DashboardCardHoverBackColor { get; set; }
            public Color CardTitleForeColor { get; set; }
            public Color CardTextForeColor { get; set; }
            public Color CardBackColor { get;set; }
        

            public TypographyStyle CardHeaderStyle { get; set; }
            public TypographyStyle CardparagraphStyle { get; set; }
            public Color ChartBackColor { get; set; }
            public Color ChartLineColor { get; set; }
            public Color ChartFillColor { get; set; }
            public Color ChartAxisColor { get; set; }
            public Color ChartTitleColor { get; set; }
        public Color ChartTextColor { get; set; }
        public Color ChartLegendBackColor { get; set; }
        public Color ChartLegendTextColor { get; set; }
        public Color ChartLegendShapeColor { get; set; }
        public Color ChartGridLineColor { get; set; }
        public List<Color> ChartDefaultSeriesColors { get; set; }
        public Color SidebarIconColor { get; set; }
            public Color SidebarSelectedIconColor { get; set; }
            public Color SidebarTextColor { get; set; }
            public Color SidebarSelectedTextColor { get; set; }
            public Color NavigationBackColor { get; set; }
            public Color NavigationForeColor { get; set; }
            public Color NavigationHoverBackColor { get; set; }
            public Color NavigationHoverForeColor { get; set; }
            public Color BadgeBackColor { get; set; }
            public Color BadgeForeColor { get; set; }
            public Color HighlightBackColor { get; set; }

            // Typography
            public TypographyStyle Heading1 { get; set; }
            public TypographyStyle Heading2 { get; set; }
            public TypographyStyle Heading3 { get; set; }
            public TypographyStyle Heading4 { get; set; }
            public TypographyStyle Heading5 { get; set; }
            public TypographyStyle Heading6 { get; set; }
            public TypographyStyle Paragraph { get; set; }
            public TypographyStyle Blockquote { get; set; }
            public Color BlockquoteBorderColor { get; set; }
            public float BlockquoteBorderWidth { get; set; }
            public float BlockquotePadding { get; set; }
            public TypographyStyle InlineCode { get; set; }
            public Color InlineCodeBackgroundColor { get; set; }
            public float InlineCodePadding { get; set; }
            public TypographyStyle CodeBlock { get; set; }
            public Color CodeBlockBackgroundColor { get; set; }
            public Color CodeBlockBorderColor { get; set; }
            public float CodeBlockBorderWidth { get; set; }
            public float CodeBlockPadding { get; set; }
            public TypographyStyle UnorderedList { get; set; }
            public TypographyStyle OrderedList { get; set; }
            public float ListItemSpacing { get; set; }
            public float ListIndentation { get; set; }
            public TypographyStyle Link { get; set; }
            public Color LinkHoverColor { get; set; }
            public bool LinkIsUnderline { get; set; }
            public TypographyStyle SmallText { get; set; }
            public TypographyStyle StrongText { get; set; }
            public TypographyStyle EmphasisText { get; set; }
            public TypographyStyle DisplayLarge { get; set; }
            public TypographyStyle DisplayMedium { get; set; }
            public TypographyStyle DisplaySmall { get; set; }
            public TypographyStyle HeadlineLarge { get; set; }
            public TypographyStyle HeadlineMedium { get; set; }
            public TypographyStyle HeadlineSmall { get; set; }
            public TypographyStyle TitleLarge { get; set; }
            public TypographyStyle TitleMedium { get; set; }
            public TypographyStyle TitleSmall { get; set; }
            public TypographyStyle BodyLarge { get; set; }
            public TypographyStyle BodyMedium { get; set; }
            public TypographyStyle BodySmall { get; set; }
            public TypographyStyle LabelLarge { get; set; }
            public TypographyStyle LabelMedium { get; set; }
            public TypographyStyle LabelSmall { get; set; }

            // Font Families and Styles
            public string FontFamily { get; set; } = "Segoe UI";
            public float FontSizeBlockHeader { get; set; } = 24f;
            public float FontSizeBlockText { get; set; } = 14f;
            public float FontSizeQuestion { get; set; } = 16f;
            public float FontSizeAnswer { get; set; } = 14f;
            public float FontSizeCaption { get; set; } = 12f;
            public float FontSizeButton { get; set; } = 14f;
            public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
            public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
            public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;
            public Color PrimaryTextColor { get; set; } = Color.Black;
            public Color SecondaryTextColor { get; set; } = Color.Gray;
            public Color AccentTextColor { get; set; } = Color.Blue;

            // Color Palette
            public Color PrimaryColor { get; set; }
            public Color SecondaryColor { get; set; }
            public Color AccentColor { get; set; }
            public Color BackgroundColor { get; set; }
            public Color SurfaceColor { get; set; }
            public Color ErrorColor { get; set; }
            public Color WarningColor { get; set; }
            public Color SuccessColor { get; set; }
            public Color OnPrimaryColor { get; set; }
            public Color OnBackgroundColor { get; set; }

            // Additional Properties
            public int PaddingSmall { get; set; }
            public int PaddingMedium { get; set; }
            public int PaddingLarge { get; set; }
            public int BorderRadius { get; set; }
            public int BorderSize { get; set; } = 1;
            public string IconSet { get; set; }
            public bool ApplyThemeToIcons { get; set; }
            public Color ShadowColor { get; set; }
            public float ShadowOpacity { get; set; }
            public double AnimationDurationShort { get; set; }
            public double AnimationDurationMedium { get; set; }
            public double AnimationDurationLong { get; set; }
            public string AnimationEasingFunction { get; set; }
            public bool HighContrastMode { get; set; }
            public Color FocusIndicatorColor { get; set; }
            public bool IsDarkTheme { get; set; }

        // Scrollbar  colors
        public Color ScrollbarBackColor { get; set; }
        public Color ScrollbarThumbColor { get; set; }
        public Color ScrollbarTrackColor { get; set; }
        public Color ScrollbarHoverThumbColor { get; set; }
        public Color ScrollbarHoverTrackColor { get; set; }
        public Color ScrollbarActiveThumbColor { get; set; }
        public Color TestimonialBackColor { get; set; } = Color.White;
        public Color TestimonialTextColor { get; set; } = Color.Black;
        public Color TestimonialNameColor { get; set; } = Color.DarkBlue;
        public Color TestimonialDetailsColor { get; set; } = Color.Gray;
        public Color TestimonialDateColor { get; set; } = Color.DarkGray;
        public Color TestimonialRatingColor { get; set; } = Color.Gold;
        public Color TestimonialStatusColor { get; set; } = Color.Green;

        public Font GetBlockHeaderFont()
        {
            return GetFont(FontFamily, FontSizeBlockHeader, FontStyleBold);
        }

        public Font GetBlockTextFont()
        {
            return GetFont(FontFamily, FontSizeBlockText, FontStyleRegular);
        }

        public Font GetQuestionFont()
        {
            return GetFont(FontFamily, FontSizeQuestion, FontStyleBold);
        }

        public Font GetAnswerFont()
        {
            return GetFont(FontFamily, FontSizeAnswer, FontStyleRegular | FontStyleItalic);
        }

        public Font GetCaptionFont()
        {
            return GetFont(FontFamily, FontSizeCaption, FontStyleRegular);
        }

        public Font GetButtonFont()
        {
            return GetFont(FontFamily, FontSizeButton, FontStyleBold);
        }


        private Font GetFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            try
            {
                // Attempt to create and return the specified font
                return new Font(fontName, fontSize, fontStyle);
            }
            catch (ArgumentException)
            {
                // Fallback to default system font if the specified font is not installed
                return new Font(System.Drawing.FontFamily.GenericSansSerif, fontSize, fontStyle);
            }
        }


        // Replace Transparent Colors with Fallbacks
        public void ReplaceTransparentColors(Color fallbackColor)
            {
                foreach (var prop in GetType().GetProperties())
                {
                    if (prop.PropertyType == typeof(Color))
                    {
                        Color color = (Color)prop.GetValue(this);
                        if (color.A == 0)
                        {
                            prop.SetValue(this, fallbackColor);
                        }
                    }
                }
            }

        public override bool Equals(object obj)
        {
            if (obj is BeepTheme other)
            {
                return this.ButtonBackColor == other.ButtonBackColor &&
                       this.ButtonForeColor == other.ButtonForeColor &&
                       this.PanelBackColor == other.PanelBackColor;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ButtonBackColor.GetHashCode() ^ ButtonForeColor.GetHashCode() ^ PanelBackColor.GetHashCode();
        }

        // Implement IEquatable<BeepTheme> for type-safe equality checks
        //public bool Equals(BeepTheme other)
        //{
        //    if (other is BeepTheme otherTheme)
        //    {
        //        return this.ThemeGuid == otherTheme.ThemeGuid; // Use ThemeGuid for unique identification
        //    }
        //    return false;
        //}

    }



}
