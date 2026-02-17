using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    /// <summary>
    /// Radix UI theme:
    /// - White background with high contrast
    /// - Accessible, WCAG AA/AAA compliant colors
    /// - Clear focus states and prominent focus rings
    /// - Sans-serif typography (Inter/Segoe UI)
    /// - Medium borders & accessible design (pairs with RadixUIFormPainter)
    /// </summary>
    public class RadixUITheme : IBeepTheme
    {
        // Identity
        public string ThemeName { get; } = "RadixUITheme";
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();

        // AppBar / Caption
        public Color AppBarBackColor { get; set; }
        public Color AppBarForeColor { get; set; }
        public Color AppBarButtonForeColor { get; set; }
        public Color AppBarButtonBackColor { get; set; }
        public Color AppBarTextBoxBackColor { get; set; }
        public Color AppBarTextBoxForeColor { get; set; }
        public Color AppBarLabelForeColor { get; set; }
        public Color AppBarLabelBackColor { get; set; }
        public Color AppBarTitleForeColor { get; set; }
        public Color AppBarTitleBackColor { get; set; }
        public Color AppBarSubTitleForeColor { get; set; }
        public Color AppBarSubTitleBackColor { get; set; }
        public Color AppBarCloseButtonColor { get; set; }
        public Color AppBarMaxButtonColor { get; set; }
        public Color AppBarMinButtonColor { get; set; }
        public TypographyStyle AppBarTitleStyle { get; set; }
        public TypographyStyle AppBarSubTitleStyle { get; set; }
        public TypographyStyle AppBarTextStyle { get; set; }
        public Color AppBarGradiantStartColor { get; set; }
        public Color AppBarGradiantEndColor { get; set; }
        public Color AppBarGradiantMiddleColor { get; set; }
        public LinearGradientMode AppBarGradiantDirection { get; set; }

        // Badge
        public Color BadgeBackColor { get; set; }
        public Color BadgeForeColor { get; set; }
        public Color HighlightBackColor { get; set; }
        public TypographyStyle BadgeFont { get; set; }

        // Core
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color PanelBackColor { get; set; }
        public Color PanelGradiantStartColor { get; set; }
        public Color PanelGradiantEndColor { get; set; }
        public Color PanelGradiantMiddleColor { get; set; }
        public LinearGradientMode PanelGradiantDirection { get; set; }
        public Color DisabledBackColor { get; set; }
        public Color DisabledForeColor { get; set; }
        public Color DisabledBorderColor { get; set; }
        public Color BorderColor { get; set; }
        public Color ActiveBorderColor { get; set; }
        public Color InactiveBorderColor { get; set; }

        // Palette
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

        // Markdown / Code
        public Color BlockquoteBorderColor { get; set; }
        public Color InlineCodeBackgroundColor { get; set; }
        public Color CodeBlockBackgroundColor { get; set; }
        public Color CodeBlockBorderColor { get; set; }

        // Buttons (incl. legacy tokens)
        public TypographyStyle ButtonFont { get; set; }
        public TypographyStyle ButtonHoverFont { get; set; }
        public TypographyStyle ButtonSelectedFont { get; set; }
        public Color ButtonHoverBackColor { get; set; }
        public Color ButtonHoverForeColor { get; set; }
        public Color ButtonHoverBorderColor { get; set; }
        public Color ButtonSelectedBorderColor { get; set; }
        public Color ButtonSelectedBackColor { get; set; }
        public Color ButtonSelectedForeColor { get; set; }
        public Color ButtonSelectedHoverBackColor { get; set; }
        public Color ButtonSelectedHoverForeColor { get; set; }
        public Color ButtonSelectedHoverBorderColor { get; set; }
        public Color ButtonBackColor { get; set; }
        public Color ButtonForeColor { get; set; }
        public Color ButtonBorderColor { get; set; }
        public Color ButtonErrorBackColor { get; set; }
        public Color ButtonErrorForeColor { get; set; }
        public Color ButtonErrorBorderColor { get; set; }
        public Color ButtonPressedBackColor { get; set; }
        public Color ButtonPressedForeColor { get; set; }
        public Color ButtonPressedBorderColor { get; set; }

        // Cards
        public TypographyStyle CardTitleFont { get; set; }
        public Color CardTextForeColor { get; set; }
        public Color CardBackColor { get; set; }
        public Color CardTitleForeColor { get; set; }
        public TypographyStyle CardSubTitleFont { get; set; }
        public Color CardSubTitleForeColor { get; set; }
        public TypographyStyle CardHeaderStyle { get; set; }
        public TypographyStyle CardparagraphStyle { get; set; }
        public TypographyStyle CardSubTitleStyle { get; set; }
        public Color CardrGradiantStartColor { get; set; }
        public Color CardGradiantEndColor { get; set; }
        public Color CardGradiantMiddleColor { get; set; }
        public LinearGradientMode CardGradiantDirection { get; set; }

        // Calendar
        public TypographyStyle CalendarTitleFont { get; set; }
        public Color CalendarTitleForColor { get; set; }
        public TypographyStyle DaysHeaderFont { get; set; }
        public Color CalendarDaysHeaderForColor { get; set; }
        public TypographyStyle SelectedDateFont { get; set; }
        public Color CalendarSelectedDateBackColor { get; set; }
        public Color CalendarSelectedDateForColor { get; set; }
        public TypographyStyle CalendarSelectedFont { get; set; }
        public TypographyStyle CalendarUnSelectedFont { get; set; }
        public Color CalendarBackColor { get; set; }
        public Color CalendarForeColor { get; set; }
        public Color CalendarTodayForeColor { get; set; }
        public Color CalendarBorderColor { get; set; }
        public Color CalendarHoverBackColor { get; set; }
        public Color CalendarHoverForeColor { get; set; }
        public TypographyStyle HeaderFont { get; set; }
        public TypographyStyle MonthFont { get; set; }
        public TypographyStyle YearFont { get; set; }
        public TypographyStyle DaysFont { get; set; }
        public TypographyStyle DaysSelectedFont { get; set; }
        public TypographyStyle DateFont { get; set; }
        public Color CalendarFooterColor { get; set; }
        public TypographyStyle FooterFont { get; set; }

        // Charts
        public TypographyStyle ChartTitleFont { get; set; }
        public TypographyStyle ChartSubTitleFont { get; set; }
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

        // CheckBox
        public Color CheckBoxBackColor { get; set; }
        public Color CheckBoxForeColor { get; set; }
        public Color CheckBoxBorderColor { get; set; }
        public Color CheckBoxCheckedBackColor { get; set; }
        public Color CheckBoxCheckedForeColor { get; set; }
        public Color CheckBoxCheckedBorderColor { get; set; }
        public Color CheckBoxHoverBackColor { get; set; }
        public Color CheckBoxHoverForeColor { get; set; }
        public Color CheckBoxHoverBorderColor { get; set; }
        public TypographyStyle CheckBoxFont { get; set; }
        public TypographyStyle CheckBoxCheckedFont { get; set; }

        // ComboBox
        public Color ComboBoxBackColor { get; set; }
        public Color ComboBoxForeColor { get; set; }
        public Color ComboBoxBorderColor { get; set; }
        public Color ComboBoxHoverBackColor { get; set; }
        public Color ComboBoxHoverForeColor { get; set; }
        public Color ComboBoxHoverBorderColor { get; set; }
        public Color ComboBoxSelectedBackColor { get; set; }
        public Color ComboBoxSelectedForeColor { get; set; }
        public Color ComboBoxSelectedBorderColor { get; set; }
        public Color ComboBoxErrorBackColor { get; set; }
        public Color ComboBoxErrorForeColor { get; set; }
        public TypographyStyle ComboBoxItemFont { get; set; }
        public TypographyStyle ComboBoxListFont { get; set; }

        // Company Popover
        public Color CompanyPopoverBackgroundColor { get; set; }
        public Color CompanyTitleColor { get; set; }
        public TypographyStyle CompanyTitleFont { get; set; }
        public Color CompanySubtitleColor { get; set; }
        public TypographyStyle CompanySubTitleFont { get; set; }
        public Color CompanyDescriptionColor { get; set; }
        public TypographyStyle CompanyDescriptionFont { get; set; }
        public Color CompanyLinkColor { get; set; }
        public TypographyStyle CompanyLinkFont { get; set; }
        public Color CompanyButtonBackgroundColor { get; set; }
        public Color CompanyButtonTextColor { get; set; }
        public TypographyStyle CompanyButtonFont { get; set; }
        public Color CompanyDropdownBackgroundColor { get; set; }
        public Color CompanyDropdownTextColor { get; set; }
        public Color CompanyLogoBackgroundColor { get; set; }

        // Dashboard
        public TypographyStyle DashboardTitleFont { get; set; }
        public TypographyStyle DashboardSubTitleFont { get; set; }
        public Color DashboardBackColor { get; set; }
        public Color DashboardCardBackColor { get; set; }
        public Color DashboardCardHoverBackColor { get; set; }
        public Color DashboardTitleForeColor { get; set; }
        public Color DashboardTitleBackColor { get; set; }
        public TypographyStyle DashboardTitleStyle { get; set; }
        public Color DashboardSubTitleForeColor { get; set; }
        public Color DashboardSubTitleBackColor { get; set; }
        public TypographyStyle DashboardSubTitleStyle { get; set; }
        public Color DashboardGradiantStartColor { get; set; }
        public Color DashboardGradiantEndColor { get; set; }
        public Color DashboardGradiantMiddleColor { get; set; }
        public LinearGradientMode DashboardGradiantDirection { get; set; }

        // Dialogs
        public Color DialogBackColor { get; set; }
        public Color DialogForeColor { get; set; }
        public TypographyStyle DialogYesButtonFont { get; set; }
        public TypographyStyle DialogNoButtonFont { get; set; }
        public TypographyStyle DialogOkButtonFont { get; set; }
        public TypographyStyle DialogCancelButtonFont { get; set; }
        public TypographyStyle DialogWarningButtonFont { get; set; }
        public TypographyStyle DialogErrorButtonFont { get; set; }
        public TypographyStyle DialogInformationButtonFont { get; set; }
        public TypographyStyle DialogQuestionButtonFont { get; set; }
        public TypographyStyle DialogHelpButtonFont { get; set; }
        public TypographyStyle DialogCloseButtonFont { get; set; }
        public TypographyStyle DialogYesButtonHoverFont { get; set; }
        public TypographyStyle DialogNoButtonHoverFont { get; set; }
        public TypographyStyle DialogOkButtonHoverFont { get; set; }
        public Color DialogYesButtonBackColor { get; set; }
        public Color DialogYesButtonForeColor { get; set; }
        public Color DialogYesButtonHoverBackColor { get; set; }
        public Color DialogYesButtonHoverForeColor { get; set; }
        public Color DialogYesButtonHoverBorderColor { get; set; }
        public Color DialogCancelButtonBackColor { get; set; }
        public Color DialogCancelButtonForeColor { get; set; }
        public Color DialogCancelButtonHoverBackColor { get; set; }
        public Color DialogCancelButtonHoverForeColor { get; set; }
        public Color DialogCancelButtonHoverBorderColor { get; set; }
        public Color DialogCloseButtonBackColor { get; set; }
        public Color DialogCloseButtonForeColor { get; set; }
        public Color DialogCloseButtonHoverBackColor { get; set; }
        public Color DialogCloseButtonHoverForeColor { get; set; }
        public Color DialogCloseButtonHoverBorderColor { get; set; }
        public Color DialogHelpButtonBackColor { get; set; }
        public Color DialogNoButtonBackColor { get; set; }
        public Color DialogNoButtonForeColor { get; set; }
        public Color DialogNoButtonHoverBackColor { get; set; }
        public Color DialogNoButtonHoverForeColor { get; set; }
        public Color DialogNoButtonHoverBorderColor { get; set; }
        public Color DialogOkButtonBackColor { get; set; }
        public Color DialogOkButtonForeColor { get; set; }
        public Color DialogOkButtonHoverBackColor { get; set; }
        public Color DialogOkButtonHoverForeColor { get; set; }
        public Color DialogOkButtonHoverBorderColor { get; set; }
        public Color DialogWarningButtonBackColor { get; set; }
        public Color DialogWarningButtonForeColor { get; set; }
        public Color DialogWarningButtonHoverBackColor { get; set; }
        public Color DialogWarningButtonHoverForeColor { get; set; }
        public Color DialogWarningButtonHoverBorderColor { get; set; }
        public Color DialogErrorButtonBackColor { get; set; }
        public Color DialogErrorButtonForeColor { get; set; }
        public Color DialogErrorButtonHoverBackColor { get; set; }
        public Color DialogErrorButtonHoverForeColor { get; set; }
        public Color DialogErrorButtonHoverBorderColor { get; set; }
        public Color DialogInformationButtonBackColor { get; set; }
        public Color DialogInformationButtonForeColor { get; set; }
        public Color DialogInformationButtonHoverBackColor { get; set; }
        public Color DialogInformationButtonHoverForeColor { get; set; }
        public Color DialogInformationButtonHoverBorderColor { get; set; }
        public Color DialogQuestionButtonBackColor { get; set; }
        public Color DialogQuestionButtonForeColor { get; set; }
        public Color DialogQuestionButtonHoverBackColor { get; set; }
        public Color DialogQuestionButtonHoverForeColor { get; set; }
        public Color DialogQuestionButtonHoverBorderColor { get; set; }

        // Fonts & Styles
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public TypographyStyle TitleStyle { get; set; }
        public TypographyStyle SubtitleStyle { get; set; }
        public TypographyStyle BodyStyle { get; set; }
        public TypographyStyle CaptionStyle { get; set; }
        public TypographyStyle ButtonStyle { get; set; }
        public TypographyStyle LinkStyle { get; set; }
        public TypographyStyle OverlineStyle { get; set; }

        // Gradient
        public Color GradientStartColor { get; set; }
        public Color GradientEndColor { get; set; }
        public LinearGradientMode GradientDirection { get; set; }

        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; }
        public TypographyStyle GridRowFont { get; set; }
        public TypographyStyle GridCellFont { get; set; }
        public TypographyStyle GridCellSelectedFont { get; set; }
        public TypographyStyle GridCellHoverFont { get; set; }
        public TypographyStyle GridCellErrorFont { get; set; }
        public TypographyStyle GridColumnFont { get; set; }

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
        public Color GridLineColor { get; set; }
        public Color RowBackColor { get; set; }
        public Color RowForeColor { get; set; }
        public Color AltRowBackColor { get; set; }
        public Color SelectedRowBackColor { get; set; }
        public Color SelectedRowForeColor { get; set; }

        // Labels
        public Color LabelBackColor { get; set; }
        public Color LabelForeColor { get; set; }
        public Color LabelBorderColor { get; set; }
        public Color LabelHoverBorderColor { get; set; }
        public Color LabelHoverBackColor { get; set; }
        public Color LabelHoverForeColor { get; set; }
        public Color LabelSelectedBorderColor { get; set; }
        public Color LabelSelectedBackColor { get; set; }
        public Color LabelSelectedForeColor { get; set; }
        public Color LabelDisabledBackColor { get; set; }
        public Color LabelDisabledForeColor { get; set; }
        public Color LabelDisabledBorderColor { get; set; }
        public TypographyStyle LabelFont { get; set; }
        public TypographyStyle SubLabelFont { get; set; }
        public Color SubLabelForColor { get; set; }
        public Color SubLabelBackColor { get; set; }
        public Color SubLabelHoverBackColor { get; set; }
        public Color SubLabelHoverForeColor { get; set; }

        // Links
        public Color LinkColor { get; set; }
        public Color VisitedLinkColor { get; set; }
        public Color HoverLinkColor { get; set; }
        public Color LinkHoverColor { get; set; }

        // List
        public TypographyStyle ListTitleFont { get; set; }
        public TypographyStyle ListSelectedFont { get; set; }
        public TypographyStyle ListUnSelectedFont { get; set; }
        public Color ListBackColor { get; set; }
        public Color ListForeColor { get; set; }
        public Color ListBorderColor { get; set; }
        public Color ListItemForeColor { get; set; }
        public Color ListItemHoverForeColor { get; set; }
        public Color ListItemHoverBackColor { get; set; }
        public Color ListItemSelectedForeColor { get; set; }
        public Color ListItemSelectedBackColor { get; set; }
        public Color ListItemSelectedBorderColor { get; set; }
        public Color ListItemBorderColor { get; set; }
        public Color ListItemHoverBorderColor { get; set; }

        // Login Popover
        public Color LoginPopoverBackgroundColor { get; set; }
        public Color LoginTitleColor { get; set; }
        public TypographyStyle LoginTitleFont { get; set; }
        public Color LoginSubtitleColor { get; set; }
        public TypographyStyle LoginSubtitleFont { get; set; }
        public Color LoginDescriptionColor { get; set; }
        public TypographyStyle LoginDescriptionFont { get; set; }
        public Color LoginLinkColor { get; set; }
        public TypographyStyle LoginLinkFont { get; set; }
        public Color LoginButtonBackgroundColor { get; set; }
        public Color LoginButtonTextColor { get; set; }
        public TypographyStyle LoginButtonFont { get; set; }
        public Color LoginDropdownBackgroundColor { get; set; }
        public Color LoginDropdownTextColor { get; set; }
        public Color LoginLogoBackgroundColor { get; set; }

        // Menu
        public TypographyStyle MenuTitleFont { get; set; }
        public TypographyStyle MenuItemSelectedFont { get; set; }
        public TypographyStyle MenuItemUnSelectedFont { get; set; }
        public Color MenuBackColor { get; set; }
        public Color MenuForeColor { get; set; }
        public Color MenuBorderColor { get; set; }
        public Color MenuMainItemForeColor { get; set; }
        public Color MenuMainItemHoverForeColor { get; set; }
        public Color MenuMainItemHoverBackColor { get; set; }
        public Color MenuMainItemSelectedForeColor { get; set; }
        public Color MenuMainItemSelectedBackColor { get; set; }
        public Color MenuItemForeColor { get; set; }
        public Color MenuItemHoverForeColor { get; set; }
        public Color MenuItemHoverBackColor { get; set; }
        public Color MenuItemSelectedForeColor { get; set; }
        public Color MenuItemSelectedBackColor { get; set; }
        public Color MenuGradiantStartColor { get; set; }
        public Color MenuGradiantEndColor { get; set; }
        public Color MenuGradiantMiddleColor { get; set; }
        public LinearGradientMode MenuGradiantDirection { get; set; }

        // Misc / Utility
        public string FontFamily { get; set; }
        public float FontSizeBlockHeader { get; set; }
        public float FontSizeBlockText { get; set; }
        public float FontSizeQuestion { get; set; }
        public float FontSizeAnswer { get; set; }
        public float FontSizeCaption { get; set; }
        public float FontSizeButton { get; set; }
        public FontStyle FontStyleRegular { get; set; }
        public FontStyle FontStyleBold { get; set; }
        public FontStyle FontStyleItalic { get; set; }
        public Color PrimaryTextColor { get; set; }
        public Color SecondaryTextColor { get; set; }
        public Color AccentTextColor { get; set; }
        public int PaddingSmall { get; set; }
        public int PaddingMedium { get; set; }
        public int PaddingLarge { get; set; }
        public int BorderRadius { get; set; }
        public int BorderSize { get; set; }
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

        // Navigation
        public TypographyStyle NavigationTitleFont { get; set; }
        public TypographyStyle NavigationSelectedFont { get; set; }
        public TypographyStyle NavigationUnSelectedFont { get; set; }
        public Color NavigationBackColor { get; set; }
        public Color NavigationForeColor { get; set; }
        public Color NavigationHoverBackColor { get; set; }
        public Color NavigationHoverForeColor { get; set; }
        public Color NavigationSelectedBackColor { get; set; }
        public Color NavigationSelectedForeColor { get; set; }

        // ProgressBar
        public Color ProgressBarBackColor { get; set; }
        public Color ProgressBarForeColor { get; set; }
        public Color ProgressBarBorderColor { get; set; }
        public Color ProgressBarChunkColor { get; set; }
        public Color ProgressBarErrorColor { get; set; }
        public Color ProgressBarSuccessColor { get; set; }
        public TypographyStyle ProgressBarFont { get; set; }
        public Color ProgressBarInsideTextColor { get; set; }
        public Color ProgressBarHoverBackColor { get; set; }
        public Color ProgressBarHoverForeColor { get; set; }
        public Color ProgressBarHoverBorderColor { get; set; }
        public Color ProgressBarHoverInsideTextColor { get; set; }

        // RadioButton
        public Color RadioButtonBackColor { get; set; }
        public Color RadioButtonForeColor { get; set; }
        public Color RadioButtonBorderColor { get; set; }
        public Color RadioButtonCheckedBackColor { get; set; }
        public Color RadioButtonCheckedForeColor { get; set; }
        public Color RadioButtonCheckedBorderColor { get; set; }
        public Color RadioButtonHoverBackColor { get; set; }
        public Color RadioButtonHoverForeColor { get; set; }
        public Color RadioButtonHoverBorderColor { get; set; }
        public TypographyStyle RadioButtonFont { get; set; }
        public TypographyStyle RadioButtonCheckedFont { get; set; }
        public Color RadioButtonSelectedForeColor { get; set; }
        public Color RadioButtonSelectedBackColor { get; set; }

        // ScrollBar
        public Color ScrollBarBackColor { get; set; }
        public Color ScrollBarThumbColor { get; set; }
        public Color ScrollBarTrackColor { get; set; }
        public Color ScrollBarHoverThumbColor { get; set; }
        public Color ScrollBarHoverTrackColor { get; set; }
        public Color ScrollBarActiveThumbColor { get; set; }

        // ScrollList
        public TypographyStyle ScrollListTitleFont { get; set; }
        public TypographyStyle ScrollListSelectedFont { get; set; }
        public TypographyStyle ScrollListUnSelectedFont { get; set; }
        public Color ScrollListBackColor { get; set; }
        public Color ScrollListForeColor { get; set; }
        public Color ScrollListBorderColor { get; set; }
        public Color ScrollListItemForeColor { get; set; }
        public Color ScrollListItemHoverForeColor { get; set; }
        public Color ScrollListItemHoverBackColor { get; set; }
        public Color ScrollListItemSelectedForeColor { get; set; }
        public Color ScrollListItemSelectedBackColor { get; set; }
        public Color ScrollListItemSelectedBorderColor { get; set; }
        public Color ScrollListItemBorderColor { get; set; }
        public TypographyStyle ScrollListIItemFont { get; set; }
        public TypographyStyle ScrollListItemSelectedFont { get; set; }

        // Side Menu
        public TypographyStyle SideMenuTitleFont { get; set; }
        public TypographyStyle SideMenuSubTitleFont { get; set; }
        public TypographyStyle SideMenuTextFont { get; set; }
        public Color SideMenuBackColor { get; set; }
        public Color SideMenuHoverBackColor { get; set; }
        public Color SideMenuSelectedBackColor { get; set; }
        public Color SideMenuForeColor { get; set; }
        public Color SideMenuSelectedForeColor { get; set; }
        public Color SideMenuHoverForeColor { get; set; }
        public Color SideMenuBorderColor { get; set; }
        public Color SideMenuTitleTextColor { get; set; }
        public Color SideMenuTitleBackColor { get; set; }
        public TypographyStyle SideMenuTitleStyle { get; set; }
        public Color SideMenuSubTitleTextColor { get; set; }
        public Color SideMenuSubTitleBackColor { get; set; }
        public TypographyStyle SideMenuSubTitleStyle { get; set; }
        public Color SideMenuGradiantStartColor { get; set; }
        public Color SideMenuGradiantEndColor { get; set; }
        public Color SideMenuGradiantMiddleColor { get; set; }
        public LinearGradientMode SideMenuGradiantDirection { get; set; }

        // Star Rating
        public Color StarRatingForeColor { get; set; }
        public Color StarRatingBackColor { get; set; }
        public Color StarRatingBorderColor { get; set; }
        public Color StarRatingFillColor { get; set; }
        public Color StarRatingHoverForeColor { get; set; }
        public Color StarRatingHoverBackColor { get; set; }
        public Color StarRatingHoverBorderColor { get; set; }
        public Color StarRatingSelectedForeColor { get; set; }
        public Color StarRatingSelectedBackColor { get; set; }
        public Color StarRatingSelectedBorderColor { get; set; }
        public TypographyStyle StarTitleFont { get; set; }
        public TypographyStyle StarSubTitleFont { get; set; }
        public TypographyStyle StarSelectedFont { get; set; }
        public TypographyStyle StarUnSelectedFont { get; set; }
        public Color StarTitleForeColor { get; set; }
        public Color StarTitleBackColor { get; set; }

        // Stats Card
        public TypographyStyle StatsTitleFont { get; set; }
        public TypographyStyle StatsSelectedFont { get; set; }
        public TypographyStyle StatsUnSelectedFont { get; set; }
        public Color StatsCardBackColor { get; set; }
        public Color StatsCardForeColor { get; set; }
        public Color StatsCardBorderColor { get; set; }
        public Color StatsCardTitleForeColor { get; set; }
        public Color StatsCardTitleBackColor { get; set; }
        public TypographyStyle StatsCardTitleStyle { get; set; }
        public Color StatsCardSubTitleForeColor { get; set; }
        public Color StatsCardSubTitleBackColor { get; set; }
        public TypographyStyle StatsCardSubStyleStyle { get; set; }
        public Color StatsCardValueForeColor { get; set; }
        public Color StatsCardValueBackColor { get; set; }
        public Color StatsCardValueBorderColor { get; set; }
        public Color StatsCardValueHoverForeColor { get; set; }
        public Color StatsCardValueHoverBackColor { get; set; }
        public Color StatsCardValueHoverBorderColor { get; set; }
        public TypographyStyle StatsCardValueStyle { get; set; }
        public Color StatsCardInfoForeColor { get; set; }
        public Color StatsCardInfoBackColor { get; set; }
        public Color StatsCardInfoBorderColor { get; set; }
        public TypographyStyle StatsCardInfoStyle { get; set; }
        public Color StatsCardTrendForeColor { get; set; }
        public Color StatsCardTrendBackColor { get; set; }
        public Color StatsCardTrendBorderColor { get; set; }
        public TypographyStyle StatsCardTrendStyle { get; set; }

        // Status Bar
        public Color StatusBarBackColor { get; set; }
        public Color StatusBarForeColor { get; set; }
        public Color StatusBarBorderColor { get; set; }
        public Color StatusBarHoverBackColor { get; set; }
        public Color StatusBarHoverForeColor { get; set; }
        public Color StatusBarHoverBorderColor { get; set; }

        // Stepper
        public TypographyStyle StepperTitleFont { get; set; }
        public TypographyStyle StepperSelectedFont { get; set; }
        public TypographyStyle StepperUnSelectedFont { get; set; }
        public Color StepperBackColor { get; set; }
        public Color StepperForeColor { get; set; }
        public Color StepperBorderColor { get; set; }
        public Color StepperItemForeColor { get; set; }
        public TypographyStyle StepperItemFont { get; set; }
        public TypographyStyle StepperSubTitleFont { get; set; }
        public Color StepperItemHoverForeColor { get; set; }
        public Color StepperItemHoverBackColor { get; set; }
        public Color StepperItemSelectedForeColor { get; set; }
        public Color StepperItemSelectedBackColor { get; set; }
        public Color StepperItemSelectedBorderColor { get; set; }
        public Color StepperItemBorderColor { get; set; }
        public Color StepperItemHoverBorderColor { get; set; }
        public Color StepperItemCheckedBoxForeColor { get; set; }
        public Color StepperItemCheckedBoxBackColor { get; set; }
        public Color StepperItemCheckedBoxBorderColor { get; set; }

        // Switch
        public TypographyStyle SwitchTitleFont { get; set; }
        public TypographyStyle SwitchSelectedFont { get; set; }
        public TypographyStyle SwitchUnSelectedFont { get; set; }
        public Color SwitchBackColor { get; set; }
        public Color SwitchBorderColor { get; set; }
        public Color SwitchForeColor { get; set; }
        public Color SwitchSelectedBackColor { get; set; }
        public Color SwitchSelectedBorderColor { get; set; }
        public Color SwitchSelectedForeColor { get; set; }
        public Color SwitchHoverBackColor { get; set; }
        public Color SwitchHoverBorderColor { get; set; }
        public Color SwitchHoverForeColor { get; set; }

        // Tabs
        public TypographyStyle TabFont { get; set; }
        public TypographyStyle TabHoverFont { get; set; }
        public TypographyStyle TabSelectedFont { get; set; }
        public Color TabBackColor { get; set; }
        public Color TabForeColor { get; set; }
        public Color ActiveTabBackColor { get; set; }
        public Color ActiveTabForeColor { get; set; }
        public Color InactiveTabBackColor { get; set; }
        public Color InactiveTabForeColor { get; set; }
        public Color TabBorderColor { get; set; }
        public Color TabHoverBackColor { get; set; }
        public Color TabHoverForeColor { get; set; }
        public Color TabSelectedBackColor { get; set; }
        public Color TabSelectedForeColor { get; set; }
        public Color TabSelectedBorderColor { get; set; }
        public Color TabHoverBorderColor { get; set; }

        // Task Card
        public TypographyStyle TaskCardTitleFont { get; set; }
        public TypographyStyle TaskCardSelectedFont { get; set; }
        public TypographyStyle TaskCardUnSelectedFont { get; set; }
        public Color TaskCardBackColor { get; set; }
        public Color TaskCardForeColor { get; set; }
        public Color TaskCardBorderColor { get; set; }
        public Color TaskCardTitleForeColor { get; set; }
        public Color TaskCardTitleBackColor { get; set; }
        public TypographyStyle TaskCardTitleStyle { get; set; }
        public Color TaskCardSubTitleForeColor { get; set; }
        public Color TaskCardSubTitleBackColor { get; set; }
        public TypographyStyle TaskCardSubStyleStyle { get; set; }
        public Color TaskCardMetricTextForeColor { get; set; }
        public Color TaskCardMetricTextBackColor { get; set; }
        public Color TaskCardMetricTextBorderColor { get; set; }
        public Color TaskCardMetricTextHoverForeColor { get; set; }
        public Color TaskCardMetricTextHoverBackColor { get; set; }
        public Color TaskCardMetricTextHoverBorderColor { get; set; }
        public TypographyStyle TaskCardMetricTextStyle { get; set; }
        public Color TaskCardProgressValueForeColor { get; set; }
        public Color TaskCardProgressValueBackColor { get; set; }
        public Color TaskCardProgressValueBorderColor { get; set; }
        public TypographyStyle TaskCardProgressValueStyle { get; set; }

        // Testimonial
        public TypographyStyle TestimoniaTitleFont { get; set; }
        public TypographyStyle TestimoniaSelectedFont { get; set; }
        public TypographyStyle TestimoniaUnSelectedFont { get; set; }
        public Color TestimonialBackColor { get; set; }
        public Color TestimonialTextColor { get; set; }
        public Color TestimonialNameColor { get; set; }
        public Color TestimonialDetailsColor { get; set; }
        public Color TestimonialDateColor { get; set; }
        public Color TestimonialRatingColor { get; set; }
        public Color TestimonialStatusColor { get; set; }

        // TextBox
        public Color TextBoxBackColor { get; set; }
        public Color TextBoxForeColor { get; set; }
        public Color TextBoxBorderColor { get; set; }
        public Color TextBoxHoverBorderColor { get; set; }
        public Color TextBoxHoverBackColor { get; set; }
        public Color TextBoxHoverForeColor { get; set; }
        public Color TextBoxSelectedBorderColor { get; set; }
        public Color TextBoxSelectedBackColor { get; set; }
        public Color TextBoxSelectedForeColor { get; set; }
        public Color TextBoxPlaceholderColor { get; set; }
        public Color TextBoxErrorBorderColor { get; set; }
        public Color TextBoxErrorBackColor { get; set; }
        public Color TextBoxErrorForeColor { get; set; }
        public Color TextBoxErrorTextColor { get; set; }
        public Color TextBoxErrorPlaceholderColor { get; set; }
        public Color TextBoxErrorTextBoxColor { get; set; }
        public Color TextBoxErrorTextBoxBorderColor { get; set; }
        public Color TextBoxErrorTextBoxHoverColor { get; set; }
        public TypographyStyle TextBoxFont { get; set; }
        public TypographyStyle TextBoxHoverFont { get; set; }
        public TypographyStyle TextBoxSelectedFont { get; set; }

        // ToolTip
        public Color ToolTipBackColor { get; set; }
        public Color ToolTipForeColor { get; set; }
        public Color ToolTipBorderColor { get; set; }
        public Color ToolTipShadowColor { get; set; }
        public Color ToolTipShadowOpacity { get; set; }
        public Color ToolTipTextColor { get; set; }
        public Color ToolTipLinkColor { get; set; }
        public Color ToolTipLinkHoverColor { get; set; }
        public Color ToolTipLinkVisitedColor { get; set; }

        // Tree
        public TypographyStyle TreeTitleFont { get; set; }
        public TypographyStyle TreeNodeSelectedFont { get; set; }
        public TypographyStyle TreeNodeUnSelectedFont { get; set; }
        public Color TreeBackColor { get; set; }
        public Color TreeForeColor { get; set; }
        public Color TreeBorderColor { get; set; }
        public Color TreeNodeForeColor { get; set; }
        public Color TreeNodeHoverForeColor { get; set; }
        public Color TreeNodeHoverBackColor { get; set; }
        public Color TreeNodeSelectedForeColor { get; set; }
        public Color TreeNodeSelectedBackColor { get; set; }
        public Color TreeNodeCheckedBoxForeColor { get; set; }
        public Color TreeNodeCheckedBoxBackColor { get; set; }

        // Typography (rich set)
        public TypographyStyle Heading1 { get; set; }
        public TypographyStyle Heading2 { get; set; }
        public TypographyStyle Heading3 { get; set; }
        public TypographyStyle Heading4 { get; set; }
        public TypographyStyle Heading5 { get; set; }
        public TypographyStyle Heading6 { get; set; }
        public TypographyStyle Paragraph { get; set; }
        public TypographyStyle Blockquote { get; set; }
        public float BlockquoteBorderWidth { get; set; }
        public float BlockquotePadding { get; set; }
        public TypographyStyle InlineCode { get; set; }
        public float InlineCodePadding { get; set; }
        public TypographyStyle CodeBlock { get; set; }
        public float CodeBlockBorderWidth { get; set; }
        public float CodeBlockPadding { get; set; }
        public TypographyStyle UnorderedList { get; set; }
        public TypographyStyle OrderedList { get; set; }
        public float ListItemSpacing { get; set; }
        public float ListIndentation { get; set; }
        public TypographyStyle Link { get; set; }
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

        // Constructor
        public RadixUITheme()
        {
            // --- Base palette (Radix UI - Accessible)
            var background = C(255, 255, 255);      // #FFFFFF - white
            var surface = C(248, 248, 248);         // #F8F8F8 - light gray
            var foreground = C(26, 26, 26);         // #1A1A1A - high contrast black
            var border = C(200, 200, 200);          // #C8C8C8 - medium contrast
            var primary = C(22, 24, 29);            // #16181D - very dark
            var secondary = C(107, 114, 128);       // #6B7280 - medium gray
            var accent = C(37, 99, 235);           // #2563EB - accessible blue
            var muted = C(249, 250, 251);          // #F9FAFB
            var mutedForeground = C(107, 114, 128); // #6B7280
            
            // Theme color aliases (replacing terminal green colors with Radix UI colors)
            var softGreen = foreground;      // Main text color
            var dimGreen = mutedForeground;   // Secondary/muted text
            var neonGreen = accent;           // Accent/primary color
            var lineGreen = border;           // Border color

            // Core palette
            PrimaryColor = primary;
            SecondaryColor = secondary;
            AccentColor = accent;
            BackgroundColor = background;
            SurfaceColor = surface;
            ErrorColor = C(220, 38, 38);            // #DC2626 - high contrast red
            WarningColor = C(234, 88, 12);         // #EA580C - high contrast orange
            SuccessColor = C(22, 163, 74);          // #16A34A - high contrast green
            OnPrimaryColor = C(255, 255, 255);      // white on primary
            OnBackgroundColor = foreground;

            BorderColor = border;

            // Core colors
            ForeColor = foreground;
            BackColor = background;
            PanelBackColor = surface;
            PanelGradiantStartColor = surface;
            PanelGradiantEndColor = surface;
            PanelGradiantMiddleColor = surface;
            PanelGradiantDirection = LinearGradientMode.Vertical;

            DisabledBackColor = C(249, 250, 251);
            DisabledForeColor = C(156, 163, 175);
            DisabledBorderColor = C(229, 231, 235);
            BorderColor = border;
            ActiveBorderColor = accent;
            InactiveBorderColor = border;
            // --- AppBar / Caption (matches RadixUIFormPainter)
            AppBarBackColor = surface;
            AppBarForeColor = foreground;
            AppBarButtonBackColor = background;
            AppBarButtonForeColor = foreground;
            AppBarTextBoxBackColor = background;
            AppBarTextBoxForeColor = foreground;
            AppBarLabelBackColor = AppBarBackColor;
            AppBarLabelForeColor = foreground;
            AppBarTitleBackColor = AppBarBackColor;
            AppBarTitleForeColor = foreground;
            AppBarSubTitleBackColor = AppBarBackColor;
            AppBarSubTitleForeColor = mutedForeground;

            // Radix UI control colors (high contrast)
            AppBarCloseButtonColor = ErrorColor;  // red
            AppBarMaxButtonColor = SuccessColor;  // green
            AppBarMinButtonColor = accent;        // blue

            // Typography for AppBar (sans-serif, high contrast)
            AppBarTitleStyle = TS(12, FontStyle.Bold, 600, foreground, mono: false);
            AppBarSubTitleStyle = TS(10, FontStyle.Regular, 400, mutedForeground, mono: false);
            AppBarTextStyle = TS(8, FontStyle.Regular, 400, foreground, mono: false);

            AppBarGradiantStartColor = AppBarBackColor;
            AppBarGradiantMiddleColor = AppBarBackColor;
            AppBarGradiantEndColor = AppBarBackColor;
            AppBarGradiantDirection = LinearGradientMode.Vertical;

            // --- Typography (Sans-serif)
            FontName = PreferSansSerif();
            FontFamily = FontName;
            FontSize = 8f;

            TitleStyle = TS(20, FontStyle.Bold, 700, foreground, mono: false);
            SubtitleStyle = TS(14, FontStyle.Regular, 600, mutedForeground, mono: false);
            BodyStyle = TS(8, FontStyle.Regular, 400, foreground, mono: false);
            CaptionStyle = TS(8, FontStyle.Regular, 400, mutedForeground, mono: false);
            ButtonStyle = TS(8, FontStyle.Bold, 600, C(255, 255, 255), mono: false);
            LinkStyle = TS(8, FontStyle.Regular, 500, accent, mono: false);
            LinkStyle.IsUnderlined = true; // Links should be underlined
            OverlineStyle = TS(8, FontStyle.Regular, 600, mutedForeground, mono: false);

            DisplayLarge = TS(44, FontStyle.Regular, 300, foreground, mono: false);
            DisplayMedium = TS(34, FontStyle.Regular, 300, foreground, mono: false);
            DisplaySmall = TS(26, FontStyle.Regular, 400, foreground, mono: false);
            HeadlineLarge = TS(22, FontStyle.Bold, 600, foreground, mono: false);
            HeadlineMedium = TS(20, FontStyle.Bold, 600, foreground, mono: false);
            HeadlineSmall = TS(18, FontStyle.Bold, 600, foreground, mono: false);
            TitleLarge = TS(16, FontStyle.Bold, 600, foreground, mono: false);
            TitleMedium = TS(14, FontStyle.Bold, 600, foreground, mono: false);
            TitleSmall = TS(12, FontStyle.Bold, 600, foreground, mono: false);
            BodyLarge = TS(12, FontStyle.Regular, 400, foreground, mono: false);
            BodyMedium = TS(10, FontStyle.Regular, 400, foreground, mono: false);
            BodySmall = TS(8, FontStyle.Regular, 400, mutedForeground, mono: false);
            LabelLarge = TS(12, FontStyle.Regular, 500, foreground, mono: false);
            LabelMedium = TS(10, FontStyle.Regular, 500, foreground, mono: false);
            LabelSmall = TS(8, FontStyle.Regular, 500, mutedForeground, mono: false);

            FontSizeBlockHeader = 14f;
            FontSizeBlockText = 8f;
            FontSizeQuestion = 12f;
            FontSizeAnswer = 10f;
            FontSizeCaption = 8f;
            FontSizeButton = 8f;

            FontStyleRegular = FontStyle.Regular;
            FontStyleBold = FontStyle.Bold;
            FontStyleItalic = FontStyle.Italic;

            PrimaryTextColor = foreground;
            SecondaryTextColor = mutedForeground;
            AccentTextColor = accent;

            PaddingSmall = 6;
            PaddingMedium = 10;
            PaddingLarge = 16;
            BorderRadius = 6;   // rounded corners for Radix UI
            BorderSize = 2;      // Medium borders for accessibility

            IconSet = "Minimal";
            ApplyThemeToIcons = true;
            ShadowColor = C(0, 0, 0);
            ShadowOpacity = 0.2f; // subtle shadows
            AnimationDurationShort = 150.0;
            AnimationDurationMedium = 200.0;
            AnimationDurationLong = 300.0;
            AnimationEasingFunction = "ease-in-out";
            HighContrastMode = true;  // Radix UI emphasizes accessibility
            FocusIndicatorColor = accent;
            IsDarkTheme = false;

            // --- Buttons
            // Radix UI style: neutral default button, distinct from background/surface
            ButtonBackColor = C(232, 234, 238);
            ButtonForeColor = foreground;  // Theme text
            ButtonBorderColor = C(200, 200, 200);  // Medium contrast border
            
            // Hover: slightly darker neutral with focus ring
            ButtonHoverBackColor = C(220, 224, 230);
            ButtonHoverForeColor = foreground;
            ButtonHoverBorderColor = accent;  // Accent border on hover
            
            // Pressed: strongest neutral
            ButtonPressedBackColor = C(206, 212, 220);
            ButtonPressedForeColor = foreground;
            ButtonPressedBorderColor = accent;

            // Selected: accent color with high contrast
            ButtonSelectedBackColor = accent;
            ButtonSelectedForeColor = C(255, 255, 255);
            ButtonSelectedBorderColor = accent;
            
            // Selected hover: darker accent
            ButtonSelectedHoverBackColor = C(29, 78, 216);
            ButtonSelectedHoverForeColor = C(255, 255, 255);
            ButtonSelectedHoverBorderColor = accent;

            // Error button: high contrast red
            ButtonErrorBackColor = ErrorColor;
            ButtonErrorForeColor = C(255, 255, 255);
            ButtonErrorBorderColor = ErrorColor;

            ButtonFont = ButtonStyle;
            ButtonHoverFont = ButtonStyle;
            ButtonSelectedFont = ButtonStyle;

            // --- Cards
            CardBackColor = surface;
            CardTextForeColor = foreground;
            CardTitleForeColor = foreground;
            CardSubTitleForeColor = mutedForeground;
            CardTitleFont = TitleSmall;
            CardSubTitleFont = BodySmall;
            CardHeaderStyle = TitleSmall;
            CardparagraphStyle = BodyMedium;
            CardSubTitleStyle = BodySmall;
            CardrGradiantStartColor = CardBackColor;
            CardGradiantMiddleColor = CardBackColor;
            CardGradiantEndColor = CardBackColor;
            CardGradiantDirection = LinearGradientMode.Vertical;

            // --- Calendar
            CalendarTitleFont = TitleSmall;
            CalendarTitleForColor = softGreen;
            DaysHeaderFont = LabelSmall;
            CalendarDaysHeaderForColor = dimGreen;
            SelectedDateFont = LabelSmall;
            CalendarSelectedDateBackColor = C(20, 40, 20);
            CalendarSelectedDateForColor = neonGreen;
            CalendarSelectedFont = LabelSmall;
            CalendarUnSelectedFont = LabelSmall;
            CalendarBackColor = SurfaceColor;
            CalendarForeColor = softGreen;
            CalendarTodayForeColor = neonGreen;
            CalendarBorderColor = lineGreen;
            CalendarHoverBackColor = C(18, 30, 18);
            CalendarHoverForeColor = neonGreen;
            HeaderFont = TitleSmall;
            MonthFont = TitleSmall;
            YearFont = TitleSmall;
            DaysFont = LabelSmall;
            DaysSelectedFont = LabelSmall;
            DateFont = LabelSmall;
            CalendarFooterColor = dimGreen;
            FooterFont = LabelSmall;

            // --- Charts
            ChartTitleFont = TitleSmall;
            ChartSubTitleFont = BodySmall;
            ChartBackColor = CardBackColor;
            ChartLineColor = softGreen;
            ChartFillColor = C(12, 32, 20);
            ChartAxisColor = dimGreen;
            ChartTitleColor = softGreen;
            ChartTextColor = softGreen;
            ChartLegendBackColor = CardBackColor;
            ChartLegendTextColor = softGreen;
            ChartLegendShapeColor = softGreen;
            ChartGridLineColor = C(22, 40, 28);
            ChartDefaultSeriesColors = new()
            {
                softGreen, neonGreen, C(0,200,120), C(0,255,204),
                C(120,255,200), C(0,180,120), C(0,255,170), C(0,220,150)
            };

            // --- CheckBox / Radio
            CheckBoxBackColor = background;
            CheckBoxForeColor = foreground;
            CheckBoxBorderColor = border;
            CheckBoxCheckedBackColor = primary;
            CheckBoxCheckedForeColor = C(255, 255, 255);
            CheckBoxCheckedBorderColor = primary;
            CheckBoxHoverBackColor = muted;
            CheckBoxHoverForeColor = foreground;
            CheckBoxHoverBorderColor = accent;  // Focus ring
            CheckBoxFont = BodySmall;
            CheckBoxCheckedFont = BodySmall;

            RadioButtonBackColor = background;
            RadioButtonForeColor = foreground;
            RadioButtonBorderColor = border;
            RadioButtonCheckedBackColor = primary;
            RadioButtonCheckedForeColor = C(255, 255, 255);
            RadioButtonCheckedBorderColor = primary;
            RadioButtonHoverBackColor = muted;
            RadioButtonHoverForeColor = foreground;
            RadioButtonHoverBorderColor = accent;  // Focus ring
            RadioButtonFont = BodySmall;
            RadioButtonCheckedFont = BodySmall;
            RadioButtonSelectedForeColor = C(255, 255, 255);
            RadioButtonSelectedBackColor = primary;

            // --- ComboBox
            ComboBoxBackColor = background;
            ComboBoxForeColor = foreground;
            ComboBoxBorderColor = border;
            ComboBoxHoverBackColor = background;
            ComboBoxHoverForeColor = foreground;
            ComboBoxHoverBorderColor = accent;  // Focus ring
            ComboBoxSelectedBackColor = muted;
            ComboBoxSelectedForeColor = foreground;
            ComboBoxSelectedBorderColor = accent;
            ComboBoxErrorBackColor = C(254, 242, 242);
            ComboBoxErrorForeColor = ErrorColor;
            ComboBoxItemFont = BodySmall;
            ComboBoxListFont = BodySmall;

            // --- Lists
            ListBackColor = CardBackColor;
            ListForeColor = softGreen;
            ListBorderColor = lineGreen;
            ListItemForeColor = softGreen;
            ListItemHoverForeColor = neonGreen;
            ListItemHoverBackColor = C(16, 32, 20);
            ListItemSelectedForeColor = neonGreen;
            ListItemSelectedBackColor = C(16, 32, 20);
            ListItemSelectedBorderColor = lineGreen;
            ListItemBorderColor = lineGreen;
            ListItemHoverBorderColor = lineGreen;
            ListTitleFont = TitleSmall;
            ListSelectedFont = BodySmall;
            ListUnSelectedFont = BodySmall;

            // --- Links
            LinkColor = neonGreen;
            VisitedLinkColor = C(0, 200, 120);
            HoverLinkColor = softGreen;
            LinkHoverColor = HoverLinkColor;

            // --- Labels
            LabelBackColor = BackgroundColor;
            LabelForeColor = softGreen;
            LabelBorderColor = lineGreen;  // Green for consistency (borders usually off anyway)
            LabelHoverBorderColor = lineGreen;
            LabelHoverBackColor = BackgroundColor;
            LabelHoverForeColor = neonGreen;
            LabelSelectedBorderColor = lineGreen;
            LabelSelectedBackColor = C(16, 32, 20);
            LabelSelectedForeColor = neonGreen;
            LabelDisabledBackColor = DisabledBackColor;
            LabelDisabledForeColor = DisabledForeColor;
            LabelDisabledBorderColor = DisabledBorderColor;
            LabelFont = BodySmall;
            SubLabelFont = BodySmall;
            SubLabelBackColor = BackgroundColor;
            SubLabelForColor = dimGreen;
            SubLabelHoverBackColor = BackgroundColor;
            SubLabelHoverForeColor = neonGreen;

            // --- TextBox
            TextBoxBackColor = background;
            TextBoxForeColor = foreground;
            TextBoxBorderColor = border;
            TextBoxHoverBackColor = background;
            TextBoxHoverForeColor = foreground;
            TextBoxHoverBorderColor = accent;  // Focus ring color
            TextBoxSelectedBackColor = background;
            TextBoxSelectedForeColor = foreground;
            TextBoxSelectedBorderColor = accent;
            TextBoxPlaceholderColor = mutedForeground;
            TextBoxErrorBorderColor = ErrorColor;
            TextBoxErrorBackColor = C(254, 242, 242);
            TextBoxErrorForeColor = ErrorColor;
            TextBoxErrorTextColor = ErrorColor;
            TextBoxErrorPlaceholderColor = ErrorColor;
            TextBoxErrorTextBoxColor = TextBoxErrorBackColor;
            TextBoxErrorTextBoxBorderColor = ErrorColor;
            TextBoxErrorTextBoxHoverColor = C(254, 242, 242);
            TextBoxFont = BodySmall;
            TextBoxHoverFont = BodySmall;
            TextBoxSelectedFont = BodySmall;

            // --- Grid
            GridHeaderFont = LabelSmall;
            GridRowFont = BodySmall;
            GridCellFont = BodySmall;
            GridCellSelectedFont = BodySmall;
            GridCellHoverFont = BodySmall;
            GridCellErrorFont = BodySmall;
            GridColumnFont = LabelSmall;
            GridBackColor = CardBackColor;
            GridForeColor = softGreen;
            GridHeaderBackColor = C(16, 16, 16);
            GridHeaderForeColor = softGreen;
            GridHeaderBorderColor = lineGreen;
            GridHeaderHoverBackColor = C(18, 18, 18);
            GridHeaderHoverForeColor = neonGreen;
            GridHeaderSelectedBackColor = C(18, 30, 18);
            GridHeaderSelectedForeColor = neonGreen;
            GridHeaderHoverBorderColor = lineGreen;
            GridHeaderSelectedBorderColor = lineGreen;
            GridRowHoverBackColor = C(16, 20, 16);
            GridRowHoverForeColor = neonGreen;
            GridRowSelectedBackColor = C(16, 32, 20);
            GridRowSelectedForeColor = neonGreen;
            GridRowHoverBorderColor = lineGreen;
            GridRowSelectedBorderColor = lineGreen;
            GridLineColor = C(18, 40, 22);
            RowBackColor = BackgroundColor;
            RowForeColor = softGreen;
            AltRowBackColor = C(14, 14, 14);
            SelectedRowBackColor = GridRowSelectedBackColor;
            SelectedRowForeColor = GridRowSelectedForeColor;

            // --- Navigation / Menu / SideMenu
            NavigationTitleFont = TitleSmall;
            NavigationSelectedFont = BodySmall;
            NavigationUnSelectedFont = BodySmall;
            NavigationBackColor = BackgroundColor;
            NavigationForeColor = softGreen;
            NavigationHoverBackColor = C(16, 32, 20);
            NavigationHoverForeColor = neonGreen;
            NavigationSelectedBackColor = C(16, 32, 20);
            NavigationSelectedForeColor = neonGreen;

            // Terminal menu - high contrast with neon green
            MenuTitleFont = TitleSmall;
            MenuItemSelectedFont = BodySmall;
            MenuItemUnSelectedFont = BodySmall;
            MenuBackColor = CardBackColor;  // Dark panel
            MenuForeColor = softGreen;
            MenuBorderColor = lineGreen;
            
            // Main menu items
            MenuMainItemForeColor = softGreen;
            MenuMainItemHoverForeColor = neonGreen;  // Bright green on hover
            MenuMainItemHoverBackColor = C(16, 32, 20);  // Dark green hover
            MenuMainItemSelectedForeColor = C(0, 255, 204);  // Very bright cyan-green when selected
            MenuMainItemSelectedBackColor = C(20, 45, 25);  // Slightly lighter green
            
            // Sub menu items
            MenuItemForeColor = softGreen;
            MenuItemHoverForeColor = neonGreen;  // Bright green on hover
            MenuItemHoverBackColor = C(14, 28, 18);
            MenuItemSelectedForeColor = C(0, 255, 204);  // Very bright cyan-green when selected
            MenuItemSelectedBackColor = C(18, 40, 22);  // Slightly lighter green
            
            MenuGradiantStartColor = MenuBackColor;
            MenuGradiantMiddleColor = MenuBackColor;
            MenuGradiantEndColor = MenuBackColor;
            MenuGradiantDirection = LinearGradientMode.Vertical;

            SideMenuBackColor = C(12, 12, 12); // distinct from Background
            SideMenuHoverBackColor = C(16, 32, 20);
            SideMenuSelectedBackColor = C(16, 32, 20);
            SideMenuForeColor = softGreen;
            SideMenuHoverForeColor = neonGreen;
            SideMenuSelectedForeColor = neonGreen;
            SideMenuBorderColor = lineGreen;
            SideMenuTitleTextColor = softGreen;
            SideMenuTitleBackColor = SideMenuBackColor;
            SideMenuTitleStyle = TitleSmall;
            SideMenuSubTitleTextColor = dimGreen;
            SideMenuSubTitleBackColor = SideMenuBackColor;
            SideMenuSubTitleStyle = BodySmall;
            SideMenuGradiantStartColor = SideMenuBackColor;
            SideMenuGradiantMiddleColor = SideMenuBackColor;
            SideMenuGradiantEndColor = SideMenuBackColor;
            SideMenuGradiantDirection = LinearGradientMode.Vertical;

            // --- Progress Bar
            ProgressBarBackColor = C(20, 24, 20);
            ProgressBarForeColor = softGreen;
            ProgressBarBorderColor = lineGreen;
            ProgressBarChunkColor = softGreen;
            ProgressBarErrorColor = ErrorColor;
            ProgressBarSuccessColor = SuccessColor;
            ProgressBarFont = LabelSmall;
            ProgressBarInsideTextColor = C(0, 0, 0);  // Black text on bright green progress bar
            ProgressBarHoverBackColor = C(18, 30, 18);
            ProgressBarHoverForeColor = neonGreen;
            ProgressBarHoverBorderColor = lineGreen;
            ProgressBarHoverInsideTextColor = C(0, 0, 0);  // Black text for readability

            // --- Scrollbars
            ScrollBarBackColor = C(14, 14, 14);
            ScrollBarTrackColor = C(14, 14, 14);
            ScrollBarThumbColor = C(26, 48, 32);
            ScrollBarHoverTrackColor = C(18, 18, 18);
            ScrollBarHoverThumbColor = C(28, 60, 38);
            ScrollBarActiveThumbColor = C(36, 80, 48);

            // --- Status bar
            StatusBarBackColor = C(12, 12, 12);
            StatusBarForeColor = softGreen;
            StatusBarBorderColor = lineGreen;
            StatusBarHoverBackColor = C(16, 16, 16);
            StatusBarHoverForeColor = neonGreen;
            StatusBarHoverBorderColor = lineGreen;

            // --- Star rating
            StarRatingBackColor = BackgroundColor;
            StarRatingForeColor = neonGreen;
            StarRatingFillColor = neonGreen;
            StarRatingBorderColor = lineGreen;
            StarRatingHoverBackColor = C(16, 32, 20);
            StarRatingHoverForeColor = neonGreen;
            StarRatingHoverBorderColor = lineGreen;
            StarRatingSelectedBackColor = C(16, 32, 20);
            StarRatingSelectedForeColor = neonGreen;
            StarRatingSelectedBorderColor = lineGreen;
            StarTitleFont = TitleSmall;
            StarSubTitleFont = BodySmall;
            StarSelectedFont = BodySmall;
            StarUnSelectedFont = BodySmall;
            StarTitleForeColor = softGreen;
            StarTitleBackColor = BackgroundColor;

            // --- Stats / Dashboard / Task
            DashboardTitleFont = TitleSmall;
            DashboardSubTitleFont = BodySmall;
            DashboardBackColor = BackgroundColor;
            DashboardCardBackColor = CardBackColor;
            DashboardCardHoverBackColor = C(16, 16, 16);
            DashboardTitleForeColor = softGreen;
            DashboardTitleBackColor = BackgroundColor;
            DashboardTitleStyle = TitleSmall;
            DashboardSubTitleForeColor = dimGreen;
            DashboardSubTitleBackColor = BackgroundColor;
            DashboardSubTitleStyle = BodySmall;
            DashboardGradiantStartColor = BackgroundColor;
            DashboardGradiantMiddleColor = BackgroundColor;
            DashboardGradiantEndColor = BackgroundColor;
            DashboardGradiantDirection = LinearGradientMode.Vertical;

            StatsTitleFont = TitleSmall;
            StatsSelectedFont = BodySmall;
            StatsUnSelectedFont = BodySmall;
            StatsCardBackColor = CardBackColor;
            StatsCardForeColor = softGreen;
            StatsCardBorderColor = lineGreen;
            StatsCardTitleForeColor = softGreen;
            StatsCardTitleBackColor = CardBackColor;
            StatsCardTitleStyle = TitleSmall;
            StatsCardSubTitleForeColor = dimGreen;
            StatsCardSubTitleBackColor = CardBackColor;
            StatsCardSubStyleStyle = BodySmall;
            StatsCardValueForeColor = neonGreen;
            StatsCardValueBackColor = C(16, 32, 20);
            StatsCardValueBorderColor = lineGreen;
            StatsCardValueHoverForeColor = neonGreen;
            StatsCardValueHoverBackColor = C(18, 36, 22);
            StatsCardValueHoverBorderColor = lineGreen;
            StatsCardValueStyle = TitleSmall;
            StatsCardInfoForeColor = dimGreen;
            StatsCardInfoBackColor = CardBackColor;
            StatsCardInfoBorderColor = lineGreen;
            StatsCardInfoStyle = BodySmall;
            StatsCardTrendForeColor = softGreen;
            StatsCardTrendBackColor = C(16, 32, 20);
            StatsCardTrendBorderColor = lineGreen;
            StatsCardTrendStyle = BodySmall;

            TaskCardTitleFont = TitleSmall;
            TaskCardSelectedFont = BodySmall;
            TaskCardUnSelectedFont = BodySmall;
            TaskCardBackColor = CardBackColor;
            TaskCardForeColor = softGreen;
            TaskCardBorderColor = lineGreen;
            TaskCardTitleForeColor = softGreen;
            TaskCardTitleBackColor = CardBackColor;
            TaskCardTitleStyle = TitleSmall;
            TaskCardSubTitleForeColor = dimGreen;
            TaskCardSubTitleBackColor = CardBackColor;
            TaskCardSubStyleStyle = BodySmall;
            TaskCardMetricTextForeColor = dimGreen;
            TaskCardMetricTextBackColor = C(14, 14, 14);
            TaskCardMetricTextBorderColor = lineGreen;
            TaskCardMetricTextHoverForeColor = neonGreen;
            TaskCardMetricTextHoverBackColor = C(16, 32, 20);
            TaskCardMetricTextHoverBorderColor = lineGreen;
            TaskCardMetricTextStyle = BodySmall;
            TaskCardProgressValueForeColor = neonGreen;
            TaskCardProgressValueBackColor = C(16, 32, 20);
            TaskCardProgressValueBorderColor = lineGreen;
            TaskCardProgressValueStyle = LabelMedium;

            // --- Dialogs (monochrome)
            DialogBackColor = BackgroundColor;
            DialogForeColor = softGreen;
            DialogYesButtonFont = ButtonStyle;
            DialogNoButtonFont = ButtonStyle;
            DialogOkButtonFont = ButtonStyle;
            DialogCancelButtonFont = ButtonStyle;
            DialogWarningButtonFont = ButtonStyle;
            DialogErrorButtonFont = ButtonStyle;
            DialogInformationButtonFont = ButtonStyle;
            DialogQuestionButtonFont = ButtonStyle;
            DialogHelpButtonFont = ButtonStyle;
            DialogCloseButtonFont = ButtonStyle;
            DialogYesButtonHoverFont = ButtonStyle;
            DialogNoButtonHoverFont = ButtonStyle;
            DialogOkButtonHoverFont = ButtonStyle;

                        DialogYesButtonBackColor = PrimaryColor;
            DialogYesButtonForeColor = OnPrimaryColor;
            DialogYesButtonHoverBackColor = ThemeUtil.Darken(PrimaryColor, 0.08);
            DialogYesButtonHoverForeColor = OnPrimaryColor;
            DialogYesButtonHoverBorderColor = ActiveBorderColor;

            DialogCancelButtonBackColor = SecondaryColor;
            DialogCancelButtonForeColor = ForeColor;
            DialogCancelButtonHoverBackColor = ThemeUtil.Darken(SurfaceColor, 0.06);
            DialogCancelButtonHoverForeColor = ForeColor;
            DialogCancelButtonHoverBorderColor = BorderColor;

            DialogCloseButtonBackColor = SecondaryColor;
            DialogCloseButtonForeColor = ForeColor;
            DialogCloseButtonHoverBackColor = ThemeUtil.Darken(SurfaceColor, 0.06);
            DialogCloseButtonHoverForeColor = ForeColor;
            DialogCloseButtonHoverBorderColor = BorderColor;

            DialogHelpButtonBackColor = SecondaryColor;
            DialogNoButtonBackColor = SecondaryColor;
            DialogNoButtonForeColor = ForeColor;
            DialogNoButtonHoverBackColor = ThemeUtil.Darken(SurfaceColor, 0.06);
            DialogNoButtonHoverForeColor = ForeColor;
            DialogNoButtonHoverBorderColor = BorderColor;

            DialogOkButtonBackColor = PrimaryColor;
            DialogOkButtonForeColor = OnPrimaryColor;
            DialogOkButtonHoverBackColor = ThemeUtil.Darken(PrimaryColor, 0.08);
            DialogOkButtonHoverForeColor = OnPrimaryColor;
            DialogOkButtonHoverBorderColor = ActiveBorderColor;

            DialogWarningButtonBackColor = WarningColor;
            DialogWarningButtonForeColor = Color.White;
            DialogWarningButtonHoverBackColor = ThemeUtil.Darken(WarningColor, 0.08);
            DialogWarningButtonHoverForeColor = Color.White;
            DialogWarningButtonHoverBorderColor = ActiveBorderColor;

            DialogErrorButtonBackColor = ErrorColor;
            DialogErrorButtonForeColor = Color.White;
            DialogErrorButtonHoverBackColor = ThemeUtil.Darken(ErrorColor, 0.08);
            DialogErrorButtonHoverForeColor = Color.White;
            DialogErrorButtonHoverBorderColor = ActiveBorderColor;

            DialogInformationButtonBackColor = PrimaryColor;
            DialogInformationButtonForeColor = OnPrimaryColor;
            DialogInformationButtonHoverBackColor = ThemeUtil.Darken(PrimaryColor, 0.08);
            DialogInformationButtonHoverForeColor = OnPrimaryColor;
            DialogInformationButtonHoverBorderColor = ActiveBorderColor;

            DialogQuestionButtonBackColor = SecondaryColor;
            DialogQuestionButtonForeColor = OnPrimaryColor;
            DialogQuestionButtonHoverBackColor = ThemeUtil.Darken(SecondaryColor, 0.08);
            DialogQuestionButtonHoverForeColor = OnPrimaryColor;
            DialogQuestionButtonHoverBorderColor = ActiveBorderColor;

            // --- Tooltips / Markdown
            ToolTipBackColor = C(0, 0, 0);
            ToolTipForeColor = neonGreen;
            ToolTipBorderColor = lineGreen;
            ToolTipShadowColor = C(0, 0, 0);
            ToolTipShadowOpacity = C(0, 0, 0);
            ToolTipTextColor = neonGreen;
            ToolTipLinkColor = softGreen;
            ToolTipLinkHoverColor = neonGreen;
            ToolTipLinkVisitedColor = C(0, 200, 120);

            Heading1 = TitleLarge;
            Heading2 = TitleMedium;
            Heading3 = TitleSmall;
            Heading4 = LabelLarge;
            Heading5 = LabelMedium;
            Heading6 = LabelSmall;
            Paragraph = BodyMedium;
            Blockquote = BodyMedium;
            BlockquoteBorderWidth = 2f;
            BlockquotePadding = 8f;
            InlineCode = BodyMedium;
            InlineCodePadding = 2f;
            CodeBlock = BodyMedium;
            CodeBlockBorderWidth = 1f;
            CodeBlockPadding = 8f;
            BlockquoteBorderColor = lineGreen;
            InlineCodeBackgroundColor = C(12, 24, 16);
            CodeBlockBackgroundColor = C(10, 18, 12);
            CodeBlockBorderColor = lineGreen;  // Consistent green border

            UnorderedList = BodyMedium;
            OrderedList = BodyMedium;
            ListItemSpacing = 4f;
            ListIndentation = 16f;
            Link = LinkStyle;
            LinkIsUnderline = false;
            SmallText = LabelSmall;
            StrongText = TS(8, FontStyle.Bold, 700, softGreen, mono: true);
            EmphasisText = TS(8, FontStyle.Italic, 400, softGreen, mono: true);

            // --- Tabs
            TabFont = BodySmall;
            TabHoverFont = BodySmall;
            TabSelectedFont = BodySmall;
            TabBackColor = CardBackColor;
            TabForeColor = dimGreen;
            ActiveTabBackColor = BackgroundColor;
            ActiveTabForeColor = softGreen;
            InactiveTabBackColor = CardBackColor;
            InactiveTabForeColor = dimGreen;
            TabBorderColor = lineGreen;
            TabHoverBackColor = C(16, 32, 20);
            TabHoverForeColor = neonGreen;
            TabSelectedBackColor = C(16, 32, 20);
            TabSelectedForeColor = neonGreen;
            TabSelectedBorderColor = lineGreen;
            TabHoverBorderColor = lineGreen;

            // --- Trees
            TreeTitleFont = TitleSmall;
            TreeNodeSelectedFont = BodySmall;
            TreeNodeUnSelectedFont = BodySmall;
            TreeBackColor = CardBackColor;
            TreeForeColor = softGreen;
            TreeBorderColor = lineGreen;
            TreeNodeForeColor = softGreen;
            TreeNodeHoverForeColor = neonGreen;
            TreeNodeHoverBackColor = C(16, 32, 20);
            TreeNodeSelectedForeColor = neonGreen;
            TreeNodeSelectedBackColor = C(16, 32, 20);
            TreeNodeCheckedBoxForeColor = neonGreen;
            TreeNodeCheckedBoxBackColor = C(16, 32, 20);

            // --- Login/Company
            LoginPopoverBackgroundColor = CardBackColor;
            LoginTitleColor = softGreen;
            LoginSubtitleColor = dimGreen;
            LoginDescriptionColor = dimGreen;
            LoginLinkColor = LinkColor;
            LoginButtonBackgroundColor = C(16, 32, 20);  // Dark green background
            LoginButtonTextColor = neonGreen;  // Bright green text
            LoginDropdownBackgroundColor = CardBackColor;
            LoginDropdownTextColor = softGreen;
            LoginLogoBackgroundColor = CardBackColor;
            LoginTitleFont = TitleSmall;
            LoginSubtitleFont = BodySmall;
            LoginDescriptionFont = BodySmall;
            LoginLinkFont = BodySmall;
            LoginButtonFont = ButtonStyle;

            CompanyPopoverBackgroundColor = CardBackColor;
            CompanyTitleColor = softGreen;
            CompanySubtitleColor = dimGreen;
            CompanyDescriptionColor = dimGreen;
            CompanyLinkColor = LinkColor;
            CompanyButtonBackgroundColor = C(16, 32, 20);  // Dark green background
            CompanyButtonTextColor = neonGreen;  // Bright green text
            CompanyDropdownBackgroundColor = CardBackColor;
            CompanyDropdownTextColor = softGreen;
            CompanyLogoBackgroundColor = CardBackColor;
            CompanyTitleFont = TitleSmall;
            CompanySubTitleFont = BodySmall;
            CompanyDescriptionFont = BodySmall;
            CompanyLinkFont = BodySmall;
            CompanyButtonFont = ButtonStyle;

            // --- Status & Gradients
            StatusBarBackColor = StatusBarBackColor; // already set
            GradientStartColor = BackgroundColor;
            GradientEndColor = SurfaceColor;
            GradientDirection = LinearGradientMode.Vertical;

            // Fill any unset Typography/Colors to safe values (no transparency)
            FillTypographyDefaultsByConvention();
            FillColorDefaultsByConvention();
            
            // Validate and auto-fix contrast issues (Radix UI needs high contrast for accessibility)
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }

        // Utility Methods (IBeepTheme)
        public TypographyStyle GetAnswerFont() => BodyMedium ?? BodyStyle ?? TS(13, FontStyle.Regular, 400, ForeColor, true);
        public TypographyStyle GetBlockHeaderFont() => TitleSmall ?? TitleStyle ?? TS(18, FontStyle.Bold, 600, ForeColor, true);
        public TypographyStyle GetBlockTextFont() => BodyMedium ?? BodyStyle ?? TS(12, FontStyle.Regular, 400, ForeColor, true);
        public TypographyStyle GetButtonFont() => ButtonStyle ?? ButtonFont ?? TS(12, FontStyle.Bold, 600, OnPrimaryColor, true);
        public TypographyStyle GetCaptionFont() => CaptionStyle ?? LabelSmall ?? TS(11, FontStyle.Regular, 400, C(120, 200, 160), true);
        public TypographyStyle GetQuestionFont() => TitleSmall ?? TitleStyle ?? TS(16, FontStyle.Bold, 600, ForeColor, true);

        public void ReplaceTransparentColors(Color fallbackColor)
        {
            foreach (var p in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(pp => pp.CanRead && pp.CanWrite && pp.PropertyType == typeof(Color)))
            {
                var val = (Color)p.GetValue(this)!;
                if (val.A == 0) p.SetValue(this, fallbackColor);
            }
        }

        // Helpers
        private static Color C(int r, int g, int b) => Color.FromArgb(255, r, g, b);

        private static Color Blend(Color a, Color b, double t)
        {
            t = Math.Clamp(t, 0, 1);
            int A(byte x, byte y) => (int)Math.Round(x + (y - x) * t);
            return Color.FromArgb(255, A(a.R, b.R), A(a.G, b.G), A(a.B, b.B));
        }

        private static Color Darken(Color c, double by) 
            => ColorAccessibilityHelper.DarkenColor(c, (float)Math.Clamp(by, 0, 1));

        private TypographyStyle TS(float size, FontStyle style, int weight, Color color, bool mono = false)
            => new TypographyStyle
            {
                FontFamily = mono ? PreferMonospace() : (FontName ?? PreferSansSerif()),
                FontSize = size,
                LineHeight = 1.5f,
                LetterSpacing = 0f,
                FontWeight = FontWeight.Normal,
                FontStyle = style,
                TextColor = color,
                IsUnderlined = false,
                IsStrikeout = false
            };

        private static string PreferMonospace()
        {
            // Prefer Consolas; fallback to Courier New; then Segoe UI
            try { return "Consolas"; } catch { }
            return "Courier New";
        }

        private static string PreferSansSerif()
        {
            // Prefer Inter; fallback to Segoe UI; then Arial
            try { return "Inter"; } catch { }
            try { return "Segoe UI"; } catch { }
            return "Arial";
        }

        private void FillTypographyDefaultsByConvention()
        {
            var typoProps = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType.Name == nameof(TypographyStyle));

            foreach (var p in typoProps)
            {
                if (p.GetValue(this) == null)
                    p.SetValue(this, BodyMedium ?? TS(12, FontStyle.Regular, 400, ForeColor, false));
            }
        }

        private void FillColorDefaultsByConvention()
        {
            var colorProps = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(Color));

            foreach (var p in colorProps)
            {
                var val = (Color)p.GetValue(this)!;
                if (val.A != 0) continue; // already set

                var name = p.Name;

                if (name.Contains("Error", StringComparison.OrdinalIgnoreCase))
                    p.SetValue(this, ErrorColor);
                else if (name.Contains("Warning", StringComparison.OrdinalIgnoreCase))
                    p.SetValue(this, WarningColor);
                else if (name.Contains("Success", StringComparison.OrdinalIgnoreCase))
                    p.SetValue(this, SuccessColor);
                else if (name.Contains("Hover", StringComparison.OrdinalIgnoreCase) && name.EndsWith("BackColor"))
                    p.SetValue(this, C(245, 245, 245));
                else if (name.Contains("Hover", StringComparison.OrdinalIgnoreCase) && name.EndsWith("ForeColor"))
                    p.SetValue(this, ForeColor);
                else if (name.Contains("Selected", StringComparison.OrdinalIgnoreCase) && name.EndsWith("BackColor"))
                    p.SetValue(this, AccentColor);
                else if (name.Contains("Selected", StringComparison.OrdinalIgnoreCase) && name.EndsWith("ForeColor"))
                    p.SetValue(this, C(255, 255, 255));
                else if (name.EndsWith("BorderColor"))
                    p.SetValue(this, BorderColor);
                else if (name.EndsWith("BackColor"))
                    p.SetValue(this, SurfaceColor);
                else if (name.EndsWith("ForeColor"))
                    p.SetValue(this, ForeColor);
                else
                    p.SetValue(this, SurfaceColor);
            }
        }
    }
}



