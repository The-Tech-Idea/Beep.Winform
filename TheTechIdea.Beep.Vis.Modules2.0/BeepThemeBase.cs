using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    [Serializable]
    public class BeepThemeBase
    {
        // Nested Classes for Organized Properties
        public TitleBarTheme TitleBar { get; set; } = new TitleBarTheme();
        public ButtonTheme Buttons { get; set; } = new ButtonTheme();
        public TypographyTheme Typography { get; set; } = new TypographyTheme();
        public TextBoxTheme TextBoxes { get; set; } = new TextBoxTheme();
        public LabelTheme Labels { get; set; } = new LabelTheme();
        public PanelTheme Panels { get; set; } = new PanelTheme();
        public GridTheme Grid { get; set; } = new GridTheme();
        public ComboBoxTheme ComboBoxes { get; set; } = new ComboBoxTheme();
        public CheckBoxTheme CheckBoxes { get; set; } = new CheckBoxTheme();
        public RadioButtonTheme RadioButtons { get; set; } = new RadioButtonTheme();
        public ScrollBarTheme ScrollBars { get; set; } = new ScrollBarTheme();
        public StatusBarTheme StatusBar { get; set; } = new StatusBarTheme();
        public TabTheme Tabs { get; set; } = new TabTheme();
        public DialogTheme Dialogs { get; set; } = new DialogTheme();
        public SideMenuTheme SideMenu { get; set; } = new SideMenuTheme();
        public DashboardTheme Dashboard { get; set; } = new DashboardTheme();
        public ChartTheme Charts { get; set; } = new ChartTheme();
        public NavigationTheme Navigation { get; set; } = new NavigationTheme();
        public TypographyStyles TypographyStyles { get; set; } = new TypographyStyles();
        public ColorPalette Palette { get; set; } = new ColorPalette();
        public LayoutSettings Layout { get; set; } = new LayoutSettings();
        public AnimationSettings Animations { get; set; } = new AnimationSettings();
        public AccessibilitySettings Accessibility { get; set; } = new AccessibilitySettings();

        // Existing methods like GetFont, ReplaceTransparentColors, etc.
        // ... (Include your existing methods here)

        // Example of existing methods (simplified for brevity)
        public Font GetBlockHeaderFont()
        {
            return GetFont(Typography.FontFamily, Typography.FontSizeBlockHeader, Typography.FontStyleBold);
        }

        private Font GetFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            try
            {
                return new Font(fontName, fontSize, fontStyle);
            }
            catch (ArgumentException)
            {
                return new Font(System.Drawing.FontFamily.GenericSansSerif, fontSize, fontStyle);
            }
        }
    }

    // Nested Classes Definitions

    [Serializable]
    public class TitleBarTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color IconColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color HoverForeColor { get; set; }
        public Color HoverIconColor { get; set; }
        public Color ActiveBackColor { get; set; }
        public Color ActiveForeColor { get; set; }
        public Color ActiveIconColor { get; set; }
        public Color InactiveBackColor { get; set; }
        public Color InactiveForeColor { get; set; }
        public Color InactiveIconColor { get; set; }
        public Color BorderColor { get; set; }
        public Color CloseButtonColor { get; set; }
        public Color MaxButtonColor { get; set; }
        public Color MinButtonColor { get; set; }
        // Add other TitleBar related properties
    }

    [Serializable]
    public class ButtonTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color BorderColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color HoverForeColor { get; set; }
        public Color ActiveBackColor { get; set; }
        public Color ActiveForeColor { get; set; }
        public Color DisabledBackColor { get; set; }
        public Color DisabledForeColor { get; set; }
        public int BorderSize { get; set; } = 1;
        public int BorderRadius { get; set; }
        // Add other Button related properties
    }

    [Serializable]
    public class TypographyTheme
    {
        public string FontFamily { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 14f;
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
        // Add other typography properties
    }

    [Serializable]
    public class TextBoxTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color BorderColor { get; set; }
        public Color FocusBorderColor { get; set; }
        // Add other TextBox related properties
    }

    [Serializable]
    public class LabelTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        // Add other Label related properties
    }

    [Serializable]
    public class PanelTheme
    {
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        // Add other Panel related properties
    }

    [Serializable]
    public class GridTheme
    {
        public Color GridLineColor { get; set; }
        public Color RowBackColor { get; set; }
        public Color RowForeColor { get; set; }
        public Color AltRowBackColor { get; set; }
        public Color SelectedRowBackColor { get; set; }
        public Color SelectedRowForeColor { get; set; }
        // Add other Grid related properties
    }

    [Serializable]
    public class ComboBoxTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color BorderColor { get; set; }
        public Color ArrowColor { get; set; }
        // Add other ComboBox related properties
    }

    [Serializable]
    public class CheckBoxTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color CheckmarkColor { get; set; }
        public Color BorderColor { get; set; }
        // Add other CheckBox related properties
    }

    [Serializable]
    public class RadioButtonTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color DotColor { get; set; }
        public Color BorderColor { get; set; }
        // Add other RadioButton related properties
    }

    [Serializable]
    public class ScrollBarTheme
    {
        public Color BackColor { get; set; }
        public Color ThumbColor { get; set; }
        public Color TrackColor { get; set; }
        // Add other ScrollBar related properties
    }

    [Serializable]
    public class StatusBarTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        // Add other StatusBar related properties
    }

    [Serializable]
    public class TabTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color ActiveBackColor { get; set; }
        public Color ActiveForeColor { get; set; }
        public Color BorderColor { get; set; }
        // Add other Tab related properties
    }

    [Serializable]
    public class DialogTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color ButtonBackColor { get; set; }
        public Color ButtonForeColor { get; set; }
        // Add other Dialog related properties
    }

    [Serializable]
    public class SideMenuTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color HoverForeColor { get; set; }
        public Color SelectedBackColor { get; set; }
        public Color SelectedForeColor { get; set; }
        public Color BorderColor { get; set; }
        public Color IconColor { get; set; }
        public Color SelectedIconColor { get; set; }
        // Add other SideMenu related properties
    }

    [Serializable]
    public class DashboardTheme
    {
        public Color BackColor { get; set; }
        public Color CardBackColor { get; set; }
        public Color CardHoverBackColor { get; set; }
        public Color CardTitleForeColor { get; set; }
        public Color CardTextForeColor { get; set; }
        public TypographyStyle CardHeaderStyle { get; set; }
        public TypographyStyle CardParagraphStyle { get; set; }
        // Add other Dashboard related properties
    }

    [Serializable]
    public class ChartTheme
    {
        public Color BackColor { get; set; }
        public Color LineColor { get; set; }
        public Color FillColor { get; set; }
        public Color AxisColor { get; set; }
        // Add other Chart related properties
    }

    [Serializable]
    public class NavigationTheme
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color HoverForeColor { get; set; }
        public Color SidebarIconColor { get; set; }
        public Color SidebarSelectedIconColor { get; set; }
        public Color SidebarTextColor { get; set; }
        public Color SidebarSelectedTextColor { get; set; }
        // Add other Navigation related properties
    }

    [Serializable]
    public class TypographyStyles
    {
        public TypographyStyle Heading1 { get; set; }
        public TypographyStyle Heading2 { get; set; }
        public TypographyStyle Heading3 { get; set; }
        public TypographyStyle Heading4 { get; set; }
        public TypographyStyle Heading5 { get; set; }
        public TypographyStyle Heading6 { get; set; }
        public TypographyStyle Subtitle { get; set; }
        public TypographyStyle Body { get; set; }
        public TypographyStyle Caption { get; set; }
        public TypographyStyle Button { get; set; }
        public TypographyStyle Link { get; set; }
        public TypographyStyle Overline { get; set; }
        // Add other typography styles
    }

    [Serializable]
    public class ColorPalette
    {
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
        // Add other palette colors if needed
    }

    [Serializable]
    public class LayoutSettings
    {
        public int PaddingSmall { get; set; }
        public int PaddingMedium { get; set; }
        public int PaddingLarge { get; set; }
        public int BorderRadius { get; set; }
        public int BorderSize { get; set; } = 1;
        public string IconSet { get; set; }
        public bool ApplyThemeToIcons { get; set; }
        // Add other layout related settings
    }

    [Serializable]
    public class AnimationSettings
    {
        public double AnimationDurationShort { get; set; }
        public double AnimationDurationMedium { get; set; }
        public double AnimationDurationLong { get; set; }
        public string AnimationEasingFunction { get; set; }
        // Add other animation related settings
    }

    [Serializable]
    public class AccessibilitySettings
    {
        public bool HighContrastMode { get; set; }
        public Color FocusIndicatorColor { get; set; }
        public bool IsDarkTheme { get; set; }
        public Color ShadowColor { get; set; }
        public float ShadowOpacity { get; set; }
        // Add other accessibility related settings
    }


}
