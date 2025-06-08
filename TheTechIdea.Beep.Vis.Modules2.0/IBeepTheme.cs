using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
   public interface IBeepTheme
    {
        // Theme Identity
        string ThemeName { get;  }
        string ThemeGuid { get; set; }


         Color AppBarBackColor { get; set; }
       Color AppBarForeColor { get; set; } 
       Color AppBarButtonForeColor { get; set; }
       Color AppBarButtonBackColor { get; set; }
       Color AppBarTextBoxBackColor { get; set; }
       Color AppBarTextBoxForeColor { get; set; }
       Color AppBarLabelForeColor { get; set; }
       Color AppBarLabelBackColor { get; set; }
       Color AppBarTitleForeColor { get; set; }
       Color AppBarTitleBackColor { get; set; }
       Color AppBarSubTitleForeColor { get; set; }
       Color AppBarSubTitleBackColor { get; set; }
       Color AppBarCloseButtonColor { get; set; }
       Color AppBarMaxButtonColor { get; set; }
       Color AppBarMinButtonColor { get; set; }
       TypographyStyle AppBarTitleStyle { get; set; }
       TypographyStyle AppBarSubTitleStyle { get; set; }
       TypographyStyle AppBarTextStyle { get; set; }
       Color AppBarGradiantStartColor { get; set; }
       Color AppBarGradiantEndColor { get; set; }
       Color AppBarGradiantMiddleColor { get; set; }
       LinearGradientMode AppBarGradiantDirection { get; set; }

        // Badge Colors & Fonts
         Color BadgeBackColor { get; set; }
        Color BadgeForeColor { get; set; }
        Color HighlightBackColor { get; set; }
             TypographyStyle BadgeFont { get; set; }

        Color ForeColor { get; set; }
        // Core Properties
        Color BackColor { get; set; }
        Color PanelBackColor { get; set; }
        Color PanelGradiantStartColor { get; set; }
        Color PanelGradiantEndColor { get; set; }
        Color PanelGradiantMiddleColor { get; set; }
        LinearGradientMode PanelGradiantDirection { get; set; }
        Color DisabledBackColor { get; set; }
        Color DisabledForeColor { get; set; }
        Color DisabledBorderColor { get; set; }
        Color BorderColor { get; set; }
        Color ActiveBorderColor { get; set; }
        Color InactiveBorderColor { get; set; }

        // Color Palette
        Color PrimaryColor { get; set; }
        Color SecondaryColor { get; set; }
        Color AccentColor { get; set; }
        Color BackgroundColor { get; set; }
        Color SurfaceColor { get; set; }
        Color ErrorColor { get; set; }
        Color WarningColor { get; set; }
        Color SuccessColor { get; set; }
        Color OnPrimaryColor { get; set; }
        Color OnBackgroundColor { get; set; }

        // Blockquote and Code
        Color BlockquoteBorderColor { get; set; }
        Color InlineCodeBackgroundColor { get; set; }
        Color CodeBlockBackgroundColor { get; set; }
        Color CodeBlockBorderColor { get; set; }

        // Button Properties
        TypographyStyle ButtonFont { get; set; }
        TypographyStyle ButtonHoverFont { get; set; }
        TypographyStyle ButtonSelectedFont { get; set; }
        Color ButtonHoverBackColor { get; set; }
        Color ButtonHoverForeColor { get; set; }
        Color ButtonHoverBorderColor { get; set; }
        Color ButtonSelectedBorderColor { get; set; }
        Color ButtonSelectedBackColor { get; set; }
        Color ButtonSelectedForeColor { get; set; }
        Color ButtonSelectedHoverBackColor { get; set; }
        Color ButtonSelectedHoverForeColor { get; set; }
        Color ButtonSelectedHoverBorderColor { get; set; }
        Color ButtonBackColor { get; set; }
        Color ButtonForeColor { get; set; }
        Color ButtonBorderColor { get; set; }
        Color ButtonErrorBackColor { get; set; }
        Color ButtonErrorForeColor { get; set; }
        Color ButtonErrorBorderColor { get; set; }
        Color ButtonPressedBackColor { get; set; }
        Color ButtonPressedForeColor { get; set; }
        Color ButtonPressedBorderColor { get; set; }

        // Card Properties
        TypographyStyle CardTitleFont { get; set; }
        Color CardTextForeColor { get; set; }
        Color CardBackColor { get; set; }
        Color CardTitleForeColor { get; set; }
        TypographyStyle CardSubTitleFont { get; set; }
        Color CardSubTitleForeColor { get; set; }
        TypographyStyle CardHeaderStyle { get; set; }
        TypographyStyle CardparagraphStyle { get; set; }
        TypographyStyle CardSubTitleStyle { get; set; }
        Color CardrGradiantStartColor { get; set; }
        Color CardGradiantEndColor { get; set; }
        Color CardGradiantMiddleColor { get; set; }
        LinearGradientMode CardGradiantDirection { get; set; }


        TypographyStyle CalendarTitleFont { get; set; }
        Color CalendarTitleForColor { get; set; }
        TypographyStyle DaysHeaderFont { get; set; }
        Color CalendarDaysHeaderForColor { get; set; }
        TypographyStyle SelectedDateFont { get; set; }
        Color CalendarSelectedDateBackColor { get; set; }
        Color CalendarSelectedDateForColor { get; set; }
        TypographyStyle CalendarSelectedFont { get; set; }
        TypographyStyle CalendarUnSelectedFont { get; set; }
        Color CalendarBackColor { get; set; }
        Color CalendarForeColor { get; set; }
        Color CalendarTodayForeColor { get; set; }
        Color CalendarBorderColor { get; set; }
        Color CalendarHoverBackColor { get; set; }
        Color CalendarHoverForeColor { get; set; }
        TypographyStyle HeaderFont { get; set; }
        TypographyStyle MonthFont { get; set; }
        TypographyStyle YearFont { get; set; }
        TypographyStyle DaysFont { get; set; }
        TypographyStyle DaysSelectedFont { get; set; }
        TypographyStyle DateFont { get; set; }
        Color CalendarFooterColor { get; set; }
        TypographyStyle FooterFont { get; set; }


        // Chart Properties
        TypographyStyle ChartTitleFont { get; set; }
        TypographyStyle ChartSubTitleFont { get; set; }
        Color ChartBackColor { get; set; }
        Color ChartLineColor { get; set; }
        Color ChartFillColor { get; set; }
        Color ChartAxisColor { get; set; }
        Color ChartTitleColor { get; set; }
        Color ChartTextColor { get; set; }
        Color ChartLegendBackColor { get; set; }
        Color ChartLegendTextColor { get; set; }
        Color ChartLegendShapeColor { get; set; }
        Color ChartGridLineColor { get; set; }
        List<Color> ChartDefaultSeriesColors { get; set; }

        // CheckBox Properties
        Color CheckBoxBackColor { get; set; }
        Color CheckBoxForeColor { get; set; }
        Color CheckBoxBorderColor { get; set; }
        Color CheckBoxCheckedBackColor { get; set; }
        Color CheckBoxCheckedForeColor { get; set; }
        Color CheckBoxCheckedBorderColor { get; set; }
        Color CheckBoxHoverBackColor { get; set; }
        Color CheckBoxHoverForeColor { get; set; }
        Color CheckBoxHoverBorderColor { get; set; }
        TypographyStyle CheckBoxFont { get; set; }
        TypographyStyle CheckBoxCheckedFont { get; set; }

        // ComboBox Properties
        Color ComboBoxBackColor { get; set; }
        Color ComboBoxForeColor { get; set; }
        Color ComboBoxBorderColor { get; set; }
        Color ComboBoxHoverBackColor { get; set; }
        Color ComboBoxHoverForeColor { get; set; }
        Color ComboBoxHoverBorderColor { get; set; }
        Color ComboBoxSelectedBackColor { get; set; }
        Color ComboBoxSelectedForeColor { get; set; }
        Color ComboBoxSelectedBorderColor { get; set; }
        Color ComboBoxErrorBackColor { get; set; }
        Color ComboBoxErrorForeColor { get; set; }
        TypographyStyle ComboBoxItemFont { get; set; }
        TypographyStyle ComboBoxListFont { get; set; }

        // Company Properties
        Color CompanyPopoverBackgroundColor { get; set; }
        Color CompanyTitleColor { get; set; }
        TypographyStyle CompanyTitleFont { get; set; }
        Color CompanySubtitleColor { get; set; }
        TypographyStyle CompanySubTitleFont { get; set; }
        Color CompanyDescriptionColor { get; set; }
        TypographyStyle CompanyDescriptionFont { get; set; }
        Color CompanyLinkColor { get; set; }
        TypographyStyle CompanyLinkFont { get; set; }
        Color CompanyButtonBackgroundColor { get; set; }
        Color CompanyButtonTextColor { get; set; }
        TypographyStyle CompanyButtonFont { get; set; }
        Color CompanyDropdownBackgroundColor { get; set; }
        Color CompanyDropdownTextColor { get; set; }
        Color CompanyLogoBackgroundColor { get; set; }

        // Dashboard Properties
        TypographyStyle DashboardTitleFont { get; set; }
        TypographyStyle DashboardSubTitleFont { get; set; }
        Color DashboardBackColor { get; set; }
        Color DashboardCardBackColor { get; set; }
        Color DashboardCardHoverBackColor { get; set; }
        Color DashboardTitleForeColor { get; set; }
        Color DashboardTitleBackColor { get; set; }
        TypographyStyle DashboardTitleStyle { get; set; }
        Color DashboardSubTitleForeColor { get; set; }
        Color DashboardSubTitleBackColor { get; set; }
        TypographyStyle DashboardSubTitleStyle { get; set; }
        Color DashboardGradiantStartColor { get; set; }
        Color DashboardGradiantEndColor { get; set; }
        Color DashboardGradiantMiddleColor { get; set; }
        LinearGradientMode DashboardGradiantDirection { get; set; }

        // Dialog Properties
        Color DialogBackColor { get; set; }
        Color DialogForeColor { get; set; }
        TypographyStyle DialogYesButtonFont { get; set; }
        TypographyStyle DialogNoButtonFont { get; set; }
        TypographyStyle DialogOkButtonFont { get; set; }
        TypographyStyle DialogCancelButtonFont { get; set; }
        TypographyStyle DialogWarningButtonFont { get; set; }
        TypographyStyle DialogErrorButtonFont { get; set; }
        TypographyStyle DialogInformationButtonFont { get; set; }
        TypographyStyle DialogQuestionButtonFont { get; set; }
        TypographyStyle DialogHelpButtonFont { get; set; }
        TypographyStyle DialogCloseButtonFont { get; set; }
        TypographyStyle DialogYesButtonHoverFont { get; set; }
        TypographyStyle DialogNoButtonHoverFont { get; set; }
        TypographyStyle DialogOkButtonHoverFont { get; set; }
        Color DialogYesButtonBackColor { get; set; }
        Color DialogYesButtonForeColor { get; set; }
        Color DialogYesButtonHoverBackColor { get; set; }
        Color DialogYesButtonHoverForeColor { get; set; }
        Color DialogYesButtonHoverBorderColor { get; set; }
        Color DialogCancelButtonBackColor { get; set; }
        Color DialogCancelButtonForeColor { get; set; }
        Color DialogCancelButtonHoverBackColor { get; set; }
        Color DialogCancelButtonHoverForeColor { get; set; }
        Color DialogCancelButtonHoverBorderColor { get; set; }
        Color DialogCloseButtonBackColor { get; set; }
        Color DialogCloseButtonForeColor { get; set; }
        Color DialogCloseButtonHoverBackColor { get; set; }
        Color DialogCloseButtonHoverForeColor { get; set; }
        Color DialogCloseButtonHoverBorderColor { get; set; }
        Color DialogHelpButtonBackColor { get; set; }
        Color DialogNoButtonBackColor { get; set; }
        Color DialogNoButtonForeColor { get; set; }
        Color DialogNoButtonHoverBackColor { get; set; }
        Color DialogNoButtonHoverForeColor { get; set; }
        Color DialogNoButtonHoverBorderColor { get; set; }
        Color DialogOkButtonBackColor { get; set; }
        Color DialogOkButtonForeColor { get; set; }
        Color DialogOkButtonHoverBackColor { get; set; }
        Color DialogOkButtonHoverForeColor { get; set; }
        Color DialogOkButtonHoverBorderColor { get; set; }
        Color DialogWarningButtonBackColor { get; set; }
        Color DialogWarningButtonForeColor { get; set; }
        Color DialogWarningButtonHoverBackColor { get; set; }
        Color DialogWarningButtonHoverForeColor { get; set; }
        Color DialogWarningButtonHoverBorderColor { get; set; }
        Color DialogErrorButtonBackColor { get; set; }
        Color DialogErrorButtonForeColor { get; set; }
        Color DialogErrorButtonHoverBackColor { get; set; }
        Color DialogErrorButtonHoverForeColor { get; set; }
        Color DialogErrorButtonHoverBorderColor { get; set; }
        Color DialogInformationButtonBackColor { get; set; }
        Color DialogInformationButtonForeColor { get; set; }
        Color DialogInformationButtonHoverBackColor { get; set; }
        Color DialogInformationButtonHoverForeColor { get; set; }
        Color DialogInformationButtonHoverBorderColor { get; set; }
        Color DialogQuestionButtonBackColor { get; set; }
        Color DialogQuestionButtonForeColor { get; set; }
        Color DialogQuestionButtonHoverBackColor { get; set; }
        Color DialogQuestionButtonHoverForeColor { get; set; }
        Color DialogQuestionButtonHoverBorderColor { get; set; }

        //Fonts
       string FontName { get; set; }
       float FontSize { get; set; }
       TypographyStyle TitleStyle { get; set; }
       TypographyStyle SubtitleStyle { get; set; }
       TypographyStyle BodyStyle { get; set; }
       TypographyStyle CaptionStyle { get; set; }
       TypographyStyle ButtonStyle { get; set; }
       TypographyStyle LinkStyle { get; set; }
       TypographyStyle OverlineStyle { get; set; }

        // Gradient Properties
       Color GradientStartColor { get; set; }
       Color GradientEndColor { get; set; }
       LinearGradientMode GradientDirection { get; set; }

        // Grid Fonts
       TypographyStyle GridHeaderFont { get; set; }
       TypographyStyle GridRowFont { get; set; }
       TypographyStyle GridCellFont { get; set; }
       TypographyStyle GridCellSelectedFont { get; set; }
       TypographyStyle GridCellHoverFont { get; set; }
       TypographyStyle GridCellErrorFont { get; set; }
       TypographyStyle GridColumnFont { get; set; }

        // Grid Colors
       Color GridBackColor { get; set; }
       Color GridForeColor { get; set; }
       Color GridHeaderBackColor { get; set; }
       Color GridHeaderForeColor { get; set; }
       Color GridHeaderBorderColor { get; set; }
       Color GridHeaderHoverBackColor { get; set; }
       Color GridHeaderHoverForeColor { get; set; }
       Color GridHeaderSelectedBackColor { get; set; }
       Color GridHeaderSelectedForeColor { get; set; }
       Color GridHeaderHoverBorderColor { get; set; }
       Color GridHeaderSelectedBorderColor { get; set; }
       Color GridRowHoverBackColor { get; set; }
       Color GridRowHoverForeColor { get; set; }
       Color GridRowSelectedBackColor { get; set; }
       Color GridRowSelectedForeColor { get; set; }
       Color GridRowHoverBorderColor { get; set; }
       Color GridRowSelectedBorderColor { get; set; }
       Color GridLineColor { get; set; }
       Color RowBackColor { get; set; }
       Color RowForeColor { get; set; }
       Color AltRowBackColor { get; set; }
       Color SelectedRowBackColor { get; set; }
       Color SelectedRowForeColor { get; set; }

        // Label Colors and Fonts
       Color LabelBackColor { get; set; }
       Color LabelForeColor { get; set; }
       Color LabelBorderColor { get; set; }
       Color LabelHoverBorderColor { get; set; }
       Color LabelHoverBackColor { get; set; }
       Color LabelHoverForeColor { get; set; }
       Color LabelSelectedBorderColor { get; set; }
       Color LabelSelectedBackColor { get; set; }
       Color LabelSelectedForeColor { get; set; }
       Color LabelDisabledBackColor { get; set; }
       Color LabelDisabledForeColor { get; set; }
       Color LabelDisabledBorderColor { get; set; }
       TypographyStyle LabelFont { get; set; }
       TypographyStyle SubLabelFont { get; set; }
       Color SubLabelForColor { get; set; }
       Color SubLabelBackColor { get; set; }
       Color SubLabelHoverBackColor { get; set; }
       Color SubLabelHoverForeColor { get; set; }

        // Link (TextBox Link) colors
       Color LinkColor { get; set; }
       Color VisitedLinkColor { get; set; }
       Color HoverLinkColor { get; set; }
       Color LinkHoverColor { get; set; }

        // List Fonts & Colors
       TypographyStyle ListTitleFont { get; set; }
       TypographyStyle ListSelectedFont { get; set; }
       TypographyStyle ListUnSelectedFont { get; set; }
       Color ListBackColor { get; set; }
       Color ListForeColor { get; set; }
       Color ListBorderColor { get; set; }
       Color ListItemForeColor { get; set; }
       Color ListItemHoverForeColor { get; set; }
       Color ListItemHoverBackColor { get; set; }
       Color ListItemSelectedForeColor { get; set; }
       Color ListItemSelectedBackColor { get; set; }
       Color ListItemSelectedBorderColor { get; set; }
       Color ListItemBorderColor { get; set; }
       Color ListItemHoverBorderColor { get; set; }
        // Login Popover Colors
       Color LoginPopoverBackgroundColor { get; set; }
       Color LoginTitleColor { get; set; }
       TypographyStyle LoginTitleFont { get; set; }
       Color LoginSubtitleColor { get; set; } 
       TypographyStyle LoginSubtitleFont { get; set; }
       Color LoginDescriptionColor { get; set; } 
       TypographyStyle LoginDescriptionFont { get; set; }
       Color LoginLinkColor { get; set; }
       TypographyStyle LoginLinkFont { get; set; }
       Color LoginButtonBackgroundColor { get; set; } 
       Color LoginButtonTextColor { get; set; }
       TypographyStyle LoginButtonFont { get; set; }
       Color LoginDropdownBackgroundColor { get; set; } 
       Color LoginDropdownTextColor { get; set; }
       Color LoginLogoBackgroundColor { get; set; }
        // Menu Fonts & Colors
       TypographyStyle MenuTitleFont { get; set; }
       TypographyStyle MenuItemSelectedFont { get; set; }
       TypographyStyle MenuItemUnSelectedFont { get; set; }
       Color MenuBackColor { get; set; }
       Color MenuForeColor { get; set; }
       Color MenuBorderColor { get; set; }
       Color MenuMainItemForeColor { get; set; }
       Color MenuMainItemHoverForeColor { get; set; }
       Color MenuMainItemHoverBackColor { get; set; }
       Color MenuMainItemSelectedForeColor { get; set; }
       Color MenuMainItemSelectedBackColor { get; set; }
       Color MenuItemForeColor { get; set; }
       Color MenuItemHoverForeColor { get; set; }
       Color MenuItemHoverBackColor { get; set; }
       Color MenuItemSelectedForeColor { get; set; }
       Color MenuItemSelectedBackColor { get; set; }
       Color MenuGradiantStartColor { get; set; }
       Color MenuGradiantEndColor { get; set; }
       Color MenuGradiantMiddleColor { get; set; }
       LinearGradientMode MenuGradiantDirection { get; set; }

        // Miscellaneous, Utility, and General Properties
       string FontFamily { get; set; }
       float FontSizeBlockHeader { get; set; } 
       float FontSizeBlockText { get; set; } 
       float FontSizeQuestion { get; set; }
       float FontSizeAnswer { get; set; } 
       float FontSizeCaption { get; set; } 
       float FontSizeButton { get; set; }
       FontStyle FontStyleRegular { get; set; }
       FontStyle FontStyleBold { get; set; }
       FontStyle FontStyleItalic { get; set; }
       Color PrimaryTextColor { get; set; } 
       Color SecondaryTextColor { get; set; }
       Color AccentTextColor { get; set; } 
       int PaddingSmall { get; set; }
       int PaddingMedium { get; set; }
       int PaddingLarge { get; set; }
       int BorderRadius { get; set; }
       int BorderSize { get; set; } 
       string IconSet { get; set; }
       bool ApplyThemeToIcons { get; set; }
       Color ShadowColor { get; set; }
       float ShadowOpacity { get; set; }
       double AnimationDurationShort { get; set; }
       double AnimationDurationMedium { get; set; }
       double AnimationDurationLong { get; set; }
       string AnimationEasingFunction { get; set; }
       bool HighContrastMode { get; set; }
       Color FocusIndicatorColor { get; set; }
       bool IsDarkTheme { get; set; }

        // Navigation & Breadcrumbs Fonts & Colors
       TypographyStyle NavigationTitleFont { get; set; }
       TypographyStyle NavigationSelectedFont { get; set; }
       TypographyStyle NavigationUnSelectedFont { get; set; }

       Color NavigationBackColor { get; set; }
       Color NavigationForeColor { get; set; }
       Color NavigationHoverBackColor { get; set; }
       Color NavigationHoverForeColor { get; set; }
       Color NavigationSelectedBackColor { get; set; }
       Color NavigationSelectedForeColor { get; set; }

        // ProgressBar properties
       Color ProgressBarBackColor { get; set; }
       Color ProgressBarForeColor { get; set; }
       Color ProgressBarBorderColor { get; set; }
       Color ProgressBarChunkColor { get; set; }
       Color ProgressBarErrorColor { get; set; }
       Color ProgressBarSuccessColor { get; set; }
       TypographyStyle ProgressBarFont { get; set; }
       Color ProgressBarInsideTextColor { get; set; }
       Color ProgressBarHoverBackColor { get; set; }
       Color ProgressBarHoverForeColor { get; set; }
       Color ProgressBarHoverBorderColor { get; set; }
       Color ProgressBarHoverInsideTextColor { get; set; }

        // RadioButton properties
       Color RadioButtonBackColor { get; set; }
       Color RadioButtonForeColor { get; set; }
       Color RadioButtonBorderColor { get; set; }
       Color RadioButtonCheckedBackColor { get; set; }
       Color RadioButtonCheckedForeColor { get; set; }
       Color RadioButtonCheckedBorderColor { get; set; }
       Color RadioButtonHoverBackColor { get; set; }
       Color RadioButtonHoverForeColor { get; set; }
       Color RadioButtonHoverBorderColor { get; set; }
       TypographyStyle RadioButtonFont { get; set; }
       TypographyStyle RadioButtonCheckedFont { get; set; }
       Color RadioButtonSelectedForeColor { get; set; }
       Color RadioButtonSelectedBackColor { get; set; }

        // ScrollBar Colors
       Color ScrollBarBackColor { get; set; }
       Color ScrollBarThumbColor { get; set; }
       Color ScrollBarTrackColor { get; set; }
       Color ScrollBarHoverThumbColor { get; set; }
       Color ScrollBarHoverTrackColor { get; set; }
       Color ScrollBarActiveThumbColor { get; set; }

        // ScrollList Fonts & Colors
       TypographyStyle ScrollListTitleFont { get; set; }
       TypographyStyle ScrollListSelectedFont { get; set; }
       TypographyStyle ScrollListUnSelectedFont { get; set; }
       Color ScrollListBackColor { get; set; }
       Color ScrollListForeColor { get; set; }
       Color ScrollListBorderColor { get; set; }
       Color ScrollListItemForeColor { get; set; }
       Color ScrollListItemHoverForeColor { get; set; }
       Color ScrollListItemHoverBackColor { get; set; }
       Color ScrollListItemSelectedForeColor { get; set; }
       Color ScrollListItemSelectedBackColor { get; set; }
       Color ScrollListItemSelectedBorderColor { get; set; }
       Color ScrollListItemBorderColor { get; set; }
       TypographyStyle ScrollListIItemFont { get; set; }
       TypographyStyle ScrollListItemSelectedFont { get; set; }

        // Side Menu Fonts & Colors
       TypographyStyle SideMenuTitleFont { get; set; }
       TypographyStyle SideMenuSubTitleFont { get; set; }
       TypographyStyle SideMenuTextFont { get; set; }
       Color SideMenuBackColor { get; set; }
       Color SideMenuHoverBackColor { get; set; }
       Color SideMenuSelectedBackColor { get; set; }
       Color SideMenuForeColor { get; set; }
       Color SideMenuSelectedForeColor { get; set; }
       Color SideMenuHoverForeColor { get; set; }
       Color SideMenuBorderColor { get; set; }
       Color SideMenuTitleTextColor { get; set; }
       Color SideMenuTitleBackColor { get; set; }
       TypographyStyle SideMenuTitleStyle { get; set; }
       Color SideMenuSubTitleTextColor { get; set; }
       Color SideMenuSubTitleBackColor { get; set; }
       TypographyStyle SideMenuSubTitleStyle { get; set; }
       Color SideMenuGradiantStartColor { get; set; }
       Color SideMenuGradiantEndColor { get; set; }
       Color SideMenuGradiantMiddleColor { get; set; }
       LinearGradientMode SideMenuGradiantDirection { get; set; }

        // Star Rating Fonts & Colors
       Color StarRatingForeColor { get; set; }
       Color StarRatingBackColor { get; set; }
       Color StarRatingBorderColor { get; set; }
       Color StarRatingFillColor { get; set; }
       Color StarRatingHoverForeColor { get; set; }
       Color StarRatingHoverBackColor { get; set; }
       Color StarRatingHoverBorderColor { get; set; }
       Color StarRatingSelectedForeColor { get; set; }
       Color StarRatingSelectedBackColor { get; set; }
       Color StarRatingSelectedBorderColor { get; set; }
       TypographyStyle StarTitleFont { get; set; }
       TypographyStyle StarSubTitleFont { get; set; }
       TypographyStyle StarSelectedFont { get; set; }
       TypographyStyle StarUnSelectedFont { get; set; }
       Color StarTitleForeColor { get; set; }
       Color StarTitleBackColor { get; set; }

        // Stats Card Fonts & Colors
       TypographyStyle StatsTitleFont { get; set; }
       TypographyStyle StatsSelectedFont { get; set; }
       TypographyStyle StatsUnSelectedFont { get; set; }
       Color StatsCardBackColor { get; set; }
       Color StatsCardForeColor { get; set; }
       Color StatsCardBorderColor { get; set; }
       Color StatsCardTitleForeColor { get; set; }
       Color StatsCardTitleBackColor { get; set; }
       TypographyStyle StatsCardTitleStyle { get; set; }
       Color StatsCardSubTitleForeColor { get; set; }
       Color StatsCardSubTitleBackColor { get; set; }
       TypographyStyle StatsCardSubStyleStyle { get; set; }
       Color StatsCardValueForeColor { get; set; }
       Color StatsCardValueBackColor { get; set; }
       Color StatsCardValueBorderColor { get; set; }
       Color StatsCardValueHoverForeColor { get; set; }
       Color StatsCardValueHoverBackColor { get; set; }
       Color StatsCardValueHoverBorderColor { get; set; }
       TypographyStyle StatsCardValueStyle { get; set; }
       Color StatsCardInfoForeColor { get; set; }
       Color StatsCardInfoBackColor { get; set; }
       Color StatsCardInfoBorderColor { get; set; }
       TypographyStyle StatsCardInfoStyle { get; set; }
       Color StatsCardTrendForeColor { get; set; }
       Color StatsCardTrendBackColor { get; set; }
       Color StatsCardTrendBorderColor { get; set; }
       TypographyStyle StatsCardTrendStyle { get; set; }

        // Status Bar Colors
       Color StatusBarBackColor { get; set; }
       Color StatusBarForeColor { get; set; }
       Color StatusBarBorderColor { get; set; }
       Color StatusBarHoverBackColor { get; set; }
       Color StatusBarHoverForeColor { get; set; }
       Color StatusBarHoverBorderColor { get; set; }

        // Stepper Fonts & Colors
       TypographyStyle StepperTitleFont { get; set; }
       TypographyStyle StepperSelectedFont { get; set; }
       TypographyStyle StepperUnSelectedFont { get; set; }
       Color StepperBackColor { get; set; }
       Color StepperForeColor { get; set; }
       Color StepperBorderColor { get; set; }
       Color StepperItemForeColor { get; set; }
       TypographyStyle StepperItemFont { get; set; }
       TypographyStyle StepperSubTitleFont { get; set; }
       Color StepperItemHoverForeColor { get; set; }
       Color StepperItemHoverBackColor { get; set; }
       Color StepperItemSelectedForeColor { get; set; }
       Color StepperItemSelectedBackColor { get; set; }
       Color StepperItemSelectedBorderColor { get; set; }
       Color StepperItemBorderColor { get; set; }
       Color StepperItemHoverBorderColor { get; set; }
       Color StepperItemCheckedBoxForeColor { get; set; }
       Color StepperItemCheckedBoxBackColor { get; set; }
       Color StepperItemCheckedBoxBorderColor { get; set; }

        // Switch control Fonts & Colors
       TypographyStyle SwitchTitleFont { get; set; }
       TypographyStyle SwitchSelectedFont { get; set; }
       TypographyStyle SwitchUnSelectedFont { get; set; }
       Color SwitchBackColor { get; set; }
       Color SwitchBorderColor { get; set; }
       Color SwitchForeColor { get; set; }
       Color SwitchSelectedBackColor { get; set; }
       Color SwitchSelectedBorderColor { get; set; }
       Color SwitchSelectedForeColor { get; set; }
       Color SwitchHoverBackColor { get; set; }
       Color SwitchHoverBorderColor { get; set; }
       Color SwitchHoverForeColor { get; set; }

        // Tab Fonts & Colors
       TypographyStyle TabFont { get; set; }
       TypographyStyle TabHoverFont { get; set; }
       TypographyStyle TabSelectedFont { get; set; }
       Color TabBackColor { get; set; }
       Color TabForeColor { get; set; }
       Color ActiveTabBackColor { get; set; }
       Color ActiveTabForeColor { get; set; }
       Color InactiveTabBackColor { get; set; }
       Color InactiveTabForeColor { get; set; }
       Color TabBorderColor { get; set; }
       Color TabHoverBackColor { get; set; }
       Color TabHoverForeColor { get; set; }
       Color TabSelectedBackColor { get; set; }
       Color TabSelectedForeColor { get; set; }
       Color TabSelectedBorderColor { get; set; }
       Color TabHoverBorderColor { get; set; }

        // Task Card Fonts & Colors
       TypographyStyle TaskCardTitleFont { get; set; }
       TypographyStyle TaskCardSelectedFont { get; set; }
       TypographyStyle TaskCardUnSelectedFont { get; set; }
       Color TaskCardBackColor { get; set; }
       Color TaskCardForeColor { get; set; }
       Color TaskCardBorderColor { get; set; }
       Color TaskCardTitleForeColor { get; set; }
       Color TaskCardTitleBackColor { get; set; }
       TypographyStyle TaskCardTitleStyle { get; set; }
       Color TaskCardSubTitleForeColor { get; set; }
       Color TaskCardSubTitleBackColor { get; set; }
       TypographyStyle TaskCardSubStyleStyle { get; set; }
       Color TaskCardMetricTextForeColor { get; set; }
       Color TaskCardMetricTextBackColor { get; set; }
       Color TaskCardMetricTextBorderColor { get; set; }
       Color TaskCardMetricTextHoverForeColor { get; set; }
       Color TaskCardMetricTextHoverBackColor { get; set; }
       Color TaskCardMetricTextHoverBorderColor { get; set; }
       TypographyStyle TaskCardMetricTextStyle { get; set; }
       Color TaskCardProgressValueForeColor { get; set; }
       Color TaskCardProgressValueBackColor { get; set; }
       Color TaskCardProgressValueBorderColor { get; set; }
       TypographyStyle TaskCardProgressValueStyle { get; set; }

        // Testimony/Testimonial Colors & Fonts
       TypographyStyle TestimoniaTitleFont { get; set; }
       TypographyStyle TestimoniaSelectedFont { get; set; }
       TypographyStyle TestimoniaUnSelectedFont { get; set; }
       Color TestimonialBackColor { get; set; } 
       Color TestimonialTextColor { get; set; } 
       Color TestimonialNameColor { get; set; } 
       Color TestimonialDetailsColor { get; set; } 
       Color TestimonialDateColor { get; set; } 
       Color TestimonialRatingColor { get; set; } 
       Color TestimonialStatusColor { get; set; }

        // Textbox colors and Fonts
       Color TextBoxBackColor { get; set; }
       Color TextBoxForeColor { get; set; }
       Color TextBoxBorderColor { get; set; }
       Color TextBoxHoverBorderColor { get; set; }
       Color TextBoxHoverBackColor { get; set; }
       Color TextBoxHoverForeColor { get; set; }
       Color TextBoxSelectedBorderColor { get; set; }
       Color TextBoxSelectedBackColor { get; set; }
       Color TextBoxSelectedForeColor { get; set; }
       Color TextBoxPlaceholderColor { get; set; }
       Color TextBoxErrorBorderColor { get; set; }
       Color TextBoxErrorBackColor { get; set; }
       Color TextBoxErrorForeColor { get; set; }
       Color TextBoxErrorTextColor { get; set; }
       Color TextBoxErrorPlaceholderColor { get; set; }
       Color TextBoxErrorTextBoxColor { get; set; }
       Color TextBoxErrorTextBoxBorderColor { get; set; }
       Color TextBoxErrorTextBoxHoverColor { get; set; }
       TypographyStyle TextBoxFont { get; set; }
       TypographyStyle TextBoxHoverFont { get; set; }
       TypographyStyle TextBoxSelectedFont { get; set; }

        // ToolTip Colors
       Color ToolTipBackColor { get; set; }
       Color ToolTipForeColor { get; set; }
       Color ToolTipBorderColor { get; set; }
       Color ToolTipShadowColor { get; set; }
       Color ToolTipShadowOpacity { get; set; }
       Color ToolTipTextColor { get; set; }
       Color ToolTipLinkColor { get; set; }
       Color ToolTipLinkHoverColor { get; set; }
       Color ToolTipLinkVisitedColor { get; set; }

        // Tree Fonts & Colors
       TypographyStyle TreeTitleFont { get; set; }
       TypographyStyle TreeNodeSelectedFont { get; set; }
       TypographyStyle TreeNodeUnSelectedFont { get; set; }
       Color TreeBackColor { get; set; }
       Color TreeForeColor { get; set; }
       Color TreeBorderColor { get; set; }
       Color TreeNodeForeColor { get; set; }
       Color TreeNodeHoverForeColor { get; set; }
       Color TreeNodeHoverBackColor { get; set; }
       Color TreeNodeSelectedForeColor { get; set; }
       Color TreeNodeSelectedBackColor { get; set; }
       Color TreeNodeCheckedBoxForeColor { get; set; }
       Color TreeNodeCheckedBoxBackColor { get; set; }

        // Typography Styles
       TypographyStyle Heading1 { get; set; }
       TypographyStyle Heading2 { get; set; }
       TypographyStyle Heading3 { get; set; }
       TypographyStyle Heading4 { get; set; }
       TypographyStyle Heading5 { get; set; }
       TypographyStyle Heading6 { get; set; }
       TypographyStyle Paragraph { get; set; }
       TypographyStyle Blockquote { get; set; }
       float BlockquoteBorderWidth { get; set; }
       float BlockquotePadding { get; set; }
       TypographyStyle InlineCode { get; set; }
       float InlineCodePadding { get; set; }
       TypographyStyle CodeBlock { get; set; }
       float CodeBlockBorderWidth { get; set; }
       float CodeBlockPadding { get; set; }
       TypographyStyle UnorderedList { get; set; }
       TypographyStyle OrderedList { get; set; }
       float ListItemSpacing { get; set; }
       float ListIndentation { get; set; }
       TypographyStyle Link { get; set; }
       bool LinkIsUnderline { get; set; }
       TypographyStyle SmallText { get; set; }
       TypographyStyle StrongText { get; set; }
       TypographyStyle EmphasisText { get; set; }
       TypographyStyle DisplayLarge { get; set; }
       TypographyStyle DisplayMedium { get; set; }
       TypographyStyle DisplaySmall { get; set; }
       TypographyStyle HeadlineLarge { get; set; }
       TypographyStyle HeadlineMedium { get; set; }
       TypographyStyle HeadlineSmall { get; set; }
       TypographyStyle TitleLarge { get; set; }
       TypographyStyle TitleMedium { get; set; }
       TypographyStyle TitleSmall { get; set; }
       TypographyStyle BodyLarge { get; set; }
       TypographyStyle BodyMedium { get; set; }
       TypographyStyle BodySmall { get; set; }
       TypographyStyle LabelLarge { get; set; }
       TypographyStyle LabelMedium { get; set; }
       TypographyStyle LabelSmall { get; set; }

        // Utility Methods

       TypographyStyle GetBlockHeaderFont();

       TypographyStyle GetBlockTextFont();

       TypographyStyle GetQuestionFont();

       TypographyStyle GetAnswerFont();
      

       TypographyStyle GetCaptionFont();
     

       TypographyStyle GetButtonFont();
      

       
       

       void ReplaceTransparentColors(Color fallbackColor);
        

       bool Equals(object obj);
     

       int GetHashCode();






    }
}
