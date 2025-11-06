using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    /// <summary>
    /// Modern theme:
    /// - Clean white background
    /// - Black text with blue accents
    /// - High contrast, no transparency
    /// - Sans-serif typography (Segoe UI)
    /// - Sharp borders & modern appearance
    /// </summary>
    public class ModernTheme : IBeepTheme
    {
        // =========================
        // Identity
        // =========================
        public string ThemeName { get; } = "ModernTheme";
        public string ThemeGuid { get; set; } = Guid.NewGuid().ToString();

        // =========================
        // AppBar / Caption
        // =========================
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

        // =========================
        // Constructor
        // =========================
        // =========================
        // Constructor
        // =========================
        public ModernTheme()
        {
            // Base palette tuned for a bright, modern look
            var canvas = C(248, 250, 255); // Primary background
            var lightGray = C(242, 245, 252); // Elevated surfaces
            var midGray = C(220, 227, 240); // Borders
            var darkGray = C(110, 118, 140); // Secondary text
            var white = C(255, 255, 255); // Pure white surfaces
            var black = C(38, 44, 57); // Soft charcoal text
            var blueAccent = C(92, 139, 255); // Accent blue
            var greenAccent = C(94, 204, 154); // Success/max
            var redAccent = C(236, 112, 132); // Error/close
            var subtleWhite = C(251, 252, 255); // Gradient midpoint
            var hoverBlue = C(229, 237, 255); // Hover states

            // Typography defaults: sans-serif, clean
            FontName = PreferSansSerif();
            FontSize = 12f;

            var TitleLarge = TS(24, FontStyle.Bold, 700, black);
            var TitleMedium = TS(18, FontStyle.Bold, 600, black);
            var TitleSmall = TS(16, FontStyle.Bold, 600, black);
            var LabelLarge = TS(14, FontStyle.Regular, 500, darkGray);
            var LabelMedium = TS(12, FontStyle.Regular, 500, darkGray);
            var LabelSmall = TS(11, FontStyle.Regular, 400, darkGray);
            var BodyLarge = TS(16, FontStyle.Regular, 400, black);
            var BodyMedium = TS(14, FontStyle.Regular, 400, black);
            var BodySmall = TS(12, FontStyle.Regular, 400, black);

            // Core palette
            PrimaryColor = blueAccent;
            SecondaryColor = greenAccent;
            AccentColor = blueAccent;
            BackgroundColor = canvas;
            SurfaceColor = white;
            ErrorColor = redAccent;
            WarningColor = C(255, 193, 7); // Amber
            SuccessColor = greenAccent;
            OnPrimaryColor = white;
            OnBackgroundColor = black;

            // Typography
            TitleStyle = TitleMedium;
            SubtitleStyle = BodyLarge;
            BodyStyle = BodyMedium;
            CaptionStyle = LabelSmall;
            ButtonStyle = LabelMedium;
            LinkStyle = TS(12, FontStyle.Regular, 400, blueAccent);
            OverlineStyle = LabelSmall;

            // Core colors
            ForeColor = black;
            BackColor = canvas;
            PanelBackColor = lightGray;
            PanelGradiantStartColor = subtleWhite;
            PanelGradiantEndColor = lightGray;
            PanelGradiantMiddleColor = white;
            PanelGradiantDirection = LinearGradientMode.Vertical;
            DisabledBackColor = C(244, 246, 252);
            DisabledForeColor = darkGray;
            DisabledBorderColor = midGray;
            BorderColor = midGray;
            ActiveBorderColor = blueAccent;
            InactiveBorderColor = midGray;

            // AppBar / Caption (inspired by ModernFormPainter caption)
            AppBarBackColor = lightGray;
            AppBarForeColor = black;
            AppBarButtonForeColor = black;
            AppBarButtonBackColor = white;
            AppBarTextBoxBackColor = white;
            AppBarTextBoxForeColor = black;
            AppBarLabelForeColor = black;
            AppBarLabelBackColor = lightGray;
            AppBarTitleForeColor = black;
            AppBarTitleBackColor = lightGray;
            AppBarSubTitleForeColor = darkGray;
            AppBarSubTitleBackColor = lightGray;
            AppBarCloseButtonColor = redAccent;
            AppBarMaxButtonColor = greenAccent;
            AppBarMinButtonColor = blueAccent;
            AppBarTitleStyle = TitleMedium;
            AppBarSubTitleStyle = BodySmall;
            AppBarTextStyle = BodyMedium;
            AppBarGradiantStartColor = subtleWhite;
            AppBarGradiantEndColor = lightGray;
            AppBarGradiantMiddleColor = white;
            AppBarGradiantDirection = LinearGradientMode.Vertical;

            // Badge
            BadgeBackColor = blueAccent;
            BadgeForeColor = white;
            HighlightBackColor = hoverBlue;
            BadgeFont = LabelSmall;

            // Markdown / Code
            BlockquoteBorderColor = midGray;
            InlineCodeBackgroundColor = C(245, 245, 245);
            CodeBlockBackgroundColor = C(250, 250, 250);
            CodeBlockBorderColor = midGray;

            // Buttons
            ButtonFont = ButtonStyle;
            ButtonHoverFont = ButtonStyle;
            ButtonSelectedFont = ButtonStyle;
            ButtonHoverBackColor = Darken(blueAccent, 0.1);
            ButtonHoverForeColor = white;
            ButtonHoverBorderColor = Darken(blueAccent, 0.2);
            ButtonSelectedBorderColor = blueAccent;
            ButtonSelectedBackColor = blueAccent;
            ButtonSelectedForeColor = white;
            ButtonSelectedHoverBackColor = Darken(blueAccent, 0.1);
            ButtonSelectedHoverForeColor = white;
            ButtonSelectedHoverBorderColor = Darken(blueAccent, 0.2);
            ButtonBackColor = blueAccent;
            ButtonForeColor = white;
            ButtonBorderColor = blueAccent;
            ButtonErrorBackColor = redAccent;
            ButtonErrorForeColor = white;
            ButtonErrorBorderColor = redAccent;
            ButtonPressedBackColor = Darken(blueAccent, 0.15);
            ButtonPressedForeColor = white;
            ButtonPressedBorderColor = Darken(blueAccent, 0.25);

            // Cards (subtle shadows implied)
            CardTitleFont = TitleSmall;
            CardTextForeColor = black;
            CardBackColor = white;
            CardTitleForeColor = black;
            CardSubTitleFont = BodySmall;
            CardSubTitleForeColor = darkGray;
            CardHeaderStyle = TitleSmall;
            CardparagraphStyle = BodyMedium;
            CardSubTitleStyle = BodySmall;
            CardrGradiantStartColor = white;
            CardGradiantEndColor = lightGray;
            CardGradiantMiddleColor = subtleWhite;
            CardGradiantDirection = LinearGradientMode.Vertical;

            // Calendar
            CalendarTitleFont = TitleSmall;
            CalendarTitleForColor = black;
            DaysHeaderFont = LabelMedium;
            CalendarDaysHeaderForColor = darkGray;
            SelectedDateFont = BodyMedium;
            CalendarSelectedDateBackColor = blueAccent;
            CalendarSelectedDateForColor = white;
            CalendarSelectedFont = BodyMedium;
            CalendarUnSelectedFont = BodyMedium;
            CalendarBackColor = white;
            CalendarForeColor = black;
            CalendarTodayForeColor = blueAccent;
            CalendarBorderColor = midGray;
            CalendarHoverBackColor = hoverBlue;
            CalendarHoverForeColor = black;
            HeaderFont = TitleSmall;
            MonthFont = BodyMedium;
            YearFont = BodyMedium;
            DaysFont = BodyMedium;
            DaysSelectedFont = BodyMedium;
            DateFont = BodyMedium;
            CalendarFooterColor = lightGray;
            FooterFont = BodySmall;

            // Charts
            ChartTitleFont = TitleMedium;
            ChartSubTitleFont = BodySmall;
            ChartBackColor = white;
            ChartLineColor = blueAccent;
            ChartFillColor = C(200, 220, 255);
            ChartAxisColor = midGray;
            ChartTitleColor = black;
            ChartTextColor = darkGray;
            ChartLegendBackColor = lightGray;
            ChartLegendTextColor = black;
            ChartLegendShapeColor = blueAccent;
            ChartGridLineColor = midGray;
            ChartDefaultSeriesColors = new List<Color> { blueAccent, greenAccent, redAccent, WarningColor };

            // CheckBox
            CheckBoxBackColor = white;
            CheckBoxForeColor = black;
            CheckBoxBorderColor = midGray;
            CheckBoxCheckedBackColor = blueAccent;
            CheckBoxCheckedForeColor = white;
            CheckBoxCheckedBorderColor = blueAccent;
            CheckBoxHoverBackColor = hoverBlue;
            CheckBoxHoverForeColor = black;
            CheckBoxHoverBorderColor = blueAccent;
            CheckBoxFont = BodyMedium;
            CheckBoxCheckedFont = BodyMedium;

            // ComboBox
            ComboBoxBackColor = white;
            ComboBoxForeColor = black;
            ComboBoxBorderColor = midGray;
            ComboBoxHoverBackColor = hoverBlue;
            ComboBoxHoverForeColor = black;
            ComboBoxHoverBorderColor = blueAccent;
            ComboBoxSelectedBackColor = blueAccent;
            ComboBoxSelectedForeColor = white;
            ComboBoxSelectedBorderColor = blueAccent;
            ComboBoxErrorBackColor = C(255, 230, 230);
            ComboBoxErrorForeColor = redAccent;
            ComboBoxItemFont = BodyMedium;
            ComboBoxListFont = BodyMedium;

            // Company Popover
            CompanyPopoverBackgroundColor = white;
            CompanyTitleColor = black;
            CompanyTitleFont = TitleSmall;
            CompanySubtitleColor = darkGray;
            CompanySubTitleFont = BodySmall;
            CompanyDescriptionColor = black;
            CompanyDescriptionFont = BodyMedium;
            CompanyLinkColor = blueAccent;
            CompanyLinkFont = BodyMedium;
            CompanyButtonBackgroundColor = blueAccent;
            CompanyButtonTextColor = white;
            CompanyButtonFont = ButtonStyle;
            CompanyDropdownBackgroundColor = white;
            CompanyDropdownTextColor = black;
            CompanyLogoBackgroundColor = lightGray;

            // Dashboard
            DashboardTitleFont = TitleSmall;
            DashboardSubTitleFont = BodySmall;
            DashboardBackColor = white;
            DashboardCardBackColor = white;
            DashboardCardHoverBackColor = subtleWhite;
            DashboardTitleForeColor = black;
            DashboardTitleBackColor = white;
            DashboardTitleStyle = TitleSmall;
            DashboardSubTitleForeColor = darkGray;
            DashboardSubTitleBackColor = white;
            DashboardSubTitleStyle = BodySmall;
            DashboardGradiantStartColor = subtleWhite;
            DashboardGradiantEndColor = lightGray;
            DashboardGradiantMiddleColor = white;
            DashboardGradiantDirection = LinearGradientMode.Vertical;

            // Dialogs
            DialogBackColor = white;
            DialogForeColor = black;
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

            DialogYesButtonBackColor = greenAccent;
            DialogYesButtonForeColor = white;
            DialogYesButtonHoverBackColor = Darken(greenAccent, 0.08);
            DialogYesButtonHoverForeColor = white;
            DialogYesButtonHoverBorderColor = Darken(greenAccent, 0.18);

            DialogCancelButtonBackColor = lightGray;
            DialogCancelButtonForeColor = black;
            DialogCancelButtonHoverBackColor = C(230, 230, 230);
            DialogCancelButtonHoverForeColor = black;
            DialogCancelButtonHoverBorderColor = midGray;

            DialogCloseButtonBackColor = lightGray;
            DialogCloseButtonForeColor = black;
            DialogCloseButtonHoverBackColor = C(230, 230, 230);
            DialogCloseButtonHoverForeColor = black;
            DialogCloseButtonHoverBorderColor = midGray;

            DialogHelpButtonBackColor = lightGray;
            DialogNoButtonBackColor = lightGray;
            DialogNoButtonForeColor = black;
            DialogNoButtonHoverBackColor = C(230, 230, 230);
            DialogNoButtonHoverForeColor = black;
            DialogNoButtonHoverBorderColor = midGray;

            DialogOkButtonBackColor = blueAccent;
            DialogOkButtonForeColor = white;
            DialogOkButtonHoverBackColor = Darken(blueAccent, 0.08);
            DialogOkButtonHoverForeColor = white;
            DialogOkButtonHoverBorderColor = Darken(blueAccent, 0.18);

            DialogWarningButtonBackColor = WarningColor;
            DialogWarningButtonForeColor = black;
            DialogWarningButtonHoverBackColor = Darken(WarningColor, 0.08);
            DialogWarningButtonHoverForeColor = black;
            DialogWarningButtonHoverBorderColor = Darken(WarningColor, 0.18);

            DialogErrorButtonBackColor = ErrorColor;
            DialogErrorButtonForeColor = white;
            DialogErrorButtonHoverBackColor = Darken(ErrorColor, 0.08);
            DialogErrorButtonHoverForeColor = white;
            DialogErrorButtonHoverBorderColor = Darken(ErrorColor, 0.18);

            DialogInformationButtonBackColor = blueAccent;
            DialogInformationButtonForeColor = white;
            DialogInformationButtonHoverBackColor = Darken(blueAccent, 0.08);
            DialogInformationButtonHoverForeColor = white;
            DialogInformationButtonHoverBorderColor = Darken(blueAccent, 0.18);

            DialogQuestionButtonBackColor = blueAccent;
            DialogQuestionButtonForeColor = white;
            DialogQuestionButtonHoverBackColor = Darken(blueAccent, 0.08);
            DialogQuestionButtonHoverForeColor = white;
            DialogQuestionButtonHoverBorderColor = Darken(blueAccent, 0.18);

            // Gradient
            GradientStartColor = subtleWhite;
            GradientEndColor = lightGray;
            GradientDirection = LinearGradientMode.Vertical;

            // Grid Fonts
            GridHeaderFont = LabelMedium;
            GridRowFont = BodyMedium;
            GridCellFont = BodyMedium;
            GridCellSelectedFont = BodyMedium;
            GridCellHoverFont = BodyMedium;
            GridCellErrorFont = BodyMedium;
            GridColumnFont = LabelMedium;

            // Grid Colors
            GridBackColor = white;
            GridForeColor = black;
            GridHeaderBackColor = lightGray;
            GridHeaderForeColor = black;
            GridHeaderBorderColor = midGray;
            GridHeaderHoverBackColor = subtleWhite;
            GridHeaderHoverForeColor = black;
            GridHeaderSelectedBackColor = blueAccent;
            GridHeaderSelectedForeColor = white;
            GridHeaderHoverBorderColor = blueAccent;
            GridHeaderSelectedBorderColor = blueAccent;
            GridRowHoverBackColor = hoverBlue;
            GridRowHoverForeColor = black;
            GridRowSelectedBackColor = blueAccent;
            GridRowSelectedForeColor = white;
            GridRowHoverBorderColor = blueAccent;

            // StarRating (assuming from TerminalTheme)
            StarRatingFillColor = C(255, 193, 7);
            StarRatingBorderColor = midGray;
            StarRatingHoverBackColor = subtleWhite;
            StarRatingHoverForeColor = C(255, 193, 7);
            StarRatingHoverBorderColor = midGray;
            StarRatingSelectedBackColor = subtleWhite;
            StarRatingSelectedForeColor = C(255, 193, 7);
            StarRatingSelectedBorderColor = midGray;
            StarTitleFont = TitleSmall;
            StarSubTitleFont = BodySmall;
            StarSelectedFont = BodyMedium;
            StarUnSelectedFont = BodyMedium;
            StarTitleForeColor = black;
            StarTitleBackColor = white;

            // Stats / Dashboard / Task (inspired by modern design)
            StatsTitleFont = TitleSmall;
            StatsSelectedFont = BodyMedium;
            StatsUnSelectedFont = BodyMedium;
            StatsCardBackColor = white;
            StatsCardForeColor = black;
            StatsCardBorderColor = midGray;
            StatsCardTitleForeColor = black;
            StatsCardTitleBackColor = white;
            StatsCardTitleStyle = TitleSmall;
            StatsCardSubTitleForeColor = darkGray;
            StatsCardSubTitleBackColor = white;
            StatsCardSubStyleStyle = BodySmall;
            StatsCardValueForeColor = blueAccent;
            StatsCardValueBackColor = lightGray;
            StatsCardValueBorderColor = midGray;
            StatsCardValueHoverForeColor = blueAccent;
            StatsCardValueHoverBackColor = subtleWhite;
            StatsCardValueHoverBorderColor = blueAccent;
            StatsCardValueStyle = TitleSmall;
            StatsCardInfoForeColor = darkGray;
            StatsCardInfoBackColor = white;
            StatsCardInfoBorderColor = midGray;
            StatsCardInfoStyle = BodySmall;
            StatsCardTrendForeColor = greenAccent;
            StatsCardTrendBackColor = lightGray;
            StatsCardTrendBorderColor = midGray;
            StatsCardTrendStyle = BodySmall;

            TaskCardTitleFont = TitleSmall;
            TaskCardSelectedFont = BodyMedium;
            TaskCardUnSelectedFont = BodyMedium;
            TaskCardBackColor = white;
            TaskCardForeColor = black;
            TaskCardBorderColor = midGray;
            TaskCardTitleForeColor = black;
            TaskCardTitleBackColor = white;
            TaskCardTitleStyle = TitleSmall;
            TaskCardSubTitleForeColor = darkGray;
            TaskCardSubTitleBackColor = white;
            TaskCardSubStyleStyle = BodySmall;
            TaskCardMetricTextForeColor = darkGray;
            TaskCardMetricTextBackColor = lightGray;
            TaskCardMetricTextBorderColor = midGray;
            TaskCardMetricTextHoverForeColor = blueAccent;
            TaskCardMetricTextHoverBackColor = subtleWhite;
            TaskCardMetricTextHoverBorderColor = blueAccent;
            TaskCardMetricTextStyle = BodySmall;
            TaskCardProgressValueForeColor = blueAccent;
            TaskCardProgressValueBackColor = lightGray;
            TaskCardProgressValueBorderColor = midGray;
            TaskCardProgressValueStyle = LabelMedium;

            // Tooltips / Markdown
            ToolTipBackColor = white;
            ToolTipForeColor = black;
            ToolTipBorderColor = midGray;
            ToolTipShadowColor = Color.FromArgb(96, black);
            ToolTipShadowOpacity = Color.FromArgb(32, black);
            ToolTipTextColor = black;
            ToolTipLinkColor = blueAccent;
            ToolTipLinkHoverColor = Darken(blueAccent, 0.1);
            ToolTipLinkVisitedColor = C(128, 0, 128);

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
            BlockquoteBorderColor = midGray;
            InlineCodeBackgroundColor = C(245, 245, 245);
            CodeBlockBackgroundColor = C(250, 250, 250);
            CodeBlockBorderColor = midGray;

            UnorderedList = BodyMedium;
            OrderedList = BodyMedium;
            ListItemSpacing = 4f;
            ListIndentation = 16f;
            Link = LinkStyle;
            LinkIsUnderline = true;
            SmallText = LabelSmall;
            StrongText = TS(12, FontStyle.Bold, 700, black);
            EmphasisText = TS(12, FontStyle.Italic, 400, black);

            // Tabs
            TabFont = BodyMedium;
            TabHoverFont = BodyMedium;
            TabSelectedFont = BodyMedium;
            TabBackColor = lightGray;
            TabForeColor = darkGray;
            ActiveTabBackColor = white;
            ActiveTabForeColor = black;
            InactiveTabBackColor = lightGray;
            InactiveTabForeColor = darkGray;
            TabBorderColor = midGray;
            TabHoverBackColor = subtleWhite;
            TabHoverForeColor = black;
            TabSelectedBackColor = white;
            TabSelectedForeColor = black;
            TabSelectedBorderColor = blueAccent;
            TabHoverBorderColor = blueAccent;

            // Trees
            TreeTitleFont = TitleSmall;
            TreeNodeSelectedFont = BodyMedium;
            TreeNodeUnSelectedFont = BodyMedium;
            TreeBackColor = white;
            TreeForeColor = black;
            TreeBorderColor = midGray;
            TreeNodeForeColor = black;
            TreeNodeHoverForeColor = black;
            TreeNodeHoverBackColor = hoverBlue;
            TreeNodeSelectedForeColor = white;
            TreeNodeSelectedBackColor = blueAccent;
            TreeNodeCheckedBoxForeColor = blueAccent;
            TreeNodeCheckedBoxBackColor = white;

            // Login/Company
            LoginPopoverBackgroundColor = white;
            LoginTitleColor = black;
            LoginSubtitleColor = darkGray;
            LoginDescriptionColor = darkGray;
            LoginLinkColor = blueAccent;
            LoginButtonBackgroundColor = blueAccent;
            LoginButtonTextColor = white;
            LoginDropdownBackgroundColor = white;
            LoginDropdownTextColor = black;
            LoginLogoBackgroundColor = lightGray;
            LoginTitleFont = TitleSmall;
            LoginSubtitleFont = BodySmall;
            LoginDescriptionFont = BodySmall;
            LoginLinkFont = BodyMedium;
            LoginButtonFont = ButtonStyle;

            CompanyPopoverBackgroundColor = white;
            CompanyTitleColor = black;
            CompanyTitleFont = TitleSmall;
            CompanySubtitleColor = darkGray;
            CompanySubTitleFont = BodySmall;
            CompanyDescriptionColor = darkGray;
            CompanyDescriptionFont = BodySmall;
            CompanyLinkColor = blueAccent;
            CompanyLinkFont = BodyMedium;
            CompanyButtonBackgroundColor = blueAccent;
            CompanyButtonTextColor = white;
            CompanyButtonFont = ButtonStyle;
            CompanyDropdownBackgroundColor = white;
            CompanyDropdownTextColor = black;
            CompanyLogoBackgroundColor = lightGray;

            // Menu (consistent with modern theme - light, clean appearance)
            MenuTitleFont = TitleSmall;
            MenuItemSelectedFont = BodyMedium;
            MenuItemUnSelectedFont = BodyMedium;
            MenuBackColor = white; // Solid white background (no transparency)
            MenuForeColor = black;
            MenuBorderColor = midGray;
            // Main menu items (top-level menu bar items)
            MenuMainItemForeColor = black;
            MenuMainItemHoverForeColor = black;
            MenuMainItemHoverBackColor = subtleWhite;
            MenuMainItemSelectedForeColor = white;
            MenuMainItemSelectedBackColor = blueAccent;
            // Menu items (dropdown items)
            MenuItemForeColor = black;
            MenuItemHoverForeColor = black;
            MenuItemHoverBackColor = hoverBlue; // Light blue hover
            MenuItemSelectedForeColor = white;
            MenuItemSelectedBackColor = blueAccent;
            // Menu gradients
            MenuGradiantStartColor = subtleWhite;
            MenuGradiantEndColor = lightGray;
            MenuGradiantMiddleColor = white;
            MenuGradiantDirection = LinearGradientMode.Vertical;

            // Side Menu (similar to Menu but for sidebars)
            SideMenuTitleFont = TitleSmall;
            SideMenuSubTitleFont = BodySmall;
            SideMenuTextFont = BodyMedium;
            SideMenuBackColor = lightGray;
            SideMenuForeColor = black;
            SideMenuBorderColor = midGray;
            SideMenuHoverBackColor = subtleWhite;
            SideMenuHoverForeColor = black;
            SideMenuSelectedBackColor = blueAccent;
            SideMenuSelectedForeColor = white;
            SideMenuTitleTextColor = black;
            SideMenuTitleBackColor = lightGray;
            SideMenuTitleStyle = TitleSmall;
            SideMenuSubTitleTextColor = darkGray;
            SideMenuSubTitleBackColor = lightGray;
            SideMenuSubTitleStyle = BodySmall;
            SideMenuGradiantStartColor = subtleWhite;
            SideMenuGradiantEndColor = lightGray;
            SideMenuGradiantMiddleColor = white;
            SideMenuGradiantDirection = LinearGradientMode.Vertical;

            // Navigation (consistent with Menu)
            NavigationTitleFont = TitleSmall;
            NavigationSelectedFont = BodyMedium;
            NavigationUnSelectedFont = BodyMedium;
            NavigationBackColor = lightGray;
            NavigationForeColor = black;
            NavigationHoverBackColor = subtleWhite;
            NavigationHoverForeColor = black;
            NavigationSelectedBackColor = blueAccent;
            NavigationSelectedForeColor = white;

            // Labels (consistent with modern theme)
            LabelFont = BodyMedium;
            SubLabelFont = BodySmall;
            LabelBackColor = white; // Solid white background (no transparency)
            LabelForeColor = black;
            LabelBorderColor = midGray;
            LabelHoverBackColor = subtleWhite;
            LabelHoverForeColor = black;
            LabelHoverBorderColor = blueAccent;
            LabelSelectedBackColor = blueAccent;
            LabelSelectedForeColor = white;
            LabelSelectedBorderColor = blueAccent;
            LabelDisabledBackColor = DisabledBackColor;
            LabelDisabledForeColor = DisabledForeColor;
            LabelDisabledBorderColor = DisabledBorderColor;
            SubLabelForColor = darkGray;
            SubLabelBackColor = white; // Solid white background (no transparency)
            SubLabelHoverBackColor = subtleWhite;
            SubLabelHoverForeColor = black;

            // TextBox (consistent with ComboBox)
            TextBoxFont = BodyMedium;
            TextBoxHoverFont = BodyMedium;
            TextBoxSelectedFont = BodyMedium;
            TextBoxBackColor = white;
            TextBoxForeColor = black;
            TextBoxBorderColor = midGray;
            TextBoxHoverBackColor = white;
            TextBoxHoverForeColor = black;
            TextBoxHoverBorderColor = blueAccent;
            TextBoxSelectedBackColor = white;
            TextBoxSelectedForeColor = black;
            TextBoxSelectedBorderColor = blueAccent;
            TextBoxPlaceholderColor = darkGray;
            TextBoxErrorBackColor = C(255, 230, 230);
            TextBoxErrorForeColor = redAccent;
            TextBoxErrorBorderColor = redAccent;
            TextBoxErrorTextColor = redAccent;
            TextBoxErrorPlaceholderColor = redAccent;
            TextBoxErrorTextBoxColor = C(255, 230, 230);
            TextBoxErrorTextBoxBorderColor = redAccent;
            TextBoxErrorTextBoxHoverColor = C(255, 240, 240);

            // List (consistent with Grid rows)
            ListTitleFont = TitleSmall;
            ListSelectedFont = BodyMedium;
            ListUnSelectedFont = BodyMedium;
            ListBackColor = white;
            ListForeColor = black;
            ListBorderColor = midGray;
            ListItemForeColor = black;
            ListItemHoverForeColor = black;
            ListItemHoverBackColor = hoverBlue;
            ListItemHoverBorderColor = blueAccent;
            ListItemSelectedForeColor = white;
            ListItemSelectedBackColor = blueAccent;
            ListItemSelectedBorderColor = blueAccent;
            ListItemBorderColor = white; // Solid white instead of transparent

            // ScrollList (similar to List)
            ScrollListTitleFont = TitleSmall;
            ScrollListSelectedFont = BodyMedium;
            ScrollListUnSelectedFont = BodyMedium;
            ScrollListIItemFont = BodyMedium;
            ScrollListItemSelectedFont = BodyMedium;
            ScrollListBackColor = white;
            ScrollListForeColor = black;
            ScrollListBorderColor = midGray;
            ScrollListItemForeColor = black;
            ScrollListItemHoverForeColor = black;
            ScrollListItemHoverBackColor = hoverBlue;
            ScrollListItemSelectedForeColor = white;
            ScrollListItemSelectedBackColor = blueAccent;
            ScrollListItemSelectedBorderColor = blueAccent;
            ScrollListItemBorderColor = white; // Solid white instead of transparent

            // Links
            LinkColor = blueAccent;
            VisitedLinkColor = C(128, 0, 128); // Purple
            HoverLinkColor = Darken(blueAccent, 0.1);
            LinkHoverColor = Darken(blueAccent, 0.1);

            // Misc / Utility
            FontFamily = FontName;
            FontStyleRegular = FontStyle.Regular;
            FontStyleBold = FontStyle.Bold;
            FontStyleItalic = FontStyle.Italic;
            PrimaryTextColor = black;
            SecondaryTextColor = darkGray;
            AccentTextColor = blueAccent;
            PaddingSmall = 4;
            PaddingMedium = 8;
            PaddingLarge = 16;
            BorderRadius = 8;
            BorderSize = 1;
            IconSet = "Fluent";
            ApplyThemeToIcons = true;
            FocusIndicatorColor = blueAccent;
            ShadowColor = Color.FromArgb(40, black);
            ShadowOpacity = 0.18f;
            AnimationDurationShort = 0.16;
            AnimationDurationMedium = 0.28;
            AnimationDurationLong = 0.4;
            AnimationEasingFunction = "CubicBezier(0.33, 1, 0.68, 1)";
            HighContrastMode = false;
            IsDarkTheme = false;

            // Status & Gradients
            StatusBarBackColor = lightGray;
            StatusBarForeColor = black;
            StatusBarBorderColor = midGray;
            StatusBarHoverBackColor = hoverBlue;
            StatusBarHoverForeColor = black;
            StatusBarHoverBorderColor = blueAccent;
            GradientStartColor = subtleWhite;
            GradientEndColor = lightGray;
            GradientDirection = LinearGradientMode.Vertical;

            // Fill any unset Typography/Colors to safe values (no transparency)
            FillTypographyDefaultsByConvention();
            FillColorDefaultsByConvention();
        }

        // =========================
        // Utility Methods (IBeepTheme)
        // =========================
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

        // =========================
        // Helpers
        // =========================
        private static Color C(int r, int g, int b) => Color.FromArgb(255, r, g, b);

        private static Color Blend(Color a, Color b, double t)
        {
            t = Math.Clamp(t, 0, 1);
            int A(byte x, byte y) => (int)Math.Round(x + (y - x) * t);
            return Color.FromArgb(255, A(a.R, b.R), A(a.G, b.G), A(a.B, b.B));
        }

        private static Color Darken(Color c, double by) => Blend(c, Color.Black, Math.Clamp(by, 0, 1));

        private TypographyStyle TS(float size, FontStyle style, int weight, Color color, bool mono = false)
            => new TypographyStyle
            {
                FontFamily = mono ? PreferMonospace() : (FontName ?? "Segoe UI"),
                FontSize = size,
                LineHeight = 1.35f,
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
            // Prefer Segoe UI; fallback to Arial
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
                    p.SetValue(this, BodyMedium ?? TS(12, FontStyle.Regular, 400, ForeColor, true));
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
                    p.SetValue(this, C(229, 237, 255));
                else if (name.Contains("Hover", StringComparison.OrdinalIgnoreCase) && name.EndsWith("ForeColor"))
                    p.SetValue(this, C(38, 44, 57));
                else if (name.Contains("Selected", StringComparison.OrdinalIgnoreCase) && name.EndsWith("BackColor"))
                    p.SetValue(this, PrimaryColor);
                else if (name.Contains("Selected", StringComparison.OrdinalIgnoreCase) && name.EndsWith("ForeColor"))
                    p.SetValue(this, OnPrimaryColor);
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
