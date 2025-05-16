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
        public Color BackColor { get; set; }
        public Color PanelBackColor { get; set; }
        public Color PanelGradiantStartColor { get; set; }
        public Color PanelGradiantEndColor { get; set; }
        public Color PanelGradiantMiddleColor { get; set; }
        public Color PanelGradiantDirection { get; set; }
        public Color DisabledBackColor { get; set; }
        public Color DisabledForeColor { get; set; }
        public Color DisabledBorderColor { get; set; }

        public Color BorderColor { get; set; }
        public Color ActiveBorderColor { get; set; }
        public Color InactiveBorderColor { get; set; }

        // Font and Typography properties
        public string FontName { get; set; }
        public float FontSize { get; set; }

        // Gradient Properties
        public Color GradientStartColor { get; set; }
        public Color GradientEndColor { get; set; }
        public LinearGradientMode GradientDirection { get; set; }

        // AppBar Buttons colors and other controls colors
        public Color AppBarBackColor { get; set; }
        public Color AppBarForeColor { get; set; } = Color.White;
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
        public Color AppBarGradiantDirection { get; set; }




        // Styles
        public TypographyStyle TitleStyle { get; set; }
        public TypographyStyle SubtitleStyle { get; set; }
        public TypographyStyle BodyStyle { get; set; }
        public TypographyStyle CaptionStyle { get; set; }
        public TypographyStyle ButtonStyle { get; set; }
        public TypographyStyle LinkStyle { get; set; }
        public TypographyStyle OverlineStyle { get; set; }
        // Button Colors
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
        //public Color ButtonDisabledBackColor { get; set; }
        //public Color ButtonDisabledForeColor { get; set; }
        //public Color ButtonDisabledBorderColor { get; set; }
        public Color ButtonErrorBackColor { get; set; }
        public Color ButtonErrorForeColor { get; set; }
        public Color ButtonErrorBorderColor { get; set; }
        public Color ButtonPressedBackColor { get; set; }
        public Color ButtonPressedForeColor { get; set; }
        public Color ButtonPressedBorderColor { get; set; }


        // Textbox colors
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
        public Font  TextBoxFont { get; set; }
        //public Color TextBoxDisabledBackColor { get; set; }
        //public Color TextBoxDisabledForeColor { get; set; }
        //public Color TextBoxDisabledBorderColor { get; set; }
        //public Color TextBoxDisabledPlaceholderColor { get; set; }
        // Label Colors

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
        public Font LabelFont { get; set; }
        public Font SubLabelFont { get; set; }
        public Color SubLabelForColor { get; set; }
        public Color SubLabelBackColor { get; set; }
        public Color SubLabelHoverBackColor { get; set; }
        public Color SubLabelHoverForeColor { get; set; }

        // ComboBox Colors
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
        public Font  ComboBoxItemFont { get; set; }
        public Font  ComboBoxListFont { get; set; }


        // CheckBox colors
        public Color CheckBoxBackColor { get; set; }
        public Color CheckBoxForeColor { get; set; }
        public Color CheckBoxBorderColor { get; set; }
        public Color CheckBoxSelectedForeColor { get; set; }
        public Color CheckBoxSelectedBackColor { get; set; }
        public Color CheckBoxHoverBackColor { get; set; }
        public Color CheckBoxHoverForeColor { get; set; }
        public Color CheckBoxHoverBorderColor { get; set; }
        public Font CheckBoxFont { get; set; }

        // Radio Button Colors
        public Color RadioButtonBackColor { get; set; }
        public Color RadioButtonForeColor { get; set; }
        public Color RadioButtonBorderColor { get; set; }
        public Color RadioButtonSelectedForeColor { get; set; }
        public Color RadioButtonSelectedBackColor { get; set; }
        public Color RadioButtonHoverBackColor { get; set; }
        public Color RadioButtonHoverForeColor { get; set; }
        public Color RadioButtonHoverBorderColor { get; set; }
        public Font RadioButtonFont { get; set; }
        // Progress Bar Colors
        public Color ProgressBarBackColor { get; set; }
        public Color ProgressBarForeColor { get; set; }
        public Color ProgressBarBorderColor { get; set; }
        public Color ProgressBarInsideTextColor { get; set; }
        public Color ProgressBarHoverBackColor { get; set; }
        public Color ProgressBarHoverForeColor { get; set; }
        public Color ProgressBarHoverBorderColor { get; set; }
        public Color ProgressBarHoverInsideTextColor { get; set; }



        // ScrollBar Colors
        public Color ScrollBarBackColor { get; set; }
        public Color ScrollBarThumbColor { get; set; }
        public Color ScrollBarTrackColor { get; set; }
        public Color ScrollBarHoverThumbColor { get; set; }
        public Color ScrollBarHoverTrackColor { get; set; }
        public Color ScrollBarActiveThumbColor { get; set; }

        // Status Bar Colors
        public Color StatusBarBackColor { get; set; }
        public Color StatusBarForeColor { get; set; }
        public Color StatusBarBorderColor { get; set; }
        public Color StatusBarHoverBackColor { get; set; }
        public Color StatusBarHoverForeColor { get; set; }
        public Color StatusBarHoverBorderColor { get; set; }
        // Textbox Link colors
        public Color LinkColor { get; set; }
        public Color VisitedLinkColor { get; set; }
        public Color HoverLinkColor { get; set; }
        // ToolTip colors

        public Color ToolTipBackColor { get; set; }
        public Color ToolTipForeColor { get; set; }
        public Color ToolTipBorderColor { get; set; }
        public Color ToolTipShadowColor { get; set; }
        public Color ToolTipShadowOpacity { get; set; }
        public Color ToolTipTextColor { get; set; }
        public Color ToolTipLinkColor { get; set; }
        public Color ToolTipLinkHoverColor { get; set; }
        public Color ToolTipLinkVisitedColor { get; set; }
        // Tab colors
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
        // Dialog colors
        public Color DialogBackColor { get; set; }
        public Color DialogForeColor { get; set; }

        // give me  Dialog Button colors like ok , no ,yes, cancel and others
        // i want to create font properties for each button
        public Font DialogYesButtonFont { get; set; }
        public Font DialogNoButtonFont { get; set; }
        public Font DialogOkButtonFont { get; set; }
        public Font DialogCancelButtonFont { get; set; }
        public Font DialogWarningButtonFont { get; set; }
        public Font DialogErrorButtonFont { get; set; }
        public Font DialogInformationButtonFont { get; set; }
        public Font DialogQuestionButtonFont { get; set; }
        public Font DialogHelpButtonFont { get; set; }
        public Font DialogCloseButtonFont { get; set; }
        public Font DialogYesButtonHoverFont { get; set; }
        public Font DialogNoButtonHoverFont { get; set; }
        public Font DialogOkButtonHoverFont { get; set; }
            
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






        // Grid Colors
        // Create properties for grid colors like header, row, cell, and other properties
        // i want to create properties for each color and give them default values
        // go ahead
        public Font GridHeaderFont { get; set; }
        public Font GridRowFont { get; set; }
        public Font GridCellFont { get; set; }
        public Font GridCellSelectedFont { get; set; }
        public Font GridCellHoverFont { get; set; }
        public Font GridCellErrorFont { get; set; }
  
        public Font GridColumnFont { get; set; }


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

        // Card Colors
        public Color CardTextForeColor { get; set; }
        public Color CardBackColor { get; set; }
        public Color CardTitleForeColor { get; set; }
        public Color CardSubTitleForeColor { get; set; }
        public TypographyStyle CardHeaderStyle { get; set; }
        public TypographyStyle CardparagraphStyle { get; set; }
        public TypographyStyle CardSubTitleStyle { get; set; }
        public Color CardrGradiantStartColor { get; set; }
        public Color CardGradiantEndColor { get; set; }
        public Color CardGradiantMiddleColor { get; set; }
        public Color CardGradiantDirection { get; set; }

        // Side Menu Colors
        public Font SideMenuTitleFont { get; set; }
        public Font SideMenuSubTitleFont { get; set; }
        public Font SideMenuTextFont { get; set; }
        public Color SideMenuBackColor { get; set; }
        public Color SideMenuHoverBackColor { get; set; }
        public Color SideMenuSelectedBackColor { get; set; }
        public Color SideMenuForeColor { get; set; }
        public Color SideMenuSelectedForeColor { get; set; }
        public Color SideMenuHoverForeColor { get; set; }
        public Color SideMenuBorderColor { get; set; }
        public Color SideMenuTitleTextColor { get; set; }
        public Color SideMenuTitleBackColor{ get; set; }
        public TypographyStyle SideMenuTitleStyle { get; set; }
        public Color SideMenuSubTitleTextColor { get; set; }
        public Color SideMenuSubTitleBackColor { get; set; }
        public TypographyStyle SideMenuSubTitleStyle { get; set; }
        public Color SideMenuGradiantStartColor { get; set; }
        public Color SideMenuGradiantEndColor { get; set; }
        public Color SideMenuGradiantMiddleColor { get; set; }
        public Color SideMenuGradiantDirection { get; set; }

        //Dashboard Colors
        public Font DashboardTitleFont { get; set; }
        public Font DashboardSubTitleFont { get; set; }
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
        public Color DashboardGradiantDirection { get; set; }


        // Chart colors
        public Font ChartTitleFont { get; set; }
        public Font ChartSubTitleFont { get; set; }
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


        // Navigation and Breadcrumbs Colors
        public Font NavigationTitleFont { get; set; }
        public Font NavigationSelectedFont { get; set; }
        public Font NavigationUnSelectedFont { get; set; }

        public Color NavigationBackColor { get; set; }
        public Color NavigationForeColor { get; set; }
        public Color NavigationHoverBackColor { get; set; }
        public Color NavigationHoverForeColor { get; set; }
        public Color NavigationSelectedBackColor { get; set; }
        public Color NavigationSelectedForeColor { get; set; }

        // Badge Colors
        public Color BadgeBackColor { get; set; }
        public Color BadgeForeColor { get; set; }
        public Color HighlightBackColor { get; set; }

        // Menu Colors with Menu Items
        public Font MenuTitleFont { get; set; }
        public Font MenuItemSelectedFont { get; set; }
        public Font MenuItemUnSelectedFont { get; set; }
        public Color MenuBackColor {  get; set; }
        public Color MenuForeColor { get; set; }
        public Color MenuBorderColor { get; set; }
        public Color MenuMainItemForeColor { get; set; }
        public Color MenuMainItemHoverForeColor { get; set; }
        public Color MenuMainItemHoverBackColor { get; set; }
        public Color MenuMainItemSelectedForeColor { get; set; }
        public Color MenuMainItemSelectedBackColor { get;set; }
        public Color MenuItemForeColor { get; set; }
        public Color MenuItemHoverForeColor { get; set; }
        public Color MenuItemHoverBackColor { get; set; }
        public Color MenuItemSelectedForeColor { get; set; }
        public Color MenuItemSelectedBackColor { get; set; }
        public Color MenuGradiantStartColor { get; set; }
        public Color MenuGradiantEndColor { get; set; }
        public Color MenuGradiantMiddleColor { get; set; }
        public Color MenuGradiantDirection { get; set; }

        // Tree Color with TreeItems and nodes
        public Font TreeTitleFont { get; set; }
        public Font TreeNodeSelectedFont { get; set; }
        public Font TreeNodeUnSelectedFont { get; set; }
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

        // Calendar Colors
        public Font CalendarTitleFont { get; set; }
        public Font CalendarSelectedFont { get; set; }
        public Font CalendarUnSelectedFont { get; set; }
        public Color CalendarBackColor { get; set; }
        public Color CalendarForeColor { get; set; }
        public Color CalendarTodayForeColor { get; set; }
        public Color CalendarBorderColor { get; set; }
        public Color CalendarHoverBackColor { get; set; }
        public Color CalendarHoverForeColor { get; set; }

        // List Colors
        public Font ListTitleFont { get; set; }
        public Font ListSelectedFont { get; set; }
        public Font ListUnSelectedFont { get; set; }
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

        // Star Rating Colors
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

        // Stats Card Colors
        public Font StatsTitleFont { get; set; }
        public Font StatsSelectedFont { get; set; }
        public Font StatsUnSelectedFont { get; set; }
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

        // switch control colors
        public Font SwitchTitleFont { get; set; }
        public Font SwitchSelectedFont { get; set; }
        public Font SwitchUnSelectedFont { get; set; }
        public Color SwitchBackColor { get; set; }
        public Color SwitchBorderColor { get; set; }
        public Color SwitchForeColor { get; set; }
        public Color SwitchSelectedBackColor { get; set; }
        public Color SwitchSelectedBorderColor { get; set; }
        public Color SwitchSelectedForeColor { get; set; }
        public Color SwitchHoverBackColor { get; set; }
        public Color SwitchHoverBorderColor { get; set; }
        public Color SwitchHoverForeColor { get; set; }

        // Task Card control Colors
        public Font TaskCardTitleFont { get; set; }
        public Font TaskCardSelectedFont { get; set; }
        public Font TaskCardUnSelectedFont { get; set; }
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



        // Testimony Colors
        public Font TestimoniaTitleFont { get; set; }
        public Font TestimoniaSelectedFont { get; set; }
        public Font TestimoniaUnSelectedFont { get; set; }
        public Color TestimonialBackColor { get; set; } = Color.White;
        public Color TestimonialTextColor { get; set; } = Color.Black;
        public Color TestimonialNameColor { get; set; } = Color.DarkBlue;
        public Color TestimonialDetailsColor { get; set; } = Color.Gray;
        public Color TestimonialDateColor { get; set; } = Color.DarkGray;
        public Color TestimonialRatingColor { get; set; } = Color.Gold;
        public Color TestimonialStatusColor { get; set; } = Color.Green;
        //Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.White;
        public Color CompanyTitleColor { get; set; } = Color.Black;
        public Color CompanySubtitleColor { get; set; } = Color.DarkBlue;
        public Color CompanyDescriptionColor { get; set; } = Color.Gray;
        public Color CompanyLinkColor { get; set; } = Color.Gray;
        public Color CompanyButtonBackgroundColor { get; set; } = Color.Blue;
        public Color CompanyButtonTextColor { get; set; } = Color.White;

        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.Black;
        public Color CompanyLogoBackgroundColor { get; set; } = Color.Gray;

        // New Login properties
        public Color LoginPopoverBackgroundColor { get; set; } = Color.White;
        public Color LoginTitleColor { get; set; } = Color.Black;
        public Color LoginSubtitleColor { get; set; } = Color.DarkBlue;
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public Color LoginLinkColor { get; set; } = Color.Blue;
        public Color LoginButtonBackgroundColor { get; set; } = Color.Blue;
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.Black;
        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;




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
