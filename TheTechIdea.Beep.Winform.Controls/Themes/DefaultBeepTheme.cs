using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    internal class DefaultBeepTheme : IBeepTheme
    {
        public string ThemeName { get; set; } = "DefaultBeepTheme";

        public string ThemeGuid { get; set; }
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
        public Color BadgeBackColor { get; set; }
        public Color BadgeForeColor { get; set; }
        public Color HighlightBackColor { get; set; }
        public TypographyStyle BadgeFont { get; set; }
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
        public Color BlockquoteBorderColor { get; set; }
        public Color InlineCodeBackgroundColor { get; set; }
        public Color CodeBlockBackgroundColor { get; set; }
        public Color CodeBlockBorderColor { get; set; }
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
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public TypographyStyle TitleStyle { get; set; }
        public TypographyStyle SubtitleStyle { get; set; }
        public TypographyStyle BodyStyle { get; set; }
        public TypographyStyle CaptionStyle { get; set; }
        public TypographyStyle ButtonStyle { get; set; }
        public TypographyStyle LinkStyle { get; set; }
        public TypographyStyle OverlineStyle { get; set; }
        public Color GradientStartColor { get; set; }
        public Color GradientEndColor { get; set; }
        public LinearGradientMode GradientDirection { get; set; }
        public TypographyStyle GridHeaderFont { get; set; }
        public TypographyStyle GridRowFont { get; set; }
        public TypographyStyle GridCellFont { get; set; }
        public TypographyStyle GridCellSelectedFont { get; set; }
        public TypographyStyle GridCellHoverFont { get; set; }
        public TypographyStyle GridCellErrorFont { get; set; }
        public TypographyStyle GridColumnFont { get; set; }
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
        public Color LinkColor { get; set; }
        public Color VisitedLinkColor { get; set; }
        public Color HoverLinkColor { get; set; }
        public Color LinkHoverColor { get; set; }
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
        public TypographyStyle NavigationTitleFont { get; set; }
        public TypographyStyle NavigationSelectedFont { get; set; }
        public TypographyStyle NavigationUnSelectedFont { get; set; }
        public Color NavigationBackColor { get; set; }
        public Color NavigationForeColor { get; set; }
        public Color NavigationHoverBackColor { get; set; }
        public Color NavigationHoverForeColor { get; set; }
        public Color NavigationSelectedBackColor { get; set; }
        public Color NavigationSelectedForeColor { get; set; }
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
        public Color ScrollBarBackColor { get; set; }
        public Color ScrollBarThumbColor { get; set; }
        public Color ScrollBarTrackColor { get; set; }
        public Color ScrollBarHoverThumbColor { get; set; }
        public Color ScrollBarHoverTrackColor { get; set; }
        public Color ScrollBarActiveThumbColor { get; set; }
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
        public Color StatusBarBackColor { get; set; }
        public Color StatusBarForeColor { get; set; }
        public Color StatusBarBorderColor { get; set; }
        public Color StatusBarHoverBackColor { get; set; }
        public Color StatusBarHoverForeColor { get; set; }
        public Color StatusBarHoverBorderColor { get; set; }
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
        public Color ToolTipBackColor { get; set; }
        public Color ToolTipForeColor { get; set; }
        public Color ToolTipBorderColor { get; set; }
        public Color ToolTipShadowColor { get; set; }
        public Color ToolTipShadowOpacity { get; set; }
        public Color ToolTipTextColor { get; set; }
        public Color ToolTipLinkColor { get; set; }
        public Color ToolTipLinkHoverColor { get; set; }
        public Color ToolTipLinkVisitedColor { get; set; }
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

            // ─────────────────────────────────────────────────────────────────────────
            // ADD THIS CONSTRUCTOR
            // ─────────────────────────────────────────────────────────────────────────
            public DefaultBeepTheme()
            {
                ThemeName = "DefaultBeepTheme";
                ThemeGuid = Guid.NewGuid().ToString();

                // ── Base palette (modern light)
                PrimaryColor = C(37, 99, 235);  // blue-600
                SecondaryColor = C(14, 165, 233);  // cyan-500
                AccentColor = C(245, 158, 11);  // amber-500
                BackgroundColor = C(255, 255, 255);  // white
                SurfaceColor = C(246, 248, 252);  // subtle surface
                ForeColor = C(17, 24, 39);  // slate-900
                OnPrimaryColor = C(255, 255, 255);  // white
                OnBackgroundColor = ForeColor;

                ErrorColor = C(211, 47, 47);
                WarningColor = C(245, 124, 0);
                SuccessColor = C(46, 125, 50);

                BorderColor = C(100, 116, 139);  // slate-500 - more visible border
                ActiveBorderColor = C(59, 130, 246);  // blue-500
                InactiveBorderColor = C(148, 163, 184);  // slate-400 - lighter but still visible

                DisabledBackColor = C(243, 244, 246);
                DisabledForeColor = C(156, 163, 175);
                DisabledBorderColor = InactiveBorderColor;

                // ── AppBar
                // For caption bar - use colors that work with BOTH the theme's AppBarBackColor
                // AND the style-specific caption backgrounds (which may be light or dark)
                AppBarBackColor = PrimaryColor;
                AppBarForeColor = OnPrimaryColor;
                AppBarButtonBackColor = Darken(PrimaryColor, 0.06);
                AppBarButtonForeColor = OnPrimaryColor;
                AppBarTextBoxBackColor = C(255, 255, 255);
                AppBarTextBoxForeColor = C(17, 24, 39);
                AppBarLabelBackColor = AppBarBackColor;
                AppBarLabelForeColor = OnPrimaryColor;
                AppBarTitleBackColor = AppBarBackColor;
                // AppBarTitleForeColor: Dark text that works on light caption bars (Glass, Material, etc.)
                // FormStyles override caption background colors, so we need text that works on light backgrounds
                AppBarTitleForeColor = C(17, 24, 39);  // Dark slate-900 for light backgrounds
                AppBarSubTitleBackColor = AppBarBackColor;
                AppBarSubTitleForeColor = Blend(OnPrimaryColor, C(224, 231, 255), 0.65);
                AppBarCloseButtonColor = C(248, 113, 113);
                AppBarMaxButtonColor = C(134, 239, 172);
                AppBarMinButtonColor = C(253, 224, 71);
                AppBarGradiantStartColor = AppBarBackColor;
                AppBarGradiantMiddleColor = AppBarBackColor;
                AppBarGradiantEndColor = Darken(AppBarBackColor, 0.03);
                AppBarGradiantDirection = LinearGradientMode.Vertical;

                // ── General surface/background
                BackColor = BackgroundColor;
                PanelBackColor = C(250, 251, 253);            // distinct from BackColor
                PanelGradiantStartColor = PanelBackColor;
                PanelGradiantMiddleColor = PanelBackColor;
                PanelGradiantEndColor = PanelBackColor;
                PanelGradiantDirection = LinearGradientMode.Vertical;
                HighlightBackColor = Blend(PrimaryColor, Color.White, 0.90);  // subtle primary tint

                // ── Typography (Segoe UI baseline)
                FontName = "Segoe UI";
                FontFamily = FontName;
                FontSize = 12f;

                TitleStyle = TS(24, FontStyle.Bold, 700, ForeColor);
                SubtitleStyle = TS(18, FontStyle.Regular, 600, C(55, 65, 81));
                BodyStyle = TS(12, FontStyle.Regular, 400, ForeColor);
                CaptionStyle = TS(11, FontStyle.Regular, 400, C(107, 114, 128));
                ButtonStyle = TS(12, FontStyle.Bold, 600, OnPrimaryColor);
                LinkStyle = TS(12, FontStyle.Regular, 500, C(37, 99, 235));
                OverlineStyle = TS(10, FontStyle.Regular, 600, C(107, 114, 128));

                // Material-ish token set
                DisplayLarge = TS(57, FontStyle.Regular, 300, ForeColor);
                DisplayMedium = TS(45, FontStyle.Regular, 300, ForeColor);
                DisplaySmall = TS(36, FontStyle.Regular, 400, ForeColor);
                HeadlineLarge = TS(32, FontStyle.Bold, 600, ForeColor);
                HeadlineMedium = TS(28, FontStyle.Bold, 600, ForeColor);
                HeadlineSmall = TS(24, FontStyle.Bold, 600, ForeColor);
                TitleLarge = TS(22, FontStyle.Bold, 600, ForeColor);
                TitleMedium = TS(16, FontStyle.Bold, 600, ForeColor);
                TitleSmall = TS(14, FontStyle.Bold, 600, ForeColor);
                BodyLarge = TS(16, FontStyle.Regular, 400, ForeColor);
                BodyMedium = TS(14, FontStyle.Regular, 400, ForeColor);
                BodySmall = TS(12, FontStyle.Regular, 400, C(55, 65, 81));
                LabelLarge = TS(14, FontStyle.Regular, 500, C(55, 65, 81));
                LabelMedium = TS(12, FontStyle.Regular, 500, C(75, 85, 99));
                LabelSmall = TS(11, FontStyle.Regular, 500, C(107, 114, 128));

                // Global font sizes
                FontSizeBlockHeader = 20f;
                FontSizeBlockText = 12f;
                FontSizeQuestion = 16f;
                FontSizeAnswer = 14f;
                FontSizeCaption = 11f;
                FontSizeButton = 12f;

                FontStyleRegular = FontStyle.Regular;
                FontStyleBold = FontStyle.Bold;
                FontStyleItalic = FontStyle.Italic;

                PrimaryTextColor = ForeColor;
                SecondaryTextColor = C(55, 65, 81);
                AccentTextColor = AccentColor;

                PaddingSmall = 6;
                PaddingMedium = 12;
                PaddingLarge = 20;
                BorderRadius = 8;
                BorderSize = 1;

                IconSet = "Fluent";
                ApplyThemeToIcons = true;
                ShadowColor = C(0, 0, 0);
                ShadowOpacity = 0.12f;
                AnimationDurationShort = 0.15;
                AnimationDurationMedium = 0.25;
                AnimationDurationLong = 0.35;
                AnimationEasingFunction = "cubic-bezier(0.2, 0, 0, 1)";
                HighContrastMode = false;
                FocusIndicatorColor = ActiveBorderColor;
                IsDarkTheme = false;

                // ── Buttons
                ButtonBackColor = PrimaryColor;
                ButtonForeColor = OnPrimaryColor;
                ButtonBorderColor = Darken(PrimaryColor, 0.18);
                ButtonHoverBackColor = Darken(PrimaryColor, 0.06);
                ButtonHoverForeColor = OnPrimaryColor;
                ButtonHoverBorderColor = Darken(PrimaryColor, 0.28);
                ButtonPressedBackColor = Darken(PrimaryColor, 0.14);
                ButtonPressedForeColor = OnPrimaryColor;
                ButtonPressedBorderColor = Darken(PrimaryColor, 0.34);

                ButtonSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                ButtonSelectedForeColor = Darken(PrimaryColor, 0.30);
                ButtonSelectedBorderColor = PrimaryColor;
                ButtonSelectedHoverBackColor = Blend(PrimaryColor, Color.White, 0.82);
                ButtonSelectedHoverForeColor = Darken(PrimaryColor, 0.40);
                ButtonSelectedHoverBorderColor = Darken(PrimaryColor, 0.10);

                ButtonErrorBackColor = ErrorColor;
                ButtonErrorForeColor = Color.White;
                ButtonErrorBorderColor = Darken(ErrorColor, 0.22);

                ButtonFont = ButtonStyle;
                ButtonHoverFont = ButtonStyle;
                ButtonSelectedFont = ButtonStyle;

                // ── Cards
                CardBackColor = SurfaceColor;
                CardTextForeColor = ForeColor;
                CardTitleForeColor = ForeColor;
                CardSubTitleForeColor = C(55, 65, 81);
                CardTitleFont = TitleSmall;
                CardSubTitleFont = BodySmall;
                CardHeaderStyle = TitleSmall;
                CardparagraphStyle = BodyMedium;
                CardSubTitleStyle = BodySmall;
                CardrGradiantStartColor = CardBackColor;
                CardGradiantMiddleColor = CardBackColor;
                CardGradiantEndColor = CardBackColor;
                CardGradiantDirection = LinearGradientMode.Vertical;

                // ── Calendar
                CalendarTitleFont = TitleSmall;
                CalendarTitleForColor = ForeColor;
                DaysHeaderFont = LabelMedium;
                CalendarDaysHeaderForColor = C(75, 85, 99);
                SelectedDateFont = LabelMedium;
                CalendarSelectedDateBackColor = Blend(PrimaryColor, Color.White, 0.85);
                CalendarSelectedDateForColor = Darken(PrimaryColor, 0.35);
                CalendarSelectedFont = LabelMedium;
                CalendarUnSelectedFont = LabelSmall;
                CalendarBackColor = SurfaceColor;
                CalendarForeColor = ForeColor;
                CalendarTodayForeColor = PrimaryColor;
                CalendarBorderColor = BorderColor;
                CalendarHoverBackColor = Blend(PrimaryColor, Color.White, 0.92);
                CalendarHoverForeColor = Darken(PrimaryColor, 0.35);
                HeaderFont = TitleSmall;
                MonthFont = TitleSmall;
                YearFont = TitleSmall;
                DaysFont = LabelMedium;
                DaysSelectedFont = LabelMedium;
                DateFont = LabelMedium;
                CalendarFooterColor = C(107, 114, 128);
                FooterFont = LabelSmall;

                // ── Charts
                ChartTitleFont = TitleSmall;
                ChartSubTitleFont = BodySmall;
                ChartBackColor = CardBackColor;
                ChartLineColor = C(55, 65, 81);
                ChartFillColor = Blend(PrimaryColor, Color.White, 0.85);
                ChartAxisColor = C(107, 114, 128);
                ChartTitleColor = ForeColor;
                ChartTextColor = C(55, 65, 81);
                ChartLegendBackColor = CardBackColor;
                ChartLegendTextColor = C(55, 65, 81);
                ChartLegendShapeColor = ChartLineColor;
                ChartGridLineColor = C(229, 231, 235);
                ChartDefaultSeriesColors = new()
            {
                C( 37,  99, 235), C( 14, 165, 233), C( 16, 185, 129),
                C(245, 158,  11), C(239,  68,  68), C(139,  92, 246),
                C( 59, 130, 246), C( 34, 197,  94)
            };

                // ── CheckBox / Radio
                CheckBoxBackColor = BackgroundColor;
                CheckBoxForeColor = ForeColor;
                CheckBoxBorderColor = BorderColor;
                CheckBoxCheckedBackColor = Blend(PrimaryColor, Color.White, 0.90);
                CheckBoxCheckedForeColor = Darken(PrimaryColor, 0.35);
                CheckBoxCheckedBorderColor = PrimaryColor;
                CheckBoxHoverBackColor = Blend(PrimaryColor, Color.White, 0.94);
                CheckBoxHoverForeColor = Darken(PrimaryColor, 0.35);
                CheckBoxHoverBorderColor = PrimaryColor;
                CheckBoxFont = BodyMedium;
                CheckBoxCheckedFont = BodyMedium;

                RadioButtonBackColor = BackgroundColor;
                RadioButtonForeColor = ForeColor;
                RadioButtonBorderColor = BorderColor;
                RadioButtonCheckedBackColor = Blend(PrimaryColor, Color.White, 0.90);
                RadioButtonCheckedForeColor = Darken(PrimaryColor, 0.35);
                RadioButtonCheckedBorderColor = PrimaryColor;
                RadioButtonHoverBackColor = Blend(PrimaryColor, Color.White, 0.94);
                RadioButtonHoverForeColor = Darken(PrimaryColor, 0.35);
                RadioButtonHoverBorderColor = PrimaryColor;
                RadioButtonFont = BodyMedium;
                RadioButtonCheckedFont = BodyMedium;
                RadioButtonSelectedForeColor = Darken(PrimaryColor, 0.35);
                RadioButtonSelectedBackColor = Blend(PrimaryColor, Color.White, 0.90);

                // ── ComboBox
                ComboBoxBackColor = BackgroundColor;
                ComboBoxForeColor = ForeColor;
                ComboBoxBorderColor = BorderColor;
                ComboBoxHoverBackColor = BackgroundColor;
                ComboBoxHoverForeColor = ForeColor;
                ComboBoxHoverBorderColor = ActiveBorderColor;
                ComboBoxSelectedBackColor = Blend(PrimaryColor, Color.White, 0.90);
                ComboBoxSelectedForeColor = Darken(PrimaryColor, 0.35);
                ComboBoxSelectedBorderColor = ActiveBorderColor;
                ComboBoxErrorBackColor = Blend(ErrorColor, Color.White, 0.92);
                ComboBoxErrorForeColor = C(97, 26, 21);
                ComboBoxItemFont = BodyMedium;
                ComboBoxListFont = BodyMedium;

                // ── Lists
                ListBackColor = CardBackColor;
                ListForeColor = ForeColor;
                ListBorderColor = BorderColor;
                ListItemForeColor = ForeColor;
                ListItemHoverForeColor = Darken(PrimaryColor, 0.35);
                ListItemHoverBackColor = Blend(PrimaryColor, Color.White, 0.92);
                ListItemSelectedForeColor = Darken(PrimaryColor, 0.35);
                ListItemSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                ListItemSelectedBorderColor = PrimaryColor;
                ListItemBorderColor = BorderColor;
                ListItemHoverBorderColor = ActiveBorderColor;
                ListTitleFont = TitleSmall;
                ListSelectedFont = BodyMedium;
                ListUnSelectedFont = BodyMedium;

                // ── Links
                LinkColor = C(37, 99, 235);
                VisitedLinkColor = C(109, 40, 217);
                HoverLinkColor = C(59, 130, 246);
                LinkHoverColor = HoverLinkColor;

                // ── Labels
                LabelBackColor = BackgroundColor;
                LabelForeColor = ForeColor;
                LabelBorderColor = Color.Transparent; // will be normalized below
                LabelHoverBorderColor = ActiveBorderColor;
                LabelHoverBackColor = BackgroundColor;
                LabelHoverForeColor = ForeColor;
                LabelSelectedBorderColor = PrimaryColor;
                LabelSelectedBackColor = Blend(PrimaryColor, Color.White, 0.90);
                LabelSelectedForeColor = Darken(PrimaryColor, 0.35);
                LabelDisabledBackColor = DisabledBackColor;
                LabelDisabledForeColor = DisabledForeColor;
                LabelDisabledBorderColor = DisabledBorderColor;
                LabelFont = BodyMedium;
                SubLabelFont = BodySmall;
                SubLabelBackColor = BackgroundColor;
                SubLabelForColor = C(75, 85, 99);
                SubLabelHoverBackColor = BackgroundColor;
                SubLabelHoverForeColor = SubLabelForColor;

                // ── TextBox
                TextBoxBackColor = BackgroundColor;
                TextBoxForeColor = ForeColor;
                TextBoxBorderColor = BorderColor;
                TextBoxHoverBackColor = BackgroundColor;
                TextBoxHoverForeColor = ForeColor;
                TextBoxHoverBorderColor = ActiveBorderColor;
                TextBoxSelectedBackColor = BackgroundColor;
                TextBoxSelectedForeColor = ForeColor;
                TextBoxSelectedBorderColor = C(29, 78, 216); // blue-700
                TextBoxPlaceholderColor = C(107, 114, 128);
                TextBoxErrorBorderColor = ErrorColor;
                TextBoxErrorBackColor = Blend(ErrorColor, Color.White, 0.94);
                TextBoxErrorForeColor = C(97, 26, 21);
                TextBoxErrorTextColor = C(97, 26, 21);
                TextBoxErrorPlaceholderColor = C(149, 57, 57);
                TextBoxErrorTextBoxColor = TextBoxErrorBackColor;
                TextBoxErrorTextBoxBorderColor = Darken(ErrorColor, 0.18);
                TextBoxErrorTextBoxHoverColor = Darken(ErrorColor, 0.12);
                TextBoxFont = BodyMedium;
                TextBoxHoverFont = BodyMedium;
                TextBoxSelectedFont = BodyMedium;

                // ── Grid
                GridHeaderFont = LabelMedium;
                GridRowFont = BodyMedium;
                GridCellFont = BodyMedium;
                GridCellSelectedFont = BodyMedium;
                GridCellHoverFont = BodyMedium;
                GridCellErrorFont = BodyMedium;
                GridColumnFont = LabelMedium;
                GridBackColor = CardBackColor;
                GridForeColor = ForeColor;
                GridHeaderBackColor = C(243, 244, 246);
                GridHeaderForeColor = C(55, 65, 81);
                GridHeaderBorderColor = BorderColor;
                GridHeaderHoverBackColor = C(229, 231, 235);
                GridHeaderHoverForeColor = C(31, 41, 55);
                GridHeaderSelectedBackColor = Blend(PrimaryColor, Color.White, 0.90);
                GridHeaderSelectedForeColor = Darken(PrimaryColor, 0.35);
                GridHeaderHoverBorderColor = ActiveBorderColor;
                GridHeaderSelectedBorderColor = PrimaryColor;
                GridRowHoverBackColor = C(250, 250, 251);
                GridRowHoverForeColor = ForeColor;
                GridRowSelectedBackColor = Blend(PrimaryColor, Color.White, 0.92);
                GridRowSelectedForeColor = Darken(PrimaryColor, 0.35);
                GridRowHoverBorderColor = ActiveBorderColor;
                GridRowSelectedBorderColor = PrimaryColor;
                GridLineColor = C(229, 231, 235);
                RowBackColor = BackgroundColor;
                RowForeColor = ForeColor;
                AltRowBackColor = C(250, 250, 251);
                SelectedRowBackColor = GridRowSelectedBackColor;
                SelectedRowForeColor = GridRowSelectedForeColor;

                // ── Navigation / Menu / Side Menu
                NavigationTitleFont = TitleSmall;
                NavigationSelectedFont = BodyMedium;
                NavigationUnSelectedFont = BodyMedium;
                NavigationBackColor = BackgroundColor;
                NavigationForeColor = ForeColor;
                NavigationHoverBackColor = Blend(PrimaryColor, Color.White, 0.93);
                NavigationHoverForeColor = Darken(PrimaryColor, 0.35);
                NavigationSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                NavigationSelectedForeColor = Darken(PrimaryColor, 0.35);

                MenuBackColor = CardBackColor;
                MenuForeColor = ForeColor;
                MenuBorderColor = BorderColor;
                MenuMainItemForeColor = ForeColor;
                MenuMainItemHoverForeColor = Darken(PrimaryColor, 0.35);
                MenuMainItemHoverBackColor = Blend(PrimaryColor, Color.White, 0.92);
                MenuMainItemSelectedForeColor = Darken(PrimaryColor, 0.35);
                MenuMainItemSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                MenuItemForeColor = ForeColor;
                MenuItemHoverForeColor = Darken(PrimaryColor, 0.35);
                MenuItemHoverBackColor = Blend(PrimaryColor, Color.White, 0.94);
                MenuItemSelectedForeColor = Darken(PrimaryColor, 0.35);
                MenuItemSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                MenuGradiantStartColor = MenuBackColor;
                MenuGradiantMiddleColor = MenuBackColor;
                MenuGradiantEndColor = MenuBackColor;
                MenuGradiantDirection = LinearGradientMode.Vertical;

                SideMenuBackColor = C(245, 247, 250); // distinct from Panel/Back
                SideMenuHoverBackColor = Blend(PrimaryColor, Color.White, 0.93);
                SideMenuSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                SideMenuForeColor = ForeColor;
                SideMenuHoverForeColor = Darken(PrimaryColor, 0.35);
                SideMenuSelectedForeColor = Darken(PrimaryColor, 0.35);
                SideMenuBorderColor = BorderColor;
                SideMenuTitleTextColor = C(31, 41, 55);
                SideMenuTitleBackColor = SideMenuBackColor;
                SideMenuTitleStyle = TitleSmall;
                SideMenuSubTitleTextColor = C(75, 85, 99);
                SideMenuSubTitleBackColor = SideMenuBackColor;
                SideMenuSubTitleStyle = BodySmall;
                SideMenuGradiantStartColor = SideMenuBackColor;
                SideMenuGradiantMiddleColor = SideMenuBackColor;
                SideMenuGradiantEndColor = SideMenuBackColor;
                SideMenuGradiantDirection = LinearGradientMode.Vertical;

                // ── Progress Bar
                ProgressBarBackColor = C(229, 231, 235);
                ProgressBarForeColor = PrimaryColor;
                ProgressBarBorderColor = BorderColor;
                ProgressBarChunkColor = PrimaryColor;
                ProgressBarErrorColor = ErrorColor;
                ProgressBarSuccessColor = SuccessColor;
                ProgressBarFont = LabelSmall;
                ProgressBarInsideTextColor = OnPrimaryColor;
                ProgressBarHoverBackColor = C(209, 213, 219);
                ProgressBarHoverForeColor = Darken(PrimaryColor, 0.05);
                ProgressBarHoverBorderColor = ActiveBorderColor;
                ProgressBarHoverInsideTextColor = OnPrimaryColor;

                // ── Scrollbars
                ScrollBarBackColor = C(243, 244, 246);
                ScrollBarTrackColor = C(243, 244, 246);
                ScrollBarThumbColor = C(209, 213, 219);
                ScrollBarHoverTrackColor = C(229, 231, 235);
                ScrollBarHoverThumbColor = C(156, 163, 175);
                ScrollBarActiveThumbColor = C(107, 114, 128);

                // ── Status bar
                StatusBarBackColor = C(248, 250, 252);
                StatusBarForeColor = C(55, 65, 81);
                StatusBarBorderColor = BorderColor;
                StatusBarHoverBackColor = C(241, 245, 249);
                StatusBarHoverForeColor = C(31, 41, 55);
                StatusBarHoverBorderColor = ActiveBorderColor;

                // ── Star rating
                StarRatingBackColor = BackgroundColor;
                StarRatingForeColor = C(234, 179, 8);
                StarRatingFillColor = C(250, 204, 21);
                StarRatingBorderColor = BorderColor;
                StarRatingHoverBackColor = Blend(AccentColor, Color.White, 0.92);
                StarRatingHoverForeColor = Darken(AccentColor, 0.25);
                StarRatingHoverBorderColor = AccentColor;
                StarRatingSelectedBackColor = Blend(AccentColor, Color.White, 0.88);
                StarRatingSelectedForeColor = Darken(AccentColor, 0.30);
                StarRatingSelectedBorderColor = AccentColor;
                StarTitleFont = TitleSmall;
                StarSubTitleFont = BodySmall;
                StarSelectedFont = BodyMedium;
                StarUnSelectedFont = BodyMedium;
                StarTitleForeColor = ForeColor;
                StarTitleBackColor = BackgroundColor;

                // ── Stats / Dashboard / Task cards
                DashboardTitleFont = TitleSmall;
                DashboardSubTitleFont = BodySmall;
                DashboardBackColor = BackgroundColor;
                DashboardCardBackColor = CardBackColor;
                DashboardCardHoverBackColor = C(250, 250, 251);
                DashboardTitleForeColor = ForeColor;
                DashboardTitleBackColor = BackgroundColor;
                DashboardTitleStyle = TitleSmall;
                DashboardSubTitleForeColor = C(75, 85, 99);
                DashboardSubTitleBackColor = BackgroundColor;
                DashboardSubTitleStyle = BodySmall;
                DashboardGradiantStartColor = BackgroundColor;
                DashboardGradiantMiddleColor = BackgroundColor;
                DashboardGradiantEndColor = BackgroundColor;
                DashboardGradiantDirection = LinearGradientMode.Vertical;

                StatsTitleFont = TitleSmall;
                StatsSelectedFont = BodyMedium;
                StatsUnSelectedFont = BodyMedium;
                StatsCardBackColor = CardBackColor;
                StatsCardForeColor = ForeColor;
                StatsCardBorderColor = BorderColor;
                StatsCardTitleForeColor = ForeColor;
                StatsCardTitleBackColor = CardBackColor;
                StatsCardTitleStyle = TitleSmall;
                StatsCardSubTitleForeColor = C(75, 85, 99);
                StatsCardSubTitleBackColor = CardBackColor;
                StatsCardSubStyleStyle = BodySmall;
                StatsCardValueForeColor = Darken(PrimaryColor, 0.35);
                StatsCardValueBackColor = Blend(PrimaryColor, Color.White, 0.90);
                StatsCardValueBorderColor = PrimaryColor;
                StatsCardValueHoverForeColor = Darken(PrimaryColor, 0.45);
                StatsCardValueHoverBackColor = Blend(PrimaryColor, Color.White, 0.86);
                StatsCardValueHoverBorderColor = Darken(PrimaryColor, 0.10);
                StatsCardValueStyle = TitleSmall;
                StatsCardInfoForeColor = C(75, 85, 99);
                StatsCardInfoBackColor = CardBackColor;
                StatsCardInfoBorderColor = BorderColor;
                StatsCardInfoStyle = BodySmall;
                StatsCardTrendForeColor = SuccessColor;
                StatsCardTrendBackColor = Blend(SuccessColor, Color.White, 0.90);
                StatsCardTrendBorderColor = SuccessColor;
                StatsCardTrendStyle = BodySmall;

                TaskCardTitleFont = TitleSmall;
                TaskCardSelectedFont = BodyMedium;
                TaskCardUnSelectedFont = BodyMedium;
                TaskCardBackColor = CardBackColor;
                TaskCardForeColor = ForeColor;
                TaskCardBorderColor = BorderColor;
                TaskCardTitleForeColor = ForeColor;
                TaskCardTitleBackColor = CardBackColor;
                TaskCardTitleStyle = TitleSmall;
                TaskCardSubTitleForeColor = C(75, 85, 99);
                TaskCardSubTitleBackColor = CardBackColor;
                TaskCardSubStyleStyle = BodySmall;
                TaskCardMetricTextForeColor = C(75, 85, 99);
                TaskCardMetricTextBackColor = C(243, 244, 246);
                TaskCardMetricTextBorderColor = BorderColor;
                TaskCardMetricTextHoverForeColor = Darken(PrimaryColor, 0.35);
                TaskCardMetricTextHoverBackColor = Blend(PrimaryColor, Color.White, 0.92);
                TaskCardMetricTextHoverBorderColor = ActiveBorderColor;
                TaskCardMetricTextStyle = BodySmall;
                TaskCardProgressValueForeColor = Darken(PrimaryColor, 0.35);
                TaskCardProgressValueBackColor = Blend(PrimaryColor, Color.White, 0.90);
                TaskCardProgressValueBorderColor = PrimaryColor;
                TaskCardProgressValueStyle = LabelMedium;

                // ── Dialogs
                DialogBackColor = BackgroundColor;
                DialogForeColor = ForeColor;
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
                DialogYesButtonHoverBackColor = Darken(PrimaryColor, 0.06);
                DialogYesButtonHoverForeColor = OnPrimaryColor;
                DialogYesButtonHoverBorderColor = Darken(PrimaryColor, 0.18);
                DialogCancelButtonBackColor = C(243, 244, 246);
                DialogCancelButtonForeColor = C(31, 41, 55);
                DialogCancelButtonHoverBackColor = C(229, 231, 235);
                DialogCancelButtonHoverForeColor = C(17, 24, 39);
                DialogCancelButtonHoverBorderColor = BorderColor;
                DialogCloseButtonBackColor = C(243, 244, 246);
                DialogCloseButtonForeColor = C(31, 41, 55);
                DialogCloseButtonHoverBackColor = C(229, 231, 235);
                DialogCloseButtonHoverForeColor = C(17, 24, 39);
                DialogCloseButtonHoverBorderColor = BorderColor;
                DialogHelpButtonBackColor = C(243, 244, 246);
                DialogNoButtonBackColor = C(243, 244, 246);
                DialogNoButtonForeColor = C(31, 41, 55);
                DialogNoButtonHoverBackColor = C(229, 231, 235);
                DialogNoButtonHoverForeColor = C(17, 24, 39);
                DialogNoButtonHoverBorderColor = BorderColor;
                DialogOkButtonBackColor = PrimaryColor;
                DialogOkButtonForeColor = OnPrimaryColor;
                DialogOkButtonHoverBackColor = Darken(PrimaryColor, 0.06);
                DialogOkButtonHoverForeColor = OnPrimaryColor;
                DialogOkButtonHoverBorderColor = Darken(PrimaryColor, 0.18);
                DialogWarningButtonBackColor = WarningColor;
                DialogWarningButtonForeColor = Color.White;
                DialogWarningButtonHoverBackColor = Darken(WarningColor, 0.08);
                DialogWarningButtonHoverForeColor = Color.White;
                DialogWarningButtonHoverBorderColor = Darken(WarningColor, 0.18);
                DialogErrorButtonBackColor = ErrorColor;
                DialogErrorButtonForeColor = Color.White;
                DialogErrorButtonHoverBackColor = Darken(ErrorColor, 0.08);
                DialogErrorButtonHoverForeColor = Color.White;
                DialogErrorButtonHoverBorderColor = Darken(ErrorColor, 0.18);
                DialogInformationButtonBackColor = C(59, 130, 246);
                DialogInformationButtonForeColor = Color.White;
                DialogInformationButtonHoverBackColor = C(37, 99, 235);
                DialogInformationButtonHoverForeColor = Color.White;
                DialogInformationButtonHoverBorderColor = Darken(C(59, 130, 246), 0.18);
                DialogQuestionButtonBackColor = C(99, 102, 241);
                DialogQuestionButtonForeColor = Color.White;
                DialogQuestionButtonHoverBackColor = C(79, 70, 229);
                DialogQuestionButtonHoverForeColor = Color.White;
                DialogQuestionButtonHoverBorderColor = Darken(C(99, 102, 241), 0.18);

                // ── Tooltips / Markdown
                ToolTipBackColor = C(31, 41, 55);
                ToolTipForeColor = Color.White;
                ToolTipBorderColor = Color.Transparent;
                ToolTipShadowColor = C(0, 0, 0);
                ToolTipShadowOpacity = Color.FromArgb(64, 0, 0, 0); // type is Color in your interface
                ToolTipTextColor = Color.White;
                ToolTipLinkColor = C(96, 165, 250);
                ToolTipLinkHoverColor = C(147, 197, 253);
                ToolTipLinkVisitedColor = C(196, 181, 253);

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
                BlockquoteBorderColor = C(203, 213, 225);
                InlineCodeBackgroundColor = C(243, 244, 246);
                CodeBlockBackgroundColor = C(246, 248, 252);
                CodeBlockBorderColor = C(229, 231, 235);

                UnorderedList = BodyMedium;
                OrderedList = BodyMedium;
                ListItemSpacing = 4f;
                ListIndentation = 16f;
                Link = LinkStyle;
                LinkIsUnderline = false;
                SmallText = LabelSmall;
                StrongText = TS(12, FontStyle.Bold, 700, ForeColor);
                EmphasisText = TS(12, FontStyle.Italic, 400, ForeColor);

                // ── Tabs
                TabFont = BodyMedium;
                TabHoverFont = BodyMedium;
                TabSelectedFont = BodyMedium;
                TabBackColor = CardBackColor;
                TabForeColor = C(75, 85, 99);
                ActiveTabBackColor = BackgroundColor;
                ActiveTabForeColor = ForeColor;
                InactiveTabBackColor = CardBackColor;
                InactiveTabForeColor = C(107, 114, 128);
                TabBorderColor = BorderColor;
                TabHoverBackColor = Blend(PrimaryColor, Color.White, 0.94);
                TabHoverForeColor = Darken(PrimaryColor, 0.35);
                TabSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                TabSelectedForeColor = Darken(PrimaryColor, 0.35);
                TabSelectedBorderColor = PrimaryColor;
                TabHoverBorderColor = ActiveBorderColor;

                // ── Trees
                TreeTitleFont = TitleSmall;
                TreeNodeSelectedFont = BodyMedium;
                TreeNodeUnSelectedFont = BodyMedium;
                TreeBackColor = CardBackColor;
                TreeForeColor = ForeColor;
                TreeBorderColor = BorderColor;
                TreeNodeForeColor = ForeColor;
                TreeNodeHoverForeColor = Darken(PrimaryColor, 0.35);
                TreeNodeHoverBackColor = Blend(PrimaryColor, Color.White, 0.94);
                TreeNodeSelectedForeColor = Darken(PrimaryColor, 0.35);
                TreeNodeSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                TreeNodeCheckedBoxForeColor = Darken(PrimaryColor, 0.35);
                TreeNodeCheckedBoxBackColor = Blend(PrimaryColor, Color.White, 0.90);

                // ── Login/Company popovers (marketing-ish)
                LoginPopoverBackgroundColor = CardBackColor;
                LoginTitleColor = ForeColor;
                LoginSubtitleColor = C(75, 85, 99);
                LoginDescriptionColor = C(75, 85, 99);
                LoginLinkColor = LinkColor;
                LoginButtonBackgroundColor = PrimaryColor;
                LoginButtonTextColor = OnPrimaryColor;
                LoginDropdownBackgroundColor = CardBackColor;
                LoginDropdownTextColor = ForeColor;
                LoginLogoBackgroundColor = CardBackColor;
                LoginTitleFont = TitleSmall;
                LoginSubtitleFont = BodySmall;
                LoginDescriptionFont = BodySmall;
                LoginLinkFont = BodyMedium;
                LoginButtonFont = ButtonStyle;

                CompanyPopoverBackgroundColor = CardBackColor;
                CompanyTitleColor = ForeColor;
                CompanySubtitleColor = C(75, 85, 99);
                CompanyDescriptionColor = C(75, 85, 99);
                CompanyLinkColor = LinkColor;
                CompanyButtonBackgroundColor = PrimaryColor;
                CompanyButtonTextColor = OnPrimaryColor;
                CompanyDropdownBackgroundColor = CardBackColor;
                CompanyDropdownTextColor = ForeColor;
                CompanyLogoBackgroundColor = CardBackColor;
                CompanyTitleFont = TitleSmall;
                CompanySubTitleFont = BodySmall;
                CompanyDescriptionFont = BodySmall;
                CompanyLinkFont = BodyMedium;
                CompanyButtonFont = ButtonStyle;

                // ── Stepper / Switch
                StepperTitleFont = TitleSmall;
                StepperSelectedFont = BodyMedium;
                StepperUnSelectedFont = BodyMedium;
                StepperBackColor = CardBackColor;
                StepperForeColor = ForeColor;
                StepperBorderColor = BorderColor;
                StepperItemFont = BodyMedium;
                StepperSubTitleFont = BodySmall;
                StepperItemForeColor = ForeColor;
                StepperItemHoverForeColor = Darken(PrimaryColor, 0.35);
                StepperItemHoverBackColor = Blend(PrimaryColor, Color.White, 0.94);
                StepperItemSelectedForeColor = Darken(PrimaryColor, 0.35);
                StepperItemSelectedBackColor = Blend(PrimaryColor, Color.White, 0.88);
                StepperItemSelectedBorderColor = PrimaryColor;
                StepperItemBorderColor = BorderColor;
                StepperItemHoverBorderColor = ActiveBorderColor;
                StepperItemCheckedBoxForeColor = Darken(PrimaryColor, 0.35);
                StepperItemCheckedBoxBackColor = Blend(PrimaryColor, Color.White, 0.90);
                StepperItemCheckedBoxBorderColor = PrimaryColor;

                SwitchTitleFont = TitleSmall;
                SwitchSelectedFont = BodyMedium;
                SwitchUnSelectedFont = BodyMedium;
                SwitchBackColor = C(229, 231, 235);
                SwitchBorderColor = BorderColor;
                SwitchForeColor = C(107, 114, 128);
                SwitchSelectedBackColor = PrimaryColor;
                SwitchSelectedBorderColor = Darken(PrimaryColor, 0.10);
                SwitchSelectedForeColor = OnPrimaryColor;
                SwitchHoverBackColor = C(209, 213, 219);
                SwitchHoverBorderColor = ActiveBorderColor;
                SwitchHoverForeColor = C(75, 85, 99);

                // ── Status & Gradient tokens
                StatusBarBackColor = StatusBarBackColor; // already set
                GradientStartColor = BackgroundColor;
                GradientEndColor = SurfaceColor;
                GradientDirection = LinearGradientMode.Vertical;

                // ── Fill ANY remaining null/empty properties via convention
                FillTypographyDefaultsByConvention();
                FillColorDefaultsByConvention();
            }

            // ─────────────────────────────────────────────────────────────────────────
            // REPLACE THE NOT IMPLEMENTED METHODS WITH THESE
            // ─────────────────────────────────────────────────────────────────────────
            public TypographyStyle GetAnswerFont() => BodyMedium ?? BodyStyle ?? TS(14, FontStyle.Regular, 400, ForeColor);
            public TypographyStyle GetBlockHeaderFont() => TitleSmall ?? TitleStyle ?? TS(18, FontStyle.Bold, 600, ForeColor);
            public TypographyStyle GetBlockTextFont() => BodyMedium ?? BodyStyle ?? TS(14, FontStyle.Regular, 400, ForeColor);
            public TypographyStyle GetButtonFont() => ButtonStyle ?? ButtonFont ?? TS(12, FontStyle.Bold, 600, OnPrimaryColor);
            public TypographyStyle GetCaptionFont() => CaptionStyle ?? LabelSmall ?? TS(11, FontStyle.Regular, 400, C(107, 114, 128));
            public TypographyStyle GetQuestionFont() => TitleSmall ?? TitleStyle ?? TS(16, FontStyle.Bold, 600, ForeColor);

            public void ReplaceTransparentColors(Color fallbackColor)
            {
                foreach (var p in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(pp => pp.CanRead && pp.CanWrite && pp.PropertyType == typeof(Color)))
                {
                    var val = (Color)p.GetValue(this)!;
                    if (val.A == 0) // Transparent or Empty
                        p.SetValue(this, fallbackColor);
                }
            }

            // ─────────────────────────────────────────────────────────────────────────
            // HELPERS
            // ─────────────────────────────────────────────────────────────────────────
            private static Color C(int r, int g, int b) => Color.FromArgb(255, r, g, b);

            private static Color Blend(Color a, Color b, double t)
            {
                t = Math.Clamp(t, 0, 1);
                int A(byte x, byte y) => (int)Math.Round(x + (y - x) * t);
                return Color.FromArgb(
                    255,
                    A(a.R, b.R),
                    A(a.G, b.G),
                    A(a.B, b.B));
            }

            private static Color Darken(Color c, double by) => Blend(c, Color.Black, Math.Clamp(by, 0, 1));
            private static Color Lighten(Color c, double by) => Blend(c, Color.White, Math.Clamp(by, 0, 1));

            private TypographyStyle TS(float size, FontStyle style, int weight, Color color)
                => new TypographyStyle
                {
                    FontFamily = FontName ?? "Segoe UI",
                    FontSize = size,
                    LineHeight = 1.4f,
                    LetterSpacing = 0f,
                    FontWeight =  FontWeight.Normal,
                    FontStyle = style,
                    TextColor = color,
                    IsUnderlined = false,
                    IsStrikeout = false
                };

            private void FillTypographyDefaultsByConvention()
            {
                // Any TypographyStyle property that is null gets BodyMedium (safe default).
                var typoProps = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType.Name == nameof(TypographyStyle));

                foreach (var p in typoProps)
                {
                    if (p.GetValue(this) == null)
                        p.SetValue(this, BodyMedium ?? TS(12, FontStyle.Regular, 400, ForeColor));
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

                    // Heuristics based on property names:
                    if (name.Contains("Error", StringComparison.OrdinalIgnoreCase))
                        p.SetValue(this, ErrorColor);
                    else if (name.Contains("Warning", StringComparison.OrdinalIgnoreCase))
                        p.SetValue(this, WarningColor);
                    else if (name.Contains("Success", StringComparison.OrdinalIgnoreCase))
                        p.SetValue(this, SuccessColor);
                    else if (name.Contains("Hover", StringComparison.OrdinalIgnoreCase) && name.EndsWith("BackColor"))
                        p.SetValue(this, Blend(PrimaryColor, BackgroundColor, 0.94));
                    else if (name.Contains("Hover", StringComparison.OrdinalIgnoreCase) && name.EndsWith("ForeColor"))
                        p.SetValue(this, Darken(PrimaryColor, 0.35));
                    else if (name.Contains("Selected", StringComparison.OrdinalIgnoreCase) && name.EndsWith("BackColor"))
                        p.SetValue(this, Blend(PrimaryColor, BackgroundColor, 0.88));
                    else if (name.Contains("Selected", StringComparison.OrdinalIgnoreCase) && name.EndsWith("ForeColor"))
                        p.SetValue(this, Darken(PrimaryColor, 0.35));
                    else if (name.EndsWith("BorderColor"))
                        p.SetValue(this, BorderColor);
                    else if (name.EndsWith("BackColor"))
                        p.SetValue(this, SurfaceColor);
                    else if (name.EndsWith("ForeColor"))
                        p.SetValue(this, ForeColor);
                    else
                        p.SetValue(this, SurfaceColor); // safe catch-all
                }
            }
        }

        // Optional helpers to mimic weight-y styles with System.Drawing.FontStyle
        internal static class FontStyleExtensions
        {
            public static FontStyle Light(this FontStyle _) => FontStyle.Regular; // if you don't support weight, keep Regular
            public static FontStyle Medium(this FontStyle _) => FontStyle.Regular;
            public static FontStyle SemiBold(this FontStyle _) => FontStyle.Bold;
        }
    }


